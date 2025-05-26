using System;
using System.ComponentModel.DataAnnotations;

namespace HIMIS_API.Models.DTOs
{
    public class DivPerformanceDTO
    {
        [Key]


        public Int64 SN { get; set; }
        public string? DIVISION { get; set; }
        public string? DIVISIONID { get; set; }
        public Int32? DELAYWORK { get; set; }
        public Int32? ONTIME { get; set; }
      
        
        public Double? TVCCR { get; set; }

        public Double? EXPCR { get; set; }  
    

    }
}
