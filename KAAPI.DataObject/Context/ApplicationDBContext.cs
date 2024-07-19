using KAAPI.DataObject.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace KAAPI.DataObject.Context
{
    public class ApplicationDBContext : DbContext, IApplicationDBContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options) { }
        public DbSet<ContactEntity> Contact { get; set; }
        public EntityEntry EntityEntry { get; set; }
    }
}