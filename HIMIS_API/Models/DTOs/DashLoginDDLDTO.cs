using System.ComponentModel.DataAnnotations;

namespace HIMIS_API.Models.DTOs
{
    public class DashLoginDDLDTO
    {

        [Key]

        public string? ID { get; set; }
        public string? Desig { get; set; }
        public string? Mobile { get; set; }
        public Int32? Rankid { get; set; }
        public string? Role { get; set; }
        

    }
}
