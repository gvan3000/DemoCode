using Microsoft.EntityFrameworkCore;
using OTAServices.Business.Entities.OTACampaign;
using OTAServices.Business.Entities.OTACampaignDeleteImsi;
using OTAServices.Business.Entities.OTACampaignSubscribers;

namespace OTAServices.DataCore.Repositories.OtaDb
{
    public class OtaDbContext : DbContext
    {
        #region [ Private fields ]

        private readonly string _connectionString;

        #endregion

        #region [ Constructor ]

        public OtaDbContext(string connectionString)
        {
            _connectionString = connectionString;
            //Set timeout to 10min
            this.Database.SetCommandTimeout(600);
        }

        #endregion

        #region [ DbSets ]

        public virtual DbSet<Campaign> Campaign { get; set; }

        public virtual DbSet<OasisRequest> OasisRequest { get; set; }

        public virtual DbSet<DeleteIMSICallback> DeleteIMSICallback { get; set; }

        #endregion

        #region [ DbContext Overriden methods ]

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(_connectionString);
        }

        #endregion

        #region [ Public methods ]

        public void AddEntity(object entity)
        {
            var entry = Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                Attach(entity);
            }
            entry.State = EntityState.Added;
        }

        public void RemoveEntity(object entity)
        {
            var entry = Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                Attach(entity);
            }
            entry.State = EntityState.Deleted;
        }

        internal void UpdateEntity(object entity)
        {
            var entry = Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                Attach(entity);
            }
            entry.State = EntityState.Modified;
        }

        #endregion
    }
}
