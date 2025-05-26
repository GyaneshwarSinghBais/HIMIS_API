using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HIMIS_API.Models
{
    [Table("AgencyDivisionMaster")]
    public class FieldLogin
    {
        [Key]



        public string? DivisionID { get; set; }
    
        public string? Remarks { get; set; }

        public string? PASS { get; set; }
   
        public string? PASSCOMMON { get; set; }

    }
}
