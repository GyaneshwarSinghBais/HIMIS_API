
using System.ComponentModel.DataAnnotations;

namespace HIMIS_API.Models.DTOs
{
    public class ProjectTimelinePyramidDTO
    {

        [Key]
        public Int32? PPId { get; set; }
        public string? Level { get; set; }
        public string? DateProgress { get; set; }
  
    }
}
