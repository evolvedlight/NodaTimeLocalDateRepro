using Microsoft.EntityFrameworkCore;
using NodaTime;

using var db = new ReproContext();

await db.Database.EnsureCreatedAsync();

// Note: This sample requires the database to be created before running.
Console.WriteLine($"Database path: {db.Database.GetConnectionString()}.");

// Create
Console.WriteLine("Inserting a new entity");
db.Add(new EntityWithLocalDate { TheDate = new LocalDate(2023, 1, 4) });
db.SaveChanges();

var resultsLocalDate = await db.TestEntity
    .Select(b => new TestThingLocalDate
    {
        Date = b.TheDate
    })
    .ToListAsync();

Console.WriteLine($"resultsLocalDate: {resultsLocalDate[0].Date}");

var resultsDateTime = await db.TestEntity
    .Select(b => new TestThingDateTime
    {
        Date = b.TheDate.ToDateTimeUnspecified()
    })
    .ToListAsync();

Console.WriteLine($"resultsLocalDate: {resultsDateTime[0].Date}");

public class ReproContext : DbContext
{
    public DbSet<EntityWithLocalDate> TestEntity { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source=repro_{SystemClock.Instance.GetCurrentInstant().ToUnixTimeTicks()}.db", c=> c.UseNodaTime());
}

public class EntityWithLocalDate
{
    public long Id { get; set; }
    public LocalDate TheDate { get; set; }
}

public class TestThingDateTime
{
    public DateTime Date { get; set; }
}

public class TestThingLocalDate
{
    public LocalDate Date { get; set; }
}