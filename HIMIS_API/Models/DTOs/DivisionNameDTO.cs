using System.ComponentModel.DataAnnotations;

namespace HIMIS_API.Models.DTOs
{
    public class DivisionNameDTO
    {
        [Key]
        public Int32 DIV_ID { get; set; }
        public string? DIVNAME_EN { get; set; }
        public string? DIVISIONID { get; set; }

    }
}
