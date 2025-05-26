using System.ComponentModel.DataAnnotations;

namespace HIMIS_API.Models.DTOs
{
    public class MasValueParaDTO
    {
        [Key]
        public Int32 ID  { get; set; }
        public string? VALUEPARA { get; set; }
      
    }
}
