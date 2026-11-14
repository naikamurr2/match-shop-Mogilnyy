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

// Name: text NOT NULL, а Description: text (без NOT NULL).
// string без "?" - обязательное поле, база не разрешит оставить его пустым (NULL).
// string? - необязательное поле: описание может отсутствовать, поэтому NULL разрешён.

// В C# выбран тип DateTime, в PostgreSQL столбец OrderDate получился timestamp with time zone.

// DateTime.Now - местное время (Kind=Local), Npgsql не принимает его для timestamptz.
// DateTime.UtcNow - время в UTC (Kind=Utc), его PostgreSQL принимает.