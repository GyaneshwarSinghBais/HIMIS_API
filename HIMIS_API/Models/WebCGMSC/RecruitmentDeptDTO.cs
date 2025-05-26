using System.ComponentModel.DataAnnotations;

namespace HIMIS_API.Models.WebCGMSC
{
    public class RecruitmentDeptDTO
    {
        [Key]
        public string RecruitmentId { get; set; }
        public string RecruitmentScheme { get; set; }
    }
}
