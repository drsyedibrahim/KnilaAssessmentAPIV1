using KAAPI.DataObject.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace KAAPI.DataObject.Context
{
    public interface IApplicationDBContext
    {
        DatabaseFacade Database { get; }
        public DbSet<ContactEntity> Contact { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}