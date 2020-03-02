using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using N8T.Domain;

namespace N8T.Infrastructure.Data
{
    public abstract class AppDbContextBase : DbContext, IUnitOfWork, IDomainEventContext
    {
        protected IDbContextTransaction CurrentTransaction;

        protected AppDbContextBase(DbContextOptions options) : base(options)
        {
        }

        public async Task BeginTransactionAsync(CancellationToken token)
        {
            if (CurrentTransaction != null)
            {
                return;
            }

            CurrentTransaction = await Database
                .BeginTransactionAsync(IsolationLevel.ReadCommitted, token)
                .ConfigureAwait(false);
        }

        public async Task CommitTransactionAsync(CancellationToken token)
        {
            try
            {
                await SaveChangesAsync(token).ConfigureAwait(false);

                CurrentTransaction?.Commit();
            }
            catch
            {
                await RollbackTransaction(token);
                throw;
            }
            finally
            {
                if (CurrentTransaction != null)
                {
                    CurrentTransaction.Dispose();
                    CurrentTransaction = null;
                }
            }
        }

        public async Task RollbackTransaction(CancellationToken token)
        {
            try
            {
                await CurrentTransaction.RollbackAsync(token);
            }
            finally
            {
                if (CurrentTransaction != null)
                {
                    await CurrentTransaction.DisposeAsync();
                    CurrentTransaction = null;
                }
            }
        }

        public IEnumerable<IDomainEvent> GetDomainEvents()
        {
            var aggregateRoots = ChangeTracker
                .Entries<AggregateRoot>()
                .Where(x =>
                    x.Entity.DomainEvents != null &&
                    x.Entity.DomainEvents.Any())
                .ToList();

            var domainEvents = aggregateRoots.SelectMany(x => x.Entity.DomainEvents);

            aggregateRoots.ForEach(entity => entity.Entity.DomainEvents.Clear());

            return domainEvents;
        }
    }
}