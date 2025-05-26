using System;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace HIMIS_API.Models.DTOs
{
    public class WorkInfoDTO
    {
        [Key]

        public string? work_id { get; set; }
        public string? LetterNo { get; set; }
        
        public string? Head { get; set; }
        public string? subengname { get; set; }
        public string? desig { get; set; }
        public string? grantname { get; set; }
        public string? Approver { get; set; }
        public string? type_name { get; set; }
        public string? DivName_En { get; set; }
        
        public string? DBStart_Name_En { get; set; }
        public string? blockname { get; set; }

        public string? AADate { get; set; }
        public string? work { get; set; }
        public string? AAAMT { get; set; }
        public string? TSAMT { get; set; }
        public string? TSDate { get; set; }
        public string? AgreementNo { get; set; }
        public string? YearofAgreement { get; set; }
        public string? WrokOrderDT { get; set; }
        public string? AgreementRefNo { get; set; }
        public string? WorkorderRefNoGovt { get; set; }
        
        public string? ActualDueDT { get; set; }
        public string? DueDTTimePerAdded { get; set; }
        public string? AcceptanceLetterRefNo { get; set; }
        public string? AcceptLetterDT { get; set; }
        public string? SanctionDetail { get; set; }

        public Decimal? PAC { get; set; }
        public Decimal? TotalAmountOfContract { get; set; }
        public Decimal? SanctionRate { get; set; }
        public Int64? TimeAllowed { get; set; }
        public string? PGReq { get; set; }
        public string? DateOfSanction { get; set; }
        public string? DateOfIssueNIT { get; set; }
        public string? CID { get; set; }
        public string? ContractorNAme { get; set; }
        public string? RegType { get; set; }
        public string? Class { get; set; }
        public string? MobNo { get; set; }
        public string? ASPath { get; set; }
        public string? DashName { get; set; }
        public string? GroupName { get; set; }
        public string? LProgress { get; set; }
        public string? Pdate { get; set; }
        public string? PRemarks { get; set; }
        public string? Remarks { get; set; }
        public string? Divisionid { get; set; }
        public Decimal? Expd { get; set; }
        public Decimal? ExpPending { get; set; }
        public string? FBill { get; set; }
        public string? TType { get; set; }
        public string? TenderReference { get; set; }
        public Int32? SR { get; set; }
        public string? ImageName { get; set; }
        public string? ImageName2 { get; set; }
        public string? ImageName3 { get; set; }
        public string? ImageName4 { get; set; }
        public string? ImageName5 { get; set; }
        public string? ismongo { get; set; }
        public string? ProgressEnterby { get; set; }
        public DateTime? ProgressEntryTime { get; set; }
        public Byte[]? NonMongoImage { get; set; }
        public string? expCompDT { get; set; }
        public string? delayreason { get; set; }

        public string? GrantNo { get; set; }



    }
}
