using System.ComponentModel.DataAnnotations;

namespace HIMIS_API.Models.Tender
{
    public class LiveTenderDetails
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
        public Decimal? ASAmt { get; set; }
        public string? AA_RAA_Date { get; set; }
        public Int32? DayssinceAS { get; set; }

        public string? startdt { get; set; }

        public string? enddt { get; set; }

        public Int32? Noofcalls { get; set; }
        public Int32? DaystoEnd { get; set; }
        public string? tenderno { get; set; }
        public string? eprocno { get; set; }
    }
}
