using System.ComponentModel.DataAnnotations;

namespace HIMIS_API.Models.Tender
{
    public class TobeTenderZonalAppliedDTO
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
        public Decimal? value { get; set; }
        public string? ASDate { get; set; }
        public string? LProgress { get; set; }
        public string? ProgressDT { get; set; }

        public string? ZonalType { get; set; }
        public string? NITNo { get; set; }
        public Int32? ppid { get; set; }
        public Int32? TenderID { get; set; }
        public Int32? apild { get; set; }
        
    }
}
