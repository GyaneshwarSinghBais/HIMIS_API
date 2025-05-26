using System.ComponentModel.DataAnnotations;
namespace HIMIS_API.Models.AS
{
    public class ASFileNameDTO
    {
    

        [Key]
        public string? ID { get; set; }
        public string? ASPath { get; set; }
        public string? ASLetterName { get; set; }
        public string? Filename { get; set; }
    }
}
