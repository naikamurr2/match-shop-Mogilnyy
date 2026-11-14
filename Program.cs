using Microsoft.EntityFrameworkCore;

Console.WriteLine("Магазин спичек «Чирк»: проект запущен!");

// Дальше — код по шагам задания.
using var db = new ShopContext();
db.Database.EnsureDeleted();   // удалить базу с прошлого запуска
db.Database.EnsureCreated();   // создать базу и таблицы по классам

// Name: text NOT NULL, а Description: text (без NOT NULL).
// string без "?" - обязательное поле, база не разрешит оставить его пустым (NULL).
// string? - необязательное поле: описание может отсутствовать, поэтому NULL разрешён.

// В C# выбран тип DateTime, в PostgreSQL столбец OrderDate получился timestamp with time zone.