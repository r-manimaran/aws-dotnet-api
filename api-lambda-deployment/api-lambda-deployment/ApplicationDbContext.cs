using api_lambda_deployment.Entities;
using Microsoft.EntityFrameworkCore;

namespace api_lambda_deployment;

public class ApplicationDbContext: DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
    {
        
    }
    public DbSet<ShortenedUrl> shortenedUrls {get;set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<ShortenedUrl>(builder =>{
                builder.Property(p=>p.Code).HasMaxLength(10);
                builder.HasIndex(p=>p.Code).IsUnique();
            });
    }

}