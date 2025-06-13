using System.ComponentModel.DataAnnotations;

namespace HIMIS_API.Models.EMS
{
    public class EqToBeTenderDTO
    {
        [Key]
        public Int32? CntItems { get; set; }
        public double? IndentValue { get; set; }
    }
}
