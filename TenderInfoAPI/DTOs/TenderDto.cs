namespace TenderInfoAPI.DTOs;

public class TenderDto
{
    public string Id { get; set; }
    public DateTime Date { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal AmountEur { get; set; }
    public List<SupplierDto> Suppliers { get; set; }
}
