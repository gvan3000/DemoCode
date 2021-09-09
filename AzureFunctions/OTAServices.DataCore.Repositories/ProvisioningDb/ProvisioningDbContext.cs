using Microsoft.EntityFrameworkCore;
using OTAServices.Business.Entities.Common.OasisRequestEnrichment;
using OTAServices.Business.Entities.ImsiManagement;
using OTAServices.Business.Entities.LeaseRequest;
using OTAServices.Business.Entities.SimManagement;
using OTAServices.DataCore.Repositories.ProvisioningDb.Configurations;

namespace OTAServices.DataCore.Repositories.ProvisioningDb
{
    public class ProvisioningDbContext : DbContext
    {
        #region [ Private fields ]

        private readonly string _connectionString;

        #endregion

        #region [ Constructor ]

        public ProvisioningDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        #endregion

        #region [ DbContext Overriden methods ]

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("ota");
            modelBuilder.Query<ImsiSponsorsStatus>();
            modelBuilder.Query<SimContent>();
            modelBuilder.Query<ImsiInfo>();
            modelBuilder.Query<SimProfileSponsor>();
            modelBuilder.Query<ProvisioningDataInfo>();
            modelBuilder.Query<UiccidSimProfileId>();
            modelBuilder.Query<OriginalTargetSimProfilePair>();
            modelBuilder.ApplyConfiguration(new SubscriberListLeaseRequestConfiguration());
        }

        #endregion

        #region [ DbSets ]

        public virtual DbQuery<ImsiSponsorsStatus> ImsiSponsorStatus { get; set; }

        public virtual DbQuery<SimContent> SimContent { get; set; }

        public virtual DbSet<SubscriberListLeaseRequest> SubscriberListLeaseRequest { get; set; }

        public virtual DbQuery<SimProfileSponsor> SimProfileSponsorMcc { get; set; }

        public virtual DbQuery<ImsiInfo> ImsiInfo { get; set; }

        public virtual DbQuery<ProvisioningDataInfo> ProvisioningDataInfo { get; set; }

        public virtual DbQuery<UiccidSimProfileId> UiccidSimProfileId { get; set; }

        public virtual DbQuery<OriginalTargetSimProfilePair> OriginalTargetSimProfilePair { get; set; }
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

        public void UpdateEntity(object entity)
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
