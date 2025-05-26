namespace HIMIS_API.Models.EMS
{
    public class GetTenderDetailDTO
    {

        public int? TENDER_ID { get; set; }
        public string? TENDER_NO { get; set; }
        public string? eprocID { get; set; }

        public string? TENDER_DATE { get; set; }
        public string? extenddt { get; set; }
        public string? ENDDate { get; set; }

        public string? tender_description { get; set; }
        public string? TENDERSTATUS { get; set; }
        public string? tenderremark { get; set; }

        public int? NoOfItems { get; set; }
        public decimal? TenderValue { get; set; }

        public bool? cover_a { get; set; }
        public bool? cover_b { get; set; }
        public bool? cover_Demo { get; set; }
        public bool? cover_c { get; set; }

        //public int? TENDER_ID { get; set; }
        //public string? TENDER_NO { get; set; }
        //public string? TENDER_DATE { get; set; }
        //public string? tender_description { get; set; }
        //public string? TenderStatus { get; set; }
        //public string? TenderRemark { get; set; }
        //public int? NoOfItems { get; set; }
        //public decimal? TenderValue { get; set; }
    }
}
