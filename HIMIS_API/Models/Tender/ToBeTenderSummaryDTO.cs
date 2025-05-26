using System.ComponentModel.DataAnnotations;

namespace HIMIS_API.Models.Tender
{
    public class ToBeTenderSummaryDTO
    {
        [Key]
    
        public Int32? ID { get; set; }
        public string? Name { get; set; }

        public Int32? nosWorks { get; set; }

        public Decimal? TValue { get; set; }
        public string? NosValue { get; set; }
        

    }
}
