using System.ComponentModel.DataAnnotations;

namespace HIMIS_API.Models.DTOs
{
    public class ProjectTimeDTO
    {
        [Key]
       
        public Int32? PPId { get; set; }
        public string? Level { get; set; }
        public string? Pdate { get; set; }
        public Int32? SinceAS { get; set; }
        public Int32? SinceLastProg { get; set; }
     
   
    }
}
