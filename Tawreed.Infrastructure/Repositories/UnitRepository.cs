using Tawreed.Domain.Entities;
using Tawreed.Domain.Interfaces;
using Tawreed.Infrastructure.Data;

namespace Tawreed.Infrastructure.Repositories;

public class UnitRepository : GenericRepository<Unit>, IUnitRepository
{
    public UnitRepository(ApplicationDbContext context) : base(context)
    {
    }
}
