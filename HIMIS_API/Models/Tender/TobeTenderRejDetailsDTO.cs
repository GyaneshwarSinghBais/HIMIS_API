using System.ComponentModel.DataAnnotations;

namespace HIMIS_API.Models.Tender
{
    public class TobeTenderRejDetailsDTO
    {

        [Key]
        public string? work_id { get; set; }
        public string? letterno { get; set; }
        public string? District { get; set; }
        public string? Head { get; set; }
        public string? Division { get; set; }
        public string? DETAILS_ENG { get; set; }
        public string? workname { get; set; }
        public string? Block_Name_En { get; set; }
        public Decimal? ValueWorks { get; set; }
        public string? ASDate { get; set; }
        public string? parentprogress { get; set; }
        public string? PDate { get; set; }

        public string? RejReason { get; set; }
        public string? RejectedDT { get; set; }
        public string? DashName { get; set; }
        public string? GroupName { get; set; }
        
        public string? LastNIT { get; set; }
        public Int32? ppid { get; set; }

        public Int32? LastEprocno { get; set; }
    }
}
