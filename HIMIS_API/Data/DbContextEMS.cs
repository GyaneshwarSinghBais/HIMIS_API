using HIMIS_API.Models.EMS;
using HIMIS_API.Models.WebCGMSC;
using Microsoft.EntityFrameworkCore;

namespace HIMIS_API.Data
{
    public class DbContextEMS : DbContext
    {
        public DbContextEMS(DbContextOptions<DbContextEMS> option) : base(option)
        {

        }
        public DbSet<GetEqpTenderDTO> GetEqpTenderDbSet { get; set; }
        public DbSet<GetEqpRCDTO> GetEqpRCDbSet { get; set; }
        public DbSet<GetTotalTendersByStatusDTO> GetTotalTendersByStatusDbSet { get; set; }
        public DbSet<GetTenderDetailDTO> GetTenderDetailDbSet { get; set; }
        public DbSet<EqToBeTenderDTO> EqToBeTenderDbSet { get; set; }
        




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Inform EF that DTOs doesn't have a key
            modelBuilder.Entity<GetEqpRCDTO>().HasNoKey();
            modelBuilder.Entity<GetTenderDetailDTO>().HasNoKey();

        }
    }
}
