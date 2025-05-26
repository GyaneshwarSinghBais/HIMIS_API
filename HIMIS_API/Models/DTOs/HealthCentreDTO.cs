using System.ComponentModel.DataAnnotations;

namespace HIMIS_API.Models.DTOs
{
    public class HealthCentreDTO
    {
        [Key]
        public Int32 TYPE_ID { get; set; }
        public string? DETAILS_ENG { get; set; }
        public string? NOSWORKS { get; set; }

    }
}
