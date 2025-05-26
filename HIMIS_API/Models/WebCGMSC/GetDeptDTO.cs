

using System.ComponentModel.DataAnnotations;

namespace HIMIS_API.Models.WebCGMSC
{
    public class GetDeptDTO
    {
        [Key]
        public string CoreDept { get; set; }
    }
}
