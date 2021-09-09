using Microsoft.EntityFrameworkCore;
using OTAServices.Business.Entities.Common.OasisRequestEnrichment;
using OTAServices.Business.Entities.SimManagement;

namespace OTAServices.DataCore.MaximityDb
{
    public class MaximityDbContext : DbContext
    {
        #region [ Private fields ]

        private readonly string _connectionString;

        #endregion

        #region [ Constructor ]

        public MaximityDbContext(string connectionString)
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
            modelBuilder.Query<SimInfo>();
            modelBuilder.Query<ProductInfo>();
        }

        #endregion

        #region [ DbSets ]

        public virtual DbQuery<SimInfo> SimInfo { get; set; }

        public virtual DbQuery<ProductInfo> ProductInfo { get; set; }
        #endregion
    }
}
