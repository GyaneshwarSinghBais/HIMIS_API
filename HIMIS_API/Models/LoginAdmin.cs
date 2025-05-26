using System.ComponentModel.DataAnnotations;

namespace HIMIS_API.Models
{
    public class LoginAdmin
    {
        [Key]
   
        public int AGENCYID { get; set; }
    
        public string? AGENCYNAME { get; set; }

        public string? PASS { get; set; }
   
        public string? PASSCOMMON { get; set; }

        public string? EMAILID { get; set; }

        public string? FIRSTNAME { get; set; }
    }
}
