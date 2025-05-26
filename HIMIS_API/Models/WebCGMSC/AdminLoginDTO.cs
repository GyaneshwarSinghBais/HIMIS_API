using System.ComponentModel.DataAnnotations;

namespace HIMIS_API.Models.WebCGMSC
{
    public class AdminLoginDTO
    {
        [Key]
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
