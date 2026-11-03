using PetAPI.Models.Entities;

namespace PetAPI.Services;

public interface IPetRepository
{
    Task<ICollection<Pet>> ReadAllAsync();
    Task<Pet?> ReadAsync(int id);
    Task<Pet> CreateAsync(Pet newPet);
    Task<bool> UpdateAsync(int oldId, Pet updatedPet);
    Task<bool> DeleteAsync(int id);
}
