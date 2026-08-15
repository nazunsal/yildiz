using yildiz.entities.Concrete;

namespace yildiz.web.Models;

public class CartProduct
{
    public Product Product { get; set; } = null!;

    public int Quantity { get; set; }
}