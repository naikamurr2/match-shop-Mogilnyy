// Классы таблиц: Customer, Product, Order (шаги 1–2).
public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Phone { get; set; } = "";
    public string City { get; set; } = "";
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Price { get; set; }          // цена коробка в рублях
    public int MatchesPerBox { get; set; }      // сколько спичек в коробке
    public int Stock { get; set; }              // сколько коробок на складе
    public string? Description { get; set; }    // описание может отсутствовать (знак ?)
}