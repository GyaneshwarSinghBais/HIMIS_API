using System.ComponentModel.DataAnnotations;

namespace HIMIS_API.Models.Handover
{
    public class HandOverAbstractDTO
    {

        [Key]
        public string? ID { get; set; }
        public string? Name { get; set; }

        public Int32? TotalWorks { get; set; }
 
        public Decimal? TVCValuecr { get; set; }
        public Int32? AvgMonthTaken { get; set; }
    }
}
