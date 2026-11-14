// Классы таблиц: Customer, Product, Order (шаги 1–2).
public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Phone { get; set; } = "";
    public string City { get; set; } = "";
    public List<Order> Orders { get; set; } = new();
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Price { get; set; }          // цена коробка в рублях
    public int MatchesPerBox { get; set; }      // сколько спичек в коробке
    public int Stock { get; set; }              // сколько коробок на складе
    public string? Description { get; set; }    // описание может отсутствовать (знак ?)
    public List<Order> Orders { get; set; } = new();
}

public class Order
{
    public int Id { get; set; }
    public int Quantity { get; set; }                  // сколько коробок
    public DateTime OrderDate { get; set; }            // дата и время заказа (усложнение)

    public int CustomerId { get; set; }                // внешний ключ на покупателя
    public Customer Customer { get; set; } = null!;

    public int ProductId { get; set; }                 // внешний ключ на товар
    public Product Product { get; set; } = null!;
}