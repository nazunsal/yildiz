namespace yildiz.entities.Concrete;

using yildiz.entities.Abstract;
public class Category : IEntity
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public ICollection<Product>? Products { get; set; }
}