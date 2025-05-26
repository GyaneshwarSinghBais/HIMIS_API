using System.ComponentModel.DataAnnotations;
namespace HIMIS_API.Models.WorkOrder
{
    public class WorkorderpendingdetailsDTO
    {

  [Key]

        public string? work_id { get; set; }
        public string? LetterNo { get; set; }

        public string? Head { get; set; }

        public string? Approver { get; set; }
        public string? type_name { get; set; }
        public string? District { get; set; }
        public string? blockname { get; set; }
        public string? work { get; set; }

        public Decimal? AAAMT { get; set; }
        public Decimal? TSAMT { get; set; }
        public string? AADate { get; set; }
        public string? TSDate { get; set; }
        public string? AcceptanceLetterRefNo { get; set; }
        public string? AcceptLetterDT { get; set; }
        public Decimal? PAC { get; set; }
        public Decimal? TotalAmountOfContract { get; set; }
        public Decimal? SanctionRate { get; set; }
        public string? SanctionDetail { get; set; }
        public Int64? TimeAllowed { get; set; }

        public string? DateOfSanction { get; set; }
        public string? DateOfIssueNIT { get; set; }
        
        public string? CID { get; set; }
        public string? ContractorNAme { get; set; }

        public string? RegType { get; set; }
        public string? Class { get; set; }
  
        public string? EnglishAddress { get; set; }
        public string? MobNo { get; set; }
        public string? ASPath { get; set; }
        public string? ASLetter { get; set; }

        public string? GroupName { get; set; }
        public string? LProgress { get; set; }
        public string? Pdate { get; set; }
        public string? PRemarks { get; set; }
        public string? Remarks { get; set; }
       

        public string? TenderReference { get; set; }
        public string? DelayReason { get; set; }
        


    }
}
