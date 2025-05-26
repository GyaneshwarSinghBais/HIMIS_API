using System.ComponentModel.DataAnnotations;

namespace HIMIS_API.Models.WebCGMSC
{
    public class GetTenderRefDTO
    {
        [Key]
        public string Content_Registration_Id { get; set; }

        public string? Content_Subject { get; set; }
    }
}
