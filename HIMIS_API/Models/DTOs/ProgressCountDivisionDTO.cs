using System.ComponentModel.DataAnnotations;
namespace HIMIS_API.Models.DTOs
{
    public class ProgressCountDivisionDTO
    {

        [Key]

        public string? DivisionID { get; set; }
        public string? DivisionName { get; set; }
        
        public Int32? nosworks { get; set; }
        public Int32? TotalToday { get; set; }
        public Int32? Mobiletoday { get; set; }

        public Int32? TotalInLast7Days { get; set; }
        public Int32? MobileInLast7Days { get; set; }

        public Int32? TotalInLast15Days { get; set; }
        public Int32? MobileLast15Days { get; set; }

        public Int32? TotalBefore15Days { get; set; }
        public Int32? MobileBefore15Days { get; set; }
    }
}
