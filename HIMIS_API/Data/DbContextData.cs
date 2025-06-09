using HIMIS_API.Models;
using HIMIS_API.Models.AS;
using HIMIS_API.Models.DetailsProgress;
using HIMIS_API.Models.DTOs;
using HIMIS_API.Models.Handover;
using HIMIS_API.Models.LandIssue;
using HIMIS_API.Models.Payment;
using HIMIS_API.Models.RunningWork;
using HIMIS_API.Models.Tender;
using HIMIS_API.Models.TS;
using HIMIS_API.Models.WebCGMSC;
using HIMIS_API.Models.WorkOrder;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace HIMIS_API.Data
{
    public class DbContextData : DbContext
    {
        public DbContextData(DbContextOptions<DbContextData> option) : base(option)
        {

        }

        public DbSet<MasValueParaDTO> MasValueParaDbSet { get; set; }
        public DbSet<AdminUserModel> AdminUserDbSet { get; set; }
        public DbSet<FieldLogin> FieldLoginDbSet { get; set; }
        public DbSet<MainSchemeDTO> MainSchemeDbSet { get; set; }
        public DbSet<DivisionNameDTO> DivisionNameDbSet { get; set; }
        public DbSet<DistrictNameDTO> DistrictNameDbSet { get; set; }
        public DbSet<ProgressLevelDTO> ProgressLevelDbSet { get; set; }
        public DbSet<HealthCentreDTO> HealthCentreDbSet { get; set; }
        public virtual DbSet<LoginAdmin> LoginDbSet { get; set; }

        public DbSet<WorkDetailDTO> WorkDetailDbSet { get; set; }
        public DbSet<WOPendingDivisionDTO> WOPendingDivisionDbSet { get; set; }
        public DbSet<WOPendingDetailDTO> WOPendingDetailDBset { get; set; }
        public DbSet<MasContractorDTO> MasContractorDbSet { get; set; }

        public DbSet<WorkContractorDTO> WorkContractorDbSet { get; set; }
        public DbSet<DivPerformanceDTO> DivPerformanceDbSet { get; set; }
        public DbSet<ProgressCountDTO> ProgressCountDbSet { get; set; }
        public DbSet<DistProgressCountDTO> DistProgressCountDbSet { get; set; }
        public DbSet<DMEProgressCountDTO> DMEProgressCountDbSet { get; set; }
        public DbSet<FillWorkDTO> FillWorkDTODbSet { get; set; }
        public DbSet<WorkInfoDTO> WorkInfoDBset { get; set; }
        public DbSet<DashLoginDDLDTO> DashLoginDDLDbSet { get; set; }
        public DbSet<ProgressCountDivisionDTO> ProgressCountDivisionDbSet { get; set; }
        public DbSet<ProgressCountDetailsDTO> ProgressCountDetailsDbSet { get; set; }

        public DbSet<WOPendingTotalDTO> WOPendingTotalDbSet { get; set; }
        public DbSet<WorkorderpendingdetailsDTO> WorkorderpendingdetailsDbSet { get; set; }
        public DbSet<LandIssueSummaryDTO> LandIssueSummaryDbSet { get; set; }
        public DbSet<HandOverAbstractDTO> HandOverAbstractDbSet { get; set; }
        public DbSet<EngAllotedDTO> EngAllotedDbSet { get; set; }
        public DbSet<DistrictEngAllotedDTO> DistrictEngAllotedDbSet { get; set; }
        public DbSet<ProjectTimeDTO> ProjectTimeDbSet { get; set; }
        public DbSet<TSSummaryDTO> TSSummaryDbSet { get; set; }
        public DbSet<TSDetailsDTO> TSDetailsDbSet { get; set; }
        public DbSet<TenderAbstractDTO> TenderAbstractDbSet { get; set; }
        public DbSet<WOrderGeneratedDTO> WOrderGeneratedDbSet { get; set; }
        public DbSet<WOGeneratedDetailsDTO> WOGeneratedDetailsDbSet { get; set; }
        public DbSet<LiveTenderDetails> LiveTenderDetailsDbSet { get; set; }
        public DbSet<EvaluationDTO> EvaluationDbSet { get; set; }
        public DbSet<EvaluatinoDetailsDTO> EvaluatinoDetailsDbSet { get; set; }
        public DbSet<PriceOpnedDTO> PriceOpnedDbSet { get; set; }
        public DbSet<PaidSummaryDTO> PaidSummaryDbSet { get; set; }
        public DbSet<UnPaidSummary> UnPaidSummaryDbSet { get; set; }
        public DbSet<PaidDetailsDTO> PaidDetailsDbSet { get; set; }
        public DbSet<UnPaidDetailsDTO> UnPaidDetailsDbSet { get; set; }
        public DbSet<WorkBillStatusDTO> WorkBillStatusDbSet { get; set; }

        public DbSet<ToBeTenderSummaryDTO> ToBeTenderSummaryDbSet { get; set; }
        public DbSet<TobeTenderDetailsASDTO> TobeTenderDetailsASDbSet { get; set; }
        public DbSet<TobeTenderZonalAppliedDTO> TobeTenderZonalAppliedDbSet { get; set; }
        public DbSet<TobeTenderRejDetailsDTO> TobeTenderRejDetailsDbSet { get; set; }
        public DbSet<ASPendingDTO> ASPendingDbSet { get; set; }
        public DbSet<ASDivsionPendingDTO> ASDivsionPendingDbSet { get; set; }
        public DbSet<ASEnteredDetailsDTO> ASEnteredDetailsDbSet { get; set; }
        public DbSet<ASEnteredSummyDTO> ASEnteredSummyDbSet { get; set; }
        public DbSet<ASFileNameDTO> ASFileNameDbSet { get; set; }


        public DbSet<WORunningHandDetailsDTO> WORunningHandDetailsDbSet { get; set; }
        public DbSet<TobeTenderDash> TobeTenderDashDbSet { get; set; }
        public DbSet<TenderinProcess> TenderinProcessDbSet { get; set; }
        public DbSet<RunningWorkDTOSummary> RunningWorkDTOSummaryDbSet { get; set; }

        public DbSet<RunningWorkDelayDTO> RunningWorkDelayDbSet { get; set; }
        public DbSet<ProjectTimelinePyramidDTO> ProjectTimelinePyramidDbSet { get; set; }
        public DbSet<RunningWorkDetailsDelnTimeDTO> RunningWorkDetailsDelnTimeDbSet { get; set; }
        public DbSet<GetTenderStatusDTO> GetTenderStatusDbSet { get; set; }
        public DbSet<GetTenderStatusDetailDTO> GetTenderStatusDetailDbSet { get; set; }

        public DbSet<ZonalTenderStatusDTO> ZonalTenderStatusDbSet { get; set; }
        public DbSet<ZonalTenderStatusDetailDTO> ZonalTenderStatusDetailDbSet { get; set; }
        public DbSet<GetToBeTenderDTO> GetToBeTenderDbSet { get; set; }
        public DbSet<ZonalToBeTenderDTO> ZonalToBeTenderDbSet { get; set; }










        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<GetTenderStatusDTO>().HasNoKey();
            modelBuilder.Entity<GetTenderStatusDetailDTO>().HasNoKey();
            modelBuilder.Entity<ZonalTenderStatusDetailDTO>().HasNoKey();
            modelBuilder.Entity<GetToBeTenderDTO>().HasNoKey();
            modelBuilder.Entity<ZonalToBeTenderDTO>().HasNoKey();

        }


    }
}
