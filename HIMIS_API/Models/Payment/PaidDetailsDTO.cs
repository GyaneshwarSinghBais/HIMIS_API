using System.ComponentModel.DataAnnotations;

namespace HIMIS_API.Models.Payment
{
    public class PaidDetailsDTO
    {
        [Key]
        public string? WORK_ID { get; set; }
        public string? HEAD { get; set; }
        public string? DIVISION { get; set; }
        public string? DISTRICT { get; set; }
        public string? WORKNAME { get; set; }
        public string? WrokOrderDT { get; set; }

        public Int32? BILLNO { get; set; }
        public string? AGRBILLSTATUS { get; set; }
        
        public Decimal? TOTALAMOUNTOFCONTRACT { get; set; }
        public Decimal? GROSSPAID { get; set; }

        public string? MesurementDT { get; set; }
        public string? BILLDATE { get; set; }
        public string? ChequeDT { get; set; }
        public Int32? DAYSSINCEMEASUREMENT { get; set; }
        public Decimal? TOTALPAIDTILLINLAC { get; set; }
    }
}
