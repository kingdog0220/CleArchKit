using BlazorWasmTemplate.Domain.Users.Entities;

namespace BlazorWasmTemplate.Domain.Users.Repositories
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllAsync();

        Task<User?> GetByIdAsync(Guid id);

        Task AddAsync(User user);

        Task UpdateAsync(User user);

        Task DeleteAsync(Guid id);
    }
}