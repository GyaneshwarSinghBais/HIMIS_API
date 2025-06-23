using HIMIS_API.Models.EMS;
using HIMIS_API.Models.StockMgm;
using HIMIS_API.Models.WebCGMSC;
using Microsoft.EntityFrameworkCore;

namespace HIMIS_API.Data
{
    public class DbContextStockMgm : DbContext
    {
        public DbContextStockMgm(DbContextOptions<DbContextStockMgm> option) : base(option)
        {

        }
         public DbSet<CoverStatusDTO> CoverStatusDbSet { get; set; }
        public DbSet<CoverStatusDetailDTO> CoverStatusDetailDbSet { get; set; }
        public DbSet<CoverStatusTenderDetailDTO> CoverStatusTenderDetailDbSet { get; set; }
        









        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Inform EF that DTOs doesn't have a key
             modelBuilder.Entity<CoverStatusDetailDTO>().HasNoKey();
            modelBuilder.Entity<CoverStatusTenderDetailDTO>().HasNoKey();


        }
    }
}
