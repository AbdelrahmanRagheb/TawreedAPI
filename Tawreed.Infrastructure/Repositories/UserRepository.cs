using Microsoft.EntityFrameworkCore;
using Tawreed.Domain.Entities;
using Tawreed.Domain.Interfaces;
using Tawreed.Infrastructure.Data;

namespace Tawreed.Infrastructure.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<bool> IsEmailUniqueAsync(string email, CancellationToken cancellationToken = default)
    {
        return !await DbSet.AnyAsync(u => u.Email == email, cancellationToken);
    }
}
