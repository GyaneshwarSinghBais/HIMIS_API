namespace HIMIS_API
{
    public class GetProductBlacklistedDTO
    {
        public string? NameofProduct { get; set; }
        public string? NameOfFirm { get; set; }
        public string? Address { get; set; }
        public string? Fromdate { get; set; }  // Formatted as varchar(19) using style 105 (dd-MM-yyyy)
        public string? Upto { get; set; }      // Same formatting as above
        public string? ReasonOfBlacklisting { get; set; }
        public string? Spremarks { get; set; } // Defaults to '-' if NULL in SQL
    }
}
