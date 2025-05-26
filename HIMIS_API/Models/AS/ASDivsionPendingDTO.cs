using System.ComponentModel.DataAnnotations;

namespace HIMIS_API.Models.AS
{
    public class ASDivsionPendingDTO
    {


        [Key]


        public string? ID { get; set; }
        public string? DivisionID { get; set; }
        public string? Division { get; set; }

        public string? login_name { get; set; }
        public string? Head { get; set; }
        public string? Letterno { get; set; }
        public string? ASDate { get; set; }

        public Int32? asid { get; set; }

        public Int32? TotalWorks { get; set; }
        public Int32? enteredWorks { get; set; }

        public Int32? BalanceWork { get; set; }

    }
}
