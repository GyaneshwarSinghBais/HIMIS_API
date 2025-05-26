using System.ComponentModel.DataAnnotations;
namespace HIMIS_API.Models.WorkOrder
{
    public class WOPendingTotalDTO
    {


                [Key]

        public string? ID { get; set; }
        public string? Name { get; set; }

        public string? PendingWork { get; set; }
        public Decimal? ContrctValuecr { get; set; }
        public string? NoofWorksGreater7Days { get; set; }
    }
}
