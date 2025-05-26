using System.ComponentModel.DataAnnotations;

namespace HIMIS_API.Models.DTOs
{
    public class WOPendingDivisionDTO
    {
        [Key]

        public string? DIVISIONID { get; set; }
        public string? DETAILS { get; set; }
    


    }
}
