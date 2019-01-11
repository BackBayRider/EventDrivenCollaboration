using System;
using System.Threading;
using System.Threading.Tasks;
using ShipRegistryCore.Application;

namespace ShipRegistryCore.Ports.Repositories
{
    public interface IRepositoryAsync<T> where T : IEntity
    {
        Task<T> AddAsync(T newEntity, CancellationToken ct = default(CancellationToken));
        Task DeleteAsync(Guid entityId, CancellationToken ct = default(CancellationToken));
        Task DeleteAllAsync(CancellationToken ct = default(CancellationToken));
        Task<T> GetAsync(Id entityId, CancellationToken ct = default(CancellationToken));
        Task UpdateAsync(T updatedEntity, CancellationToken ct = default(CancellationToken));
    }
}