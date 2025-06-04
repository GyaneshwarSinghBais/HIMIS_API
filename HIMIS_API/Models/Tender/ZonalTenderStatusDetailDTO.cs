namespace HIMIS_API.Models.Tender
{
    public class ZonalTenderStatusDetailDTO
    {
        public int? TenderID { get; set; }
        public string? TenderNo { get; set; }
        public string? eProcNo { get; set; }
        public string? Discription { get; set; }
        public string? startDT { get; set; }
        public string? endDT { get; set; }
        public long? Capacity { get; set; }
        public string? ZonalType { get; set; }
        public string? district { get; set; }
        public string? block { get; set; }
        public int? DistrictID { get; set; }
        public string? NagarNigam { get; set; }
        public int? calls { get; set; }
        public string? tenderstatus { get; set; }
        public string? tenderremark { get; set; }
        public string? entrydate { get; set; }
        public string? CoverA { get; set; }
        public string? CoverC { get; set; }
    }
}
