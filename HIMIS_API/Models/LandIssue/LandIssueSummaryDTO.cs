using System.ComponentModel.DataAnnotations;

namespace HIMIS_API.Models.LandIssue
{
    public class LandIssueSummaryDTO
    {

        [Key]

        public string? ID { get; set; }
        public string? Name { get; set; }

        public Int32? TotalWorks { get; set; }
        public Decimal? valuecr { get; set; }
        public Int32? WOIssued { get; set; }
        public Decimal? TVCValuecr { get; set; }
        public Int32? Month2Above { get; set; }
    }
}
