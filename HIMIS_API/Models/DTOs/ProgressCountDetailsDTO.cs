using System;
using System.ComponentModel.DataAnnotations;

namespace HIMIS_API.Models.DTOs
{
    public class ProgressCountDetailsDTO
    {
        [Key]
        public string? work_id { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public DateTime? EntryDT { get; set; }
        
        public string? DivisionID { get; set; }
        public string? DivName_En { get; set; }


        public Int32? TotalToday { get; set; }
        public Int32? Mobiletoday { get; set; }

        public Int32? TotalInLast7Days { get; set; }
        public Int32? MobileInLast7Days { get; set; }

        public Int32? TotalInLast15Days { get; set; }
        public Int32? MobileLast15Days { get; set; }

        public Int32? TotalBefore15Days { get; set; }
        public Int32? MobileBefore15Days { get; set; }

        public string? District_ID { get; set; }
        public string? districtname { get; set; }
        public string? Block_Name_En { get; set; }
        public string? workname { get; set; }
        public string? Head { get; set; }
        public string? PLevel { get; set; }
        public string? ProgressDT { get; set; }
        public string? Premarks { get; set; }
        public string? AADT { get; set; }

        public Decimal? ASAmt { get; set; }
        public string? WrokOrderDT { get; set; }
        public string? DueDTTimePerAdded { get; set; }
        public string? contrctorname { get; set; }
        public Decimal? TotalAmountOfContract { get; set; }

        public Decimal? GrossExpPaid { get; set; }
        public string? subeng { get; set; }
        public string? AE { get; set; }
        public string? Approver { get; set; }
        public Int32? SR { get; set; }
        public string? ImageName { get; set; }




    }
}
