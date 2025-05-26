using System.ComponentModel.DataAnnotations;

namespace HIMIS_API.Models.WorkOrder
{
    public class DistrictEngAllotedDTO
    {
  

        [Key]
        public string? ID { get; set; }
        public string? DistrictID { get; set; }
        public string? EMPID { get; set; }
        public string? EngName { get; set; }
 
        public string? DistrictName { get; set; }
  
        

        public Int32? TotalWorks { get; set; }

        public Decimal? TVCValuecr { get; set; }
        public Int32? running { get; set; }
        public Int32? WOIssue { get; set; }
        public Int32? ladissue { get; set; }
    }
}
