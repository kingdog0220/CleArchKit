using BlazorWasmTemplate.Domain.Entities;
using BlazorWasmTemplate.Domain.Repositories;

namespace BlazorWasmTemplate.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        public Task<List<User>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<User?> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task AddAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

    }
}