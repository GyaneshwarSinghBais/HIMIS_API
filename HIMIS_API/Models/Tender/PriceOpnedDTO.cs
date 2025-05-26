using System.ComponentModel.DataAnnotations;

namespace HIMIS_API.Models.Tender
{
    public class PriceOpnedDTO
    {
        [Key]
        public string? work_id { get; set; }
        public string? letterno { get; set; }
        public string? District { get; set; }
        public string? Head { get; set; }
        public string? Division { get; set; }
      
        public string? workname { get; set; }
 
        public Decimal? ASAmt { get; set; }
        public Decimal? TSAmt { get; set; }
        public Decimal? Valueworksinlas { get; set; }
        
        public string? AA_RAA_Date { get; set; }


        public string? startdt { get; set; }

        public string? enddt { get; set; }

        public Int32? Noofcalls { get; set; }
        public Int32? daysSinceOpen { get; set; }
        public string? TOpnedDT { get; set; }

        public string? tenderno { get; set; }
        public string? eprocno { get; set; }
        public string? topnedpricedt { get; set; }

        public string? CID { get; set; }
        public string? SanctionDetail { get; set; }
        public Decimal? SanctionRate { get; set; }

        public string? CNAme { get; set; }
        public string? MobNo { get; set; }



    }
}
