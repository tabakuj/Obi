namespace ObiMenagement.Core.Models;

public class RoadExpense:IdLongBaseModel
{
    public virtual Trip Trip { get; set; }
    public virtual RoadData? RoadData { get; set; }
    public ExpenseType ExpenseType { get; set; }
    public decimal Quantity { get; set; }
    public Payment Payment { get; set; }
    public decimal Price { get; set; }
    public string Name { get; set; }
    public string? Note { get; set; }
    public Country? Country { get; set; }
}
