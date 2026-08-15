namespace yildiz.entities.Concrete;

public class Order
{
    public int OrderId { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public string CustomerEmail { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public decimal TotalPrice { get; set; }

    public DateTime OrderDate { get; set; }

    public string Status { get; set; } = "Hazırlanıyor";

    public List<OrderItem> OrderItems { get; set; } = new();
}