using System.ComponentModel.DataAnnotations;

namespace HIMIS_API.Models.WorkOrder
{
    public class EngAllotedDTO
    {


        [Key]
        public string? EMPID { get; set; }
        public string? EngName { get; set; }
        public string? ID { get; set; }
        public string? Name { get; set; }
  
        

        public Int32? TotalWorks { get; set; }

        public Decimal? TVCValuecr { get; set; }
        public Int32? running { get; set; }
        public Int32? WOIssue { get; set; }
        public Int32? ladissue { get; set; }
    }
}
