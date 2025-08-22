using BlazorWasmTemplate.Domain.Entities;

namespace BlazorWasmTemplate.Application.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllAsync();

        Task<User?> GetByIdAsync(Guid id);

        Task AddAsync(User user);

        Task UpdateAsync(User user);

        Task DeleteAsync(Guid id);
    }
}