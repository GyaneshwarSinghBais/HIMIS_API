using System.ComponentModel.DataAnnotations;

namespace HIMIS_API.Models.DTOs
{
    public class WorkDetailDTO
    {
        [Key]
        public Int64 SN { get; set; }
        public string? DISTRICT { get; set; }
        public string? WORKNAME { get; set; }
        public string? DIVNAME_EN { get; set; }
        public string? ASDATE { get; set; }
        public decimal? ASAMT { get; set; }
        public decimal? TSAMT { get; set; }
        public string? TSDATE { get; set; }
        public string? HEAD { get; set; }
        public string? WORKTYPE { get; set; }
        public string? AAYEAR { get; set; }
        public string? ACCEPTLETTERDT { get; set; }
        public string? NITNO { get; set; }
        public string? ISZONAL { get; set; }
        public string? CNAME { get; set; }
        public string? CMOBNO { get; set; }
        public string? WROKORDERDT { get; set; }
        public decimal? SANCTIONRATE { get; set; }
        public string? SANCTIONDETAIL { get; set; }
        public decimal? TOTALAMOUNTOFCONTRACT { get; set; }
        public decimal? TIMEALLOWED { get; set; }
        public string? DUEDATEOFCOMPLETION { get; set; }
        public string? PSTATUS { get; set; }
        public string? STATUS { get; set; }
        public string? LASTPROGRESSDT { get; set; }
        public string? MREMARKS { get; set; }
        public string? DELAYREASON { get; set; }
        public string? EXPCOMPDT { get; set; }
        public string? FINALBILL { get; set; }
        public decimal? TOTALEXP { get; set; }
        public string? WORK_ID { get; set; }
        public string? FPSTATUS { get; set; }
        public decimal? PAC { get; set; }  //,imagename, imagename2, imagename3, imagename4, ImageName5,IsMongo
        public string? IMAGENAME { get; set; }
        public string? IMAGENAME2 { get; set; }
        public string? IMAGENAME3 { get; set; }
        public string? IMAGENAME4 { get; set; }
        public string? IMAGENAME5 { get; set; }
        public string? ISMONGO { get; set; }
        public Int32? SR { get; set; }


    }
}
