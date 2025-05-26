using System.ComponentModel.DataAnnotations;

namespace HIMIS_API.Models.WorkOrder
{
    public class WOrderGeneratedDTO
    {

        [Key]

        public string? ID { get; set; }
        public string? Name { get; set; }

        public string? TotalWorks { get; set; }
        public Decimal? TotalTVC { get; set; }
        public Int32? AVGDaysSinceAcceptance { get; set; }
        public Int32? ZonalWorks { get; set; }
        public Int32? TenderWorks { get; set; }
        public Decimal? TotalZonalTVC { get; set; }
        public Decimal? TotalNormalTVC { get; set; }
    }
}
