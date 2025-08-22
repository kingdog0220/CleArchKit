using BlazorWasmTemplate.Domain.Entities;

namespace BlazorWasmTemplate.Application.Cache
{
    public interface IUserCache
    {
        Task<IEnumerable<User>> GetAllAsync();

        Task<User?> GetByIdAsync(Guid id);

        Task RefreshAsync();

    }
}