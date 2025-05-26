using System.ComponentModel.DataAnnotations;

namespace HIMIS_API.Models.RunningWork
{
    public class RunningWorkDelayDTO
    {
        [Key]


        public string? ID { get; set; }
        public string? Name { get; set; }

        public Int32? TotalWorks { get; set; }
        public Decimal? TVCValuecr { get; set; }
        public Decimal? PaidTillcr { get; set; }
        public Decimal? GrossPendingcr { get; set; }
        public Int32? MorethanSixMonth { get; set; }
        public Int32? D_91_180Days { get; set; }
        public Int32? D_1_90Days { get; set; }
        public Int32? TimeValid { get; set; }


    }
}
