using Microsoft.EntityFrameworkCore;
using PetAPI.Models.Entities;

namespace PetAPI.Services;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Pet> Pets => Set<Pet>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Pet>().HasData(
            new Pet { Id = 1, Name = "Buddy", Weight = 30.5f },
            new Pet { Id = 2, Name = "Whiskers", Weight = 4.2f },
            new Pet { Id = 3, Name = "Goldie", Weight = 0.1f },
            new Pet { Id = 4, Name = "Thumper", Weight = 1.8f },
            new Pet { Id = 5, Name = "Max", Weight = 38.0f });
    }
}
