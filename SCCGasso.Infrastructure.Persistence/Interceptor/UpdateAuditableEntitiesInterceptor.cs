using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using SCCGasso.Core.Domain.Common;
using System;
using SCC_Gasso.Core.Application.Dtos.Account;
using SCC_Gasso.Core.Application.Helpers;

namespace SCCGasso.Infrastructure.Persistence.Interceptor
{
    public class UpdateAuditableEntitiesInterceptor
    : SaveChangesInterceptor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private AuthenticationResponse? userViewModel;
        public UpdateAuditableEntitiesInterceptor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            userViewModel = _httpContextAccessor.HttpContext.Session.Get<AuthenticationResponse>("user");
            DbContext? dbContext = eventData.Context;
            if (dbContext == null)
            {
                return base.SavingChangesAsync(eventData, result, cancellationToken);
            }
            var entries = dbContext.ChangeTracker
                .Entries<IAuditableEntity>();
            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Property(a => a.CreatedDate).CurrentValue = DateTime.UtcNow;
                    if (userViewModel == null)
                        entry.Property(a => a.CreatedById).CurrentValue = "Default";
                    else
                        entry.Property(a => a.CreatedById).CurrentValue = userViewModel.Id;
                }
                if (entry.State == EntityState.Modified)
                {
                    entry.Property(a => a.ModifiedDate).CurrentValue = DateTime.UtcNow;
                    entry.Property(a => a.ModifiedById).CurrentValue = userViewModel?.Id;
                }
            }
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}
