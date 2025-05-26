using System.ComponentModel.DataAnnotations;

namespace HIMIS_API.Models.WebCGMSC
{
    public class GetEmployeeDTO
    {
        public string? FullName { get; set; }
        public string? Designation { get; set; }
        public string? Department { get; set; }
        public string? EmailId { get; set; }
        [Key]
        public string? ContactNo { get; set; }
    }
}
