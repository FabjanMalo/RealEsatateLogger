using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace RealEsatateLogger;

public class EstateLoggerDbContext : DbContext
{
    public EstateLoggerDbContext(DbContextOptions<EstateLoggerDbContext> options) : base(options)
    { }
    public DbSet<ErrorLog> ErrorLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}
