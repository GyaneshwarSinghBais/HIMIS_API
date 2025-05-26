using System;
using System.ComponentModel.DataAnnotations;

namespace HIMIS_API.Models.DTOs
{
    public class WorkContractorDTO
    {
        [Key]

        
        public Int32? PPID { get; set; }
        public string? CONTRACTORID { get; set; }
        public string? CNAME { get; set; }
        public Int32? NOSWORKS { get; set; }
        public Int32? WLSTATUSID { get; set; }
        public string? PSTATUS { get; set; }
        public string? PARENTPROGRESS { get; set; }
        
        public decimal? TOTALEXPLAC { get; set; }

        public decimal? WORKVALUELC { get; set; }  
    

    }
}
