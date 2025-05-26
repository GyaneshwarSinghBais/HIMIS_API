using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;

namespace HIMIS_API.Models.DetailsProgress
{
    public class WORunningHandDetailsDTO
    {

        [Key]

        public string? work_id { get; set; }
        public string? LetterNo { get; set; }
        
        public string? Head { get; set; }

        public string? Approver { get; set; }
        public string? type_name { get; set; }
        public string? DivName_En { get; set; }
        public string? District { get; set; }
        public string? blockname { get; set; }
        public string? work { get; set; }

        public Decimal? ASAmt { get; set; }
        public Decimal? TSAMT { get; set; }
        public string? AADT { get; set; }
        public string? TSDate { get; set; }
        public string? TType { get; set; }
        public string? TenderReference { get; set; }
        public string? AcceptanceLetterRefNo { get; set; }
        public string? AcceptLetterDT { get; set; }

        public Decimal? TotalAmountOfContract { get; set; }
        public Decimal? Totalpaid { get; set; }
        public Decimal? Totalunpaid { get; set; }

        public Decimal? SanctionRate { get; set; }
        public string? SanctionDetail { get; set; }
        public string? WrokOrderDT { get; set; }
        public string? HOAllotedDT { get; set; }

        public string? AgreementRefNo { get; set; }
        public string? WorkorderRefNoGovt { get; set; }
        public string? DueDTTimePerAdded { get; set; }
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
        public string? ProgressDT { get; set; }
        public string? PRemarks { get; set; }
        public string? Remarks { get; set; }
        public string? display { get; set; }
        public string? descri { get; set; }
        public string? fmrcode { get; set; }
        
        public string? expcompdt { get; set; }
        public string? delayreason { get; set; }

        public string? subengname { get; set; }
        public string? AEName { get; set; }
        public string? GrantNo { get; set; }
        
    }
}
