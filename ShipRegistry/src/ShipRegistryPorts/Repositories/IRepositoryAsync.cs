using System.Threading;
using System.Threading.Tasks;
using ShipRegistryApplication;

namespace ShipRegistryPorts.Repositories
{
    public interface IRepositoryAsync<T> where T : IEntity
    {
        Task<T> AddAsync(T newEntity, CancellationToken ct = default(CancellationToken));
        Task DeleteAsync(Id entityId, CancellationToken ct = default(CancellationToken));
        Task DeleteAllAsync(CancellationToken ct = default(CancellationToken));
        Task<T> GetAsync(Id entityId, CancellationToken ct = default(CancellationToken));
        Task UpdateAsync(T updatedEntity, CancellationToken ct = default(CancellationToken));
    }
}