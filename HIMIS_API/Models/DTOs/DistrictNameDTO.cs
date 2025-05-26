using System.ComponentModel.DataAnnotations;

namespace HIMIS_API.Models.DTOs
{
    public class DistrictNameDTO
    {
        [Key]
        public string? DISTRICT_ID { get; set; }
        public string? DISTRICTNAME { get; set; }
        public Int32? DIV_ID { get; set; }

    }
}
