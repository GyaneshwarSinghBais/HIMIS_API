using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;

namespace HIMIS_API.Models.DetailsProgress
{
    public class TobeTenderDash
    {
 
        [Key]
        public string? WORK_ID { get; set; }
        public string? DIVNAME_EN { get; set; }
        public string? HEAD { get; set; }
   
        public string? LETTERNO { get; set; }
        public string? APPROVER { get; set; }
        public string? AADT { get; set; }
        public decimal? ASAMT { get; set; }
        public string? DISTRICT { get; set; }
        public string? BLOCKNAME { get; set; } // Default empty string if null
        public string? WORK { get; set; }
        public decimal? TSAMT { get; set; }
        public string? TSDATE { get; set; } // Changed to DateTime?
        public string? LPROGRESS { get; set; }
        public string? PROGRESSDT { get; set; } // Changed to DateTime?
        public string? REMARKS { get; set; }
        public string? PPID { get; set; }
        public string? GROUPNAME { get; set; }
        public string? PGROUPID { get; set; }
        public string? DID { get; set; }
        public string? DASHNAME { get; set; }
        public string? ASID { get; set; }
        public string? ASPATH { get; set; }
        public string? ASLETTER { get; set; }
        public string? DESCRI { get; set; }
        public string? TYPE_NAME { get; set; }
        public string? FMRCODE { get; set; }
        public string? FMRID { get; set; }
        public string? DELAYREASON { get; set; }
        public string? EXPCOMPDT { get; set; } // Changed to DateTime?
        public string? SUBENGNAME { get; set; }
        public string? AENAME { get; set; }
        public string? LASTNIT { get; set; }
        public string? LASTEPROCNO { get; set; }
        public string? REJREASON { get; set; }
        public string? REJECTEDDT { get; set; } // Changed to DateTime?
        public string? GrantNo { get; set; }
        
        //public string? work_id { get; set; }
        //public string? LetterNo { get; set; }

        //public string? Head { get; set; }

        //public string? Approver { get; set; }
        //public string? type_name { get; set; }
        //public string? DivName_En { get; set; }
        //public string? District { get; set; }
        //public string? blockname { get; set; }
        //public string? work { get; set; }

        //public Decimal? ASAmt { get; set; }
        //public Decimal? TSAMT { get; set; }
        //public string? TSDate { get; set; }
        //public string? AADT { get; set; }
        //public string? LProgress { get; set; }
        //public string? ProgressDT { get; set; }
        //public string? Remarks { get; set; }
        //public string? GroupName { get; set; }
        //public string? DashName { get; set; }


        //public string? ASPath { get; set; }
        //public string? ASLetter { get; set; }
        //public Int32? asid { get; set; }



        //public string? descri { get; set; }
        //public string? fmrcode { get; set; }

        //public string? expcompdt { get; set; }
        //public string? delayreason { get; set; }

        //public string? subengname { get; set; }
        //public string? AEName { get; set; }

        //public string? LastNIT { get; set; }
        //public string? LastEprocno { get; set; }
        //public string? RejReason { get; set; }
        //public string? RejectedDT { get; set; }

    }
}
