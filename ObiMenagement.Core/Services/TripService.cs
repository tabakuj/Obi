using Microsoft.Extensions.Logging;
using ObiMenagement.Core.Common;
using ObiMenagement.Core.Interfaces;
using ObiMenagement.Core.Models;

namespace ObiMenagement.Core.Services;
public class TripService : LongBaseService<Trip>, ITripService
{
    public TripService(IUnitOfWork unitOfWork, ILogger<TripService> logger) :
        base(unitOfWork, unitOfWork.TripRepository, logger)
    {
    }

    protected override async Task<bool> ValidateModel(Trip model, Response result)
    {
        if (model.Employee is null || model.Employee.Id == 0)
        {
            result.Exception = new ObiException(ErrorMessages.NotNull(nameof(model.Employee)));
            return true;
        }
        if (model.TruckBase is null || model.TruckBase.Id == 0)
        {
            result.Exception = new ObiException(ErrorMessages.NotNull(nameof(model.TruckBase)));
            return true;
        }
        if (model.TruckContainer is null || model.TruckContainer.Id == 0)
        {
            result.Exception = new ObiException(ErrorMessages.NotNull(nameof(model.TruckContainer)));
            return true;
        }
        model.Employee = await _unitOfWork.EmployeeRepository.FirstOrDefault(a => a.Id == model.Employee.Id);
        model.TruckBase = await _unitOfWork.TruckBaseRepository.FirstOrDefault(a => a.Id == model.TruckBase.Id);
        model.TruckContainer = await _unitOfWork.TruckContainerRepository.FirstOrDefault(a => a.Id == model.TruckContainer.Id);

        if (model.Id == 0 && model.Number == 0)
        {
            model.Number = await CalculateNumber(model.TruckBase.Id, model.TripDate);
        }
        return false;
    }

    public override async Task<Response<IEnumerable<Trip>>> GetAllAsync(string search = null)
    {
        var result = new Response<IEnumerable<Trip>>();

        try
        {
            result.Result = await _repository.WhereAsync(a => true, a => a.Employee, a => a.Employee.Person, a => a.TruckBase, a => a.TruckContainer);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "failed to delete the model");
            result.Exception = e;
            Logger.Instance.LogError(e);
        }
        return result;
    }

    public override async Task<Response<Trip>> GetByIdAsync(int id)
    {
        var result = new Response<Trip>();

        try
        {
            result.Result = await _repository.FirstOrDefault(a => a.Id == id, a => a.Employee, a => a.Employee.Person, a => a.TruckBase, a => a.TruckContainer);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "failed to delete the model");
            result.Exception = e;
            Logger.Instance.LogError(e);
        }
        return result;
    }

    public async Task<int> CalculateNumber(long truckId, DateTime dateTime)
    {
        dateTime = dateTime.Date;
        var currentNumber = (await _repository.WhereAsync(a => a.TruckBase.Id == truckId)).Count(a => a.TripDate.Date == dateTime.Date);
        return currentNumber + 1;
    }
}