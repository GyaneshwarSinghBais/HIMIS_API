using System;
using System.ComponentModel.DataAnnotations;

namespace HIMIS_API.Models.DTOs
{
    public class WOPendingDetailDTO
    {


        [Key]

        public string? WORK_ID { get; set; }
        public string? HEAD { get; set; }
        public string? ASDATE { get; set; }
        public string? ASAMT { get; set; }
        public string? TSDATE { get; set; }
        
        public string? DISTRICT { get; set; }
        public string? BLOCK { get; set; }
        
        public string? WORKNAME { get; set; }


        public string? ACCEPTLETTERDT { get; set; }
        public string? TENDERREFERENCE { get; set; }
        public string? TENDERTYPE { get; set; }
        public string? CNAME { get; set; }
        public string? CONTRACTORID { get; set; }

        public decimal? SANCTIONRATE { get; set; }
        public decimal? PAC { get; set; }
        public string? SANCTIONDETAIL { get; set; }
        public string? TOTALAMOUNTOFCONTRACT { get; set; }
        public string? TIMEALLOWED { get; set; }
       
        public string? ACCEPTANCELETTERREFNO { get; set; }
        public string? DATEOFSANCTION { get; set; }
        public string? PROGRESSDT { get; set; }
        
        public string? LPROGRESS { get; set; }
        public string? PREMARKS { get; set; }

        public string? DELAYREASON { get; set; }
       


    }
}
