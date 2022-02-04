using Microsoft.EntityFrameworkCore;

public class TestDbContext : DbContext
{
    public DbSet<Customer> Customers => Set<Customer>();

    public string DbPath { get; }

    public TestDbContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "test.db");
    }

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options
            .AddInterceptors(new EFTransactionInterceptor())
            .UseSqlite($"Data Source={DbPath}");
}

public class Customer
{
    public int CustomerId { get; set; }
    public string? Name { get; set; }
}