namespace HIMIS_API.Models.Tender
{
    public class GetTenderStatusDetailDTO
    {
        public string? TenderStatus { get; set; }
        public string? work_id { get; set; }
        public string? WorkName { get; set; }
        public string? TenderNo { get; set; }
        public Int32? EprocNo { get; set; }
        public decimal? ASAmt { get; set; }
        public decimal? TSAmount { get; set; }
        public string? StartDt { get; set; }        // format: dd-MM-yyyy
        public string? EndDate { get; set; }        // format: dd-MM-yyyy
        public int? NoOfCalls { get; set; }
        public string? CoverADT { get; set; }       // format: dd/MM/yyyy
        public string? CoverBDT { get; set; }       // format: dd/MM/yyyy
        public string? CoverCDT { get; set; }       // format: dd/MM/yyyy
        public string? Tstatus { get; set; }
        public int? PGroupID { get; set; }
        public int? TenderID { get; set; }
        public int? RejId { get; set; }

        public string? tenderremark { get; set; }
        public string? entrydate { get; set; }
        public string? Color { get; set; }
        public string? Head { get; set; }
        public Int32? MainSchemeID { get; set; }

    }
}
