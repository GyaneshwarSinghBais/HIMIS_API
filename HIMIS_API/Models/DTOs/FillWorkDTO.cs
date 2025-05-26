using System;
using System.ComponentModel.DataAnnotations;


namespace HIMIS_API.Models.DTOs
{
    public class FillWorkDTO
    {
      

        [Key]

        public string? WORK_ID { get; set; }
        public Int32? mainschemeid { get; set; }
        public Int32? district_id { get; set; }
        public string? divisionid { get; set; }
        public string? searchname { get; set; }
        
       
    }
}
