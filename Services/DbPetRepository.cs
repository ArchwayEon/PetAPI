using Microsoft.EntityFrameworkCore;
using PetAPI.Models.Entities;

namespace PetAPI.Services;

public class DbPetRepository : IPetRepository
{
    private readonly ApplicationDbContext _db;

    public DbPetRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ICollection<Pet>> ReadAllAsync()
    {
        return await _db.Pets.AsNoTracking().ToListAsync();
    }

    public async Task<Pet?> ReadAsync(int id)
    {
        return await _db.Pets.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Pet> CreateAsync(Pet newPet)
    {
        _db.Pets.Add(newPet);
        await _db.SaveChangesAsync();
        return newPet;
    }

    public async Task<bool> UpdateAsync(int oldId, Pet updatedPet)
    {
        var existing = await _db.Pets.FirstOrDefaultAsync(p => p.Id == oldId);
        if (existing is null)
        {
            return false;
        }

        existing.Name = updatedPet.Name;
        existing.Weight = updatedPet.Weight;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _db.Pets.FirstOrDefaultAsync(p => p.Id == id);
        if (existing is null)
        {
            return false;
        }

        _db.Pets.Remove(existing);
        await _db.SaveChangesAsync();
        return true;
    }
}
