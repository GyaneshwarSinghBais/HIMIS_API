namespace HIMIS_API.Models.Tender
{
    public class GetTenderStatusDTO
    {
        public int? PGroupID { get; set; }
        public string? TenderStatus { get; set; }

        public int? nosTender { get; set; }
        public int? nosWorks { get; set; }

        public decimal? TotalValuecr { get; set; }
        public int? PPID { get; set; }
        
    }
}
