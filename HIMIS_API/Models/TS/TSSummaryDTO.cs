using System.ComponentModel.DataAnnotations;

namespace HIMIS_API.Models.LandIssue
{
    public class TSSummaryDTO
    {
        [Key]

        public string? ID { get; set; }
        public string? Name { get; set; }

        public Int32? nosWorks { get; set; }
        public Decimal? ASValuecr { get; set; }
        public Int32? Above2crWork { get; set; }

        public Int32? below2crWork { get; set; }
    }
}
