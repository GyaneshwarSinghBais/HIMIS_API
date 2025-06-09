using HIMIS_API.Models.Tender;
using HIMIS_API.Models.WebCGMSC;
using Microsoft.EntityFrameworkCore;

namespace HIMIS_API.Data
{
    public class DbContextWeb : DbContext
    {

        public DbContextWeb(DbContextOptions<DbContextWeb> option) : base(option)
        {

        }
        public DbSet<AdminLoginDTO> AdminLoginDbSet { get; set; }
        public DbSet<DrugTenderDTO> DrugTenderDbSet { get; set; }
        public DbSet<EquipmentDTO> EquipmentDbSet { get; set; }
        public DbSet<CivilTenderDTO> CivilTenderDbSet { get; set; }
        public DbSet<OtherTenderDTO> OtherTenderDbSet { get; set; }
        public DbSet<MostVisitedContentDTO> MostVisitedContentDbSet { get; set; }


        public DbSet<GetContentHeaderDTO> GetContentHeaderDbSet { get; set; }
        public DbSet<GetContentAttachmentDTO> GetContentAttachmentDbSet { get; set; }
        public DbSet<GetTenderRefDTO> GetTenderRefDbSet { get; set; }
        public DbSet<GetDrugTenderListAllDTO> GetDrugTenderListAllDbSet { get; set; }

        public DbSet<CivilTenderAllDTO> CivilTenderAllDbSet { get; set; }
        public DbSet<GetNoticCircularDTO> GetNoticCircularDbSet { get; set; }
        public DbSet<GetDeptDTO> GetDeptDbSet { get; set; }
        public DbSet<GetEmployeeDTO> GetEmployeeDbSet { get; set; }
        public DbSet<GetProductBlacklistedDTO> GetProductBlacklistedDbSet { get; set; }
        public DbSet<GetFirmBlacklistedDTO> GetFirmBlacklistedDbSet { get; set; }
        public DbSet<GetEqpProductBlacklistedDTO> GetEqpProductBlacklistedDbSet { get; set; }
        public DbSet<GetEqpBlacklistedFirmsDTO> GetEqpBlacklistedFirmsDbSet { get; set; }
        public DbSet<GetHRarchiveParticularDTO> GetHRarchiveParticularDbSet { get; set; }
        public DbSet<ContentCategoryDTO> ContentCategoryDbSet { get; set; }
        public DbSet<RecruitmentDeptDTO> RecruitmentDeptDbSet { get; set; }
        public DbSet<HRContentDeptCatDTO> HRContentDeptCatDbSet { get; set; }
        public DbSet<QCTenderAttachmentDTO> QCTenderAttachmentDbSet { get; set; }
        public DbSet<DynamicLightBoxDTO> DynamicLightBoxDbSet { get; set; }
    
        


















        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Inform EF that DTOs doesn't have a key
            modelBuilder.Entity<DrugTenderDTO>().HasNoKey();
            modelBuilder.Entity<EquipmentDTO>().HasNoKey();
            modelBuilder.Entity<CivilTenderDTO>().HasNoKey();
            modelBuilder.Entity<OtherTenderDTO>().HasNoKey();
            modelBuilder.Entity<MostVisitedContentDTO>().HasNoKey();
            modelBuilder.Entity<GetContentHeaderDTO>().HasNoKey();
            modelBuilder.Entity<GetContentAttachmentDTO>().HasNoKey();
            modelBuilder.Entity<GetDrugTenderListAllDTO>().HasNoKey();
            modelBuilder.Entity<CivilTenderAllDTO>().HasNoKey();
            modelBuilder.Entity<GetNoticCircularDTO>().HasNoKey();
            modelBuilder.Entity<GetDeptDTO>().HasNoKey();
            modelBuilder.Entity<GetEmployeeDTO>().HasNoKey();
            modelBuilder.Entity<GetProductBlacklistedDTO>().HasNoKey();
            modelBuilder.Entity<GetFirmBlacklistedDTO>().HasNoKey();
            modelBuilder.Entity<GetEqpProductBlacklistedDTO>().HasNoKey();
            modelBuilder.Entity<GetEqpBlacklistedFirmsDTO>().HasNoKey();
            modelBuilder.Entity<GetHRarchiveParticularDTO>().HasNoKey();
            modelBuilder.Entity<ContentCategoryDTO>().HasNoKey();
            modelBuilder.Entity<HRContentDeptCatDTO>().HasNoKey();
            modelBuilder.Entity<QCTenderAttachmentDTO>().HasNoKey();
            modelBuilder.Entity<DynamicLightBoxDTO>().HasNoKey();


        }
    }
}
