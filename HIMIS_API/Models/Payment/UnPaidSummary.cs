using System.ComponentModel.DataAnnotations;

namespace HIMIS_API.Models.Payment
{
    public class UnPaidSummary
    {
   
        [Key]
        public string? ID { get; set; }
        public string? Name { get; set; }

        public Int32? NoofWorks { get; set; }

        public Decimal? Unpaidcr { get; set; }
        public Int32? AvgDaySinceM { get; set; }
    }
}
