using System.ComponentModel.DataAnnotations;

namespace HIMIS_API.Models.AS
{
    public class ASEnteredDetailsDTO
    {

        [Key]
        public string? work_id { get; set; }
        public string? letterno { get; set; }
        public string? District { get; set; }
        public string? Block_Name_En { get; set; }
        public string? Head { get; set; }
        public string? Division { get; set; }
        public string? login_name { get; set; }
        public string? ASDate { get; set; }
        public string? workname { get; set; }

        public Decimal? ASAmt { get; set; }
  
        public Int32? ASID { get; set; }

    }
}
