namespace HIMIS_API.Models.StockMgm
{
    public class CoverStatusDetailDTO
    {
        public int? CSID { get; set; }
        public int? tender_id { get; set; }
        public string? isGemTender { get; set; }
        public string? tender_no { get; set; }
        public string? tender_description { get; set; }
        public string? tender_date { get; set; }
        public string? ENDDate { get; set; }
        public string? cover_a { get; set; }
        public string? cover_b { get; set; }
        public string? CStatus { get; set; }
        public int? cntItems { get; set; }
        public decimal? TenderValue { get; set; }
        public string? TENDERSTATUS { get; set; }
        public string? tenderremark { get; set; }
        public string? entrydate { get; set; }
    }
}
