using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Payroll.Application.Common.Interfaces;
using Payroll.Domain.Common;

namespace Payroll.Infrastructure.Persistence.Interceptors
{
    public class AuditableEntityInterceptor : SaveChangesInterceptor
    {
        private readonly ICurrentUser _currentUser;

        public AuditableEntityInterceptor(ICurrentUser currentUser)
        {
            _currentUser = currentUser;
        }
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            Apply(eventData.Context);
            return base.SavingChanges(eventData, result);
        }
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
       DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            Apply(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
        private void Apply(DbContext? context)
        {
            if (context is null) return;
            var now = DateTime.UtcNow;
            var user = _currentUser.UserId;

            foreach (var entry in context.ChangeTracker.Entries())
            {
                if (entry is { Entity: BaseEntity entity })
                {
                    if (entry.State == EntityState.Added)
                    {
                        entity.CreatedAt = now;
                        entity.CreatedBy = user;
                    }

                    if (entry.State == EntityState.Deleted)
                    {
                        entry.State = EntityState.Modified;
                        entity.IsDeleted = true;
                    }

                    if (entry.State == EntityState.Modified)
                    {
                        entity.UpdatedAt = now;
                        entity.UpdatedBy = user;
                    }

                }
            }
        }
    }
}
