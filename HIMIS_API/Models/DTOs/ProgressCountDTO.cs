using System.ComponentModel.DataAnnotations;

namespace HIMIS_API.Models.DTOs
{
    public class ProgressCountDTO
    {

        [Key]


        public Int32 DID { get; set; }
        public string? DASHNAME { get; set; }
  
        public Int32? NOSWORKS { get; set; }
       
    }
}
