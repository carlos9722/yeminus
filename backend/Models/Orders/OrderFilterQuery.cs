namespace ServiceOrders.Api.Models.Orders;

public sealed class OrderFilterQuery
{
    public string? Status { get; set; }
    public string? TechnicianName { get; set; }
    public string? TechnicianSpecialty { get; set; }
    public string? ClientName { get; set; }
    public string? ClientIdentityDoc { get; set; }
    public DateOnly? CreatedFrom { get; set; }
    public DateOnly? CreatedTo { get; set; }
}
