using System.ComponentModel.DataAnnotations;

namespace HIMIS_API.Models.DTOs
{
    public class MainSchemeDTO
    {
        [Key]
        public Int32 MainSchemeID { get; set; }
        public string? Name { get; set; }
      
    }
}
