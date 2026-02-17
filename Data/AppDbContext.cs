using Microsoft.EntityFrameworkCore;

namespace JobTracker.api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<JobApplication> JobApplications => Set<JobApplication>();
    public DbSet<User> Users => Set<User>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<JobApplication>()
            .Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        modelBuilder.Entity<JobApplication>()
            .HasQueryFilter(x => !x.IsDeleted);
    }


}

