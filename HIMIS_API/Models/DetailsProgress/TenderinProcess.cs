using System.ComponentModel.DataAnnotations;

namespace HIMIS_API.Models.DetailsProgress
{
    public class TenderinProcess
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
        public string? TSDate { get; set; }
        public string? AADT { get; set; }
        public string? LProgress { get; set; }
        public string? ProgressDT { get; set; }
        public string? Remarks { get; set; }
        public string? GroupName { get; set; }
        public string? DashName { get; set; }


        public string? ASPath { get; set; }
        public string? ASLetter { get; set; }
        public Int32? asid { get; set; }



        public string? descri { get; set; }
        public string? fmrcode { get; set; }



        public string? startdt { get; set; }
        public string? enddt { get; set; }
        public Int32? Noofcalls { get; set; }
        public string? tenderno { get; set; }
        public string? eprocno { get; set; }
        public string? CovOpenedDT { get; set; }
        public string? topnedpricedt { get; set; }
  
    }
}
