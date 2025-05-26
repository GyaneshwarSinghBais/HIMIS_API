using System.ComponentModel.DataAnnotations;

namespace HIMIS_API.Models.Tender
{
    public class EvaluationDTO
    {

        [Key]

        public string? ID { get; set; }
        public string? Name { get; set; }

        public Int32? nosWorks { get; set; }
        public Int32? nosTender { get; set; }
        public Decimal? TotalValuecr { get; set; }
        public Int32? AvgDaysSince { get; set; }

    }
}
