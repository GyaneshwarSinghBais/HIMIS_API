using System.ComponentModel.DataAnnotations;

namespace HIMIS_API.Models.DTOs
{
    public class MasContractorDTO
    {
        [Key]
        public string? CONTRACTORID { get; set; }
        public string? CONTRACTORNAME { get; set; }
      
    }
}
