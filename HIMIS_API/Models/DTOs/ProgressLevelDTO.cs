using System.ComponentModel.DataAnnotations;

namespace HIMIS_API.Models.DTOs
{
    public class ProgressLevelDTO
    {
        [Key]
        public Int32 PPID { get; set; }
        public string? PPSTATUS { get; set; }
   

    }
}
