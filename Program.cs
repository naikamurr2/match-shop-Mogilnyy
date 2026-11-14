using Microsoft.EntityFrameworkCore;

Console.WriteLine("Магазин спичек «Чирк»: проект запущен!");

// Дальше — код по шагам задания.
using var db = new ShopContext();
db.Database.EnsureDeleted();   // удалить базу с прошлого запуска
db.Database.EnsureCreated();   // создать базу и таблицы по классам

Console.WriteLine("=== Шаг 3 ===");

var ivan   = new Customer { Name = "Иван Петров",     Phone = "+7 900 111-22-33", City = "Омск" };
var olga   = new Customer { Name = "Ольга Смирнова",  Phone = "+7 900 444-55-66", City = "Томск" };
var dmitry = new Customer { Name = "Дмитрий Орлов",   Phone = "+7 900 777-88-99", City = "Омск" };
var anna   = new Customer { Name = "Анна Козлова",    Phone = "+7 900 123-45-67", City = "Новосибирск" };
var sergey = new Customer { Name = "Сергей Волков",   Phone = "+7 900 987-65-43", City = "Омск" };
db.Customers.AddRange(ivan, olga, dmitry, anna, sergey);

var household = new Product { Name = "Хозяйственные", Price = 40m,  MatchesPerBox = 50, Stock = 200 };
var souvenir  = new Product { Name = "Сувенирные",    Price = 85m,  MatchesPerBox = 30, Stock = 40,
                              Description = "Яркие головки, подходят для подарков" };
var fireplace = new Product { Name = "Каминные",      Price = 150m, MatchesPerBox = 45, Stock = 50,
                              Description = "Длинные, для розжига камина" };
var tourist   = new Product { Name = "Туристические", Price = 180m, MatchesPerBox = 25, Stock = 30,
                              Description = "Влагостойкие" };
var hunting   = new Product { Name = "Охотничьи",     Price = 220m, MatchesPerBox = 20, Stock = 10,
                              Description = "Горят даже под дождём" };
db.Products.AddRange(household, souvenir, fireplace, tourist, hunting);

db.Orders.AddRange(
    new Order { Customer = ivan,   Product = fireplace, Quantity = 3,  OrderDate = DateTime.UtcNow.AddDays(-6) },
    new Order { Customer = ivan,   Product = hunting,   Quantity = 1,  OrderDate = DateTime.UtcNow.AddDays(-4) },
    new Order { Customer = dmitry, Product = household, Quantity = 10, OrderDate = DateTime.UtcNow.AddDays(-3) },
    new Order { Customer = anna,   Product = tourist,   Quantity = 2,  OrderDate = DateTime.UtcNow.AddDays(-2) },
    new Order { Customer = sergey, Product = fireplace, Quantity = 1,  OrderDate = DateTime.UtcNow.AddDays(-1) }
);

db.SaveChanges();   // только здесь данные уходят в базу

Console.WriteLine("=== Шаг 4 ===");

// Пример: покупатели из Омска по алфавиту
var omsk = db.Customers
    .Where(c => c.City == "Омск")
    .OrderBy(c => c.Name)
    .ToList();
foreach (var c in omsk)
    Console.WriteLine($"{c.Name}, {c.Phone}");

// Задание 1: спички дешевле 100 ₽ от дешёвых к дорогим
var cheap = db.Products
    .Where(p => p.Price < 100)
    .OrderBy(p => p.Price)
    .ToList();
Console.WriteLine("Дешевле 100 ₽:");
foreach (var p in cheap)
    Console.WriteLine($"{p.Name} — {p.Price} ₽");

// Задание 2: сколько видов спичек есть на складе
var inStockCount = db.Products.Count(p => p.Stock > 0);
Console.WriteLine($"Видов спичек на складе: {inStockCount}");

// Задание 3: три самых дорогих товара
var top3 = db.Products
    .OrderByDescending(p => p.Price)
    .Take(3)
    .ToList();
Console.WriteLine("Три самых дорогих:");
foreach (var p in top3)
    Console.WriteLine($"{p.Name} — {p.Price} ₽");

// Усложнение: все заказы одной строкой и общая выручка
var allOrders = db.Orders
    .Include(o => o.Customer)
    .Include(o => o.Product)
    .ToList();
Console.WriteLine("Все заказы:");
foreach (var o in allOrders)
    Console.WriteLine($"{o.Customer.Name} — {o.Product.Name} — {o.Quantity} шт. — {o.Product.Price * o.Quantity} ₽");

var revenue = allOrders.Sum(o => o.Product.Price * o.Quantity);
Console.WriteLine($"Общая выручка: {revenue} ₽");

Console.WriteLine("=== Шаг 5 ===");

// Пример: новый телефон Ивана
var ivanToChange = db.Customers.First(c => c.Name == "Иван Петров");
ivanToChange.Phone = "+7 900 999-00-00";
db.SaveChanges();

// Задание 1: все спички дороже на 10%
foreach (var p in db.Products.ToList())
    p.Price *= 1.1m;

// Задание 2: один товар закончился
var soldOut = db.Products.First(p => p.Name == "Сувенирные");
soldOut.Stock = 0;

db.SaveChanges();   // один SaveChanges отправит все изменения

static void PlaceOrder(ShopContext db, Customer buyer, Product item, int boxes)
{
    if (item.Stock < boxes)
    {
        Console.WriteLine("Недостаточно на складе");
        return;   // ничего не меняем
    }

    db.Orders.Add(new Order
    {
        Customer = buyer,
        Product = item,
        Quantity = boxes,
        OrderDate = DateTime.UtcNow
    });
    item.Stock -= boxes;   // уменьшаем остаток

    // Заказ и новый остаток сохраняются вместе, одним SaveChanges.
    // Это одна транзакция: либо записалось и то и другое, либо ничего.
    // Если сохранять по отдельности, при сбое между ними получится заказ без списания
    // со склада (или списание без заказа), и учёт разойдётся с реальностью.
    db.SaveChanges();

    Console.WriteLine($"Заказ оформлен: {buyer.Name} — {item.Name} — {boxes} шт. Остаток: {item.Stock}");
}

// Проверка двух случаев
var buyerAnna    = db.Customers.First(c => c.Name == "Анна Козлова");
var fireplaceBox = db.Products.First(p => p.Name == "Каминные");
var huntingBox   = db.Products.First(p => p.Name == "Охотничьи");

PlaceOrder(db, buyerAnna, fireplaceBox, 2);    // хватает: 50 → 48
PlaceOrder(db, buyerAnna, huntingBox, 50);     // не хватает: на складе только 10

// Name: text NOT NULL, а Description: text (без NOT NULL).
// string без "?" - обязательное поле, база не разрешит оставить его пустым (NULL).
// string? - необязательное поле: описание может отсутствовать, поэтому NULL разрешён.

// В C# выбран тип DateTime, в PostgreSQL столбец OrderDate получился timestamp with time zone.

// DateTime.Now - местное время (Kind=Local), Npgsql не принимает его для timestamptz.
// DateTime.UtcNow - время в UTC (Kind=Utc), его PostgreSQL принимает.