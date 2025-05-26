

namespace HIMIS_API.Models.WebCGMSC
{
    public class GetFirmBlacklistedDTO
    {
      
        public string? NameOfFirm { get; set; }
        public string? Address { get; set; }
        public string? Fromdate { get; set; }  // Formatted as string (dd-MM-yyyy)
        public string? Upto { get; set; }      // Formatted as string (dd-MM-yyyy)
        public string? ReasonOfBlacklisting { get; set; }
        public string? Spremarks { get; set; } // Defaults to '-' in SQL if NULL
    }
}
