using System.ComponentModel.DataAnnotations;

namespace HIMIS_API.Models.AS
{
    public class ASPendingDTO
    {
        [Key]

    
        public Int32? asid { get; set; }
        public string? login_name { get; set; }
        public string? Head { get; set; }
        public string? Letterno { get; set; }
        public string? ASDate { get; set; }
        
        public string? TotalWorks { get; set; }
        public Int32? enteredWorks { get; set; }
  
        public Int32? BaltobeEnter { get; set; }
        public Decimal? TotalASAmt { get; set; }
        public Decimal? EnteredTotalAS { get; set; }
        public Decimal? BalanceASAmount { get; set; }
        


      
    }
}
