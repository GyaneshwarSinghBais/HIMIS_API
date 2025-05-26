using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HIMIS_API.Models
{
    [Table("AGENCYMASTER")]
    public class AdminUserModel
    {
        
        [Key]

        public int AGENCYID { get; set; }
        public string? AGENCYNAME { get; set; }
        public string? PASS { get; set; }
        public string? PASSCOMMON { get; set; }
     
    }
}
