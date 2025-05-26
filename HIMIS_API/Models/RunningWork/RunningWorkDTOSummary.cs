using System.ComponentModel.DataAnnotations;

namespace HIMIS_API.Models.RunningWork
{
    public class RunningWorkDTOSummary
    {
        [Key]


        public string? ID { get; set; }
        public string? Name { get; set; }

        public Int32? TotalWorks { get; set; }
        public Decimal? TVCValuecr { get; set; }
        public Decimal? PaidTillcr { get; set; }
        public Decimal? GrossPendingcr { get; set; }

    }
}
