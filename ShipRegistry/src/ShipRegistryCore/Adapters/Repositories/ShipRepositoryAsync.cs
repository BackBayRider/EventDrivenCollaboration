using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShipRegistryCore.Adapters.Db;
using ShipRegistryCore.Application;
using ShipRegistryCore.Ports.Repositories;

namespace ShipRegistryCore.Adapters.Repositories
{
    public class ShipRepositoryAsync : IRepositoryAsync<Ship>
    {
        private readonly ShipRegistryDbContext _uow;

        public ShipRepositoryAsync(ShipRegistryDbContext uow)
        {
            _uow = uow;
        }

        public async Task<Ship> AddAsync(Ship newEntity, CancellationToken ct = default(CancellationToken))
        {
            var savedItem = _uow.Ships.Add(newEntity);
            await _uow.SaveChangesAsync(ct);
            return savedItem.Entity;
        }

        public async Task DeleteAsync(Guid shipId, CancellationToken ct = default(CancellationToken))
        {
            var toDoItem = await _uow.Ships.SingleAsync(t => t.Id == new Id(shipId), ct);
            _uow.Remove(toDoItem);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task DeleteAllAsync(CancellationToken ct = default(CancellationToken))
        {
            _uow.Ships.RemoveRange(await _uow.Ships.ToListAsync(ct));
            await _uow.SaveChangesAsync(ct);
        }

        public async Task<Ship> GetAsync(Guid entityId, CancellationToken ct = new CancellationToken())
        {
            return await _uow.Ships.SingleAsync(t => t.Id == new Id(entityId), ct);
        }

        public async Task UpdateAsync(Ship updatedEntity, CancellationToken ct = new CancellationToken())
        {
            await _uow.SaveChangesAsync(ct);
        }
    }
}