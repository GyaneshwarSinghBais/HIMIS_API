
using System.ComponentModel.DataAnnotations;


namespace HIMIS_API.Models.RunningWork
{
    public class RunningWorkDetailsDelnTimeDTO
    {
        [Key]

        public string? work_id { get; set; }
        public string? LetterNo { get; set; }
        public string? Head { get; set; }
        public string? Approver { get; set; }
        public string? type_name { get; set; }
        public string? DivName_En { get; set; }
        public string? DivisionID { get; set; }
        public string? District { get; set; }
        public string? blockname { get; set; }
        public string? work { get; set; }
        public Decimal? AAAMT { get; set; }
        public Decimal? TSAMT { get; set; }
        public string? AADate { get; set; }
        public string? TSDate { get; set; }
        public string? AcceptanceLetterRefNo { get; set; }
        public string? AcceptLetterDT { get; set; }
        public Double? TVC { get; set; }
        public Decimal? PaidTillLacs { get; set; }
        public Decimal? GrossPendinglacs { get; set; }
        public string? WorkorderDT { get; set; }
        public string? DueDTTimePerAdded { get; set; }
        public Int32? DelayDays { get; set; }
        public Int32? TimeAllowed { get; set; }
        public string? DateOfSanction { get; set; }
        public string? DateOfIssueNIT { get; set; }

        public string? TenderReference { get; set; }
        public string? TType { get; set; }
        public string? CID { get; set; }
        public string? ContractorNAme { get; set; }
        public string? RegType { get; set; }
        public string? Class { get; set; }
        public string? EnglishAddress { get; set; }
        public string? MobNo { get; set; }
        public string? LProgress { get; set; }
        public string? ProgressDT { get; set; }
        public string? Remarks { get; set; }
        public string? expcompdt { get; set; }
        public string? delayreason { get; set; }
        public string? subengname { get; set; }
        public string? AEName { get; set; }
    }
}
