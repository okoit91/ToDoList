using App.Domain;
using Microsoft.EntityFrameworkCore;
using Task = App.Domain.Task;


namespace App.DAL.EF;

public class AppDbContext : DbContext
{
    public DbSet<Task> Tasks { get; set; } = default!;
    
    public DbSet<TaskHistory> TaskHistories { get; set; } = default!;
    
    public DbSet<ToDoList> ToDoLists { get; set; } = default!;
    
    
    public AppDbContext(DbContextOptions options): base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        foreach (var relationship in builder.Model
                     .GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
            {
                var properties = entry.Properties.Where(p =>
                    p.Metadata.ClrType == typeof(DateTime) || p.Metadata.ClrType == typeof(DateTime?));
                foreach (var prop in properties)
                {
                    if (prop.CurrentValue != null) // Check for null if nullable
                    {
                        var dateTime = (DateTime)prop.CurrentValue;
                        prop.CurrentValue = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
                    }
                }
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }

}