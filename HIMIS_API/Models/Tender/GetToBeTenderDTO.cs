namespace HIMIS_API.Models.Tender
{
    public class GetToBeTenderDTO
    {
        public string? Head { get; set; }
        public string? Division { get; set; }
        public string? District { get; set; }
        public string? Work_id { get; set; }
        public string? workname { get; set; }
        public string? ASLetterNO { get; set; }
        public DateTime? ASDate { get; set; }
        public decimal? ASAmt { get; set; }
        public decimal? TSAmount { get; set; }
        public decimal? ValueWorks { get; set; }
        public string? WorkStatus { get; set; }
    }
}
