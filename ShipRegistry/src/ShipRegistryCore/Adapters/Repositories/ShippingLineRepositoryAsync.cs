using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShipRegistryCore.Adapters.Db;
using ShipRegistryCore.Application;
using ShipRegistryCore.Ports.Repositories;

namespace ShipRegistryCore.Adapters.Repositories
{
    public class ShippingLineRepositoryAsync : IRepositoryAsync<ShippingLine>
    {
        private readonly ShipRegistryDbContext _uow;

        public ShippingLineRepositoryAsync(ShipRegistryDbContext uow)
        {
            _uow = uow;
        }

        public async Task<ShippingLine> AddAsync(ShippingLine newEntity, CancellationToken ct = default(CancellationToken))
        {
            var savedItem = _uow.Lines.Add(newEntity);
            await _uow.SaveChangesAsync(ct);
            return savedItem.Entity;
        }

        public async Task DeleteAsync(Id lineId, CancellationToken ct = default(CancellationToken))
        {
            var toDoItem = await _uow.Lines.SingleAsync(t => t.Id == lineId, ct);
            _uow.Remove(toDoItem);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task DeleteAllAsync(CancellationToken ct = default(CancellationToken))
        {
            _uow.Lines.RemoveRange(await _uow.Lines.ToListAsync(ct));
            await _uow.SaveChangesAsync(ct);
        }

        public async Task<ShippingLine> GetAsync(Id entityId, CancellationToken ct = new CancellationToken())
        {
            return await _uow.Lines.SingleOrDefaultAsync(t => t.Id == entityId, ct);
        }

        public async Task UpdateAsync(ShippingLine updatedEntity, CancellationToken ct = new CancellationToken())
        {
            await _uow.SaveChangesAsync(ct);
        }
    }
}
