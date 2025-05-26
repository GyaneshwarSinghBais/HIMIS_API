using System.ComponentModel.DataAnnotations;


namespace HIMIS_API.Models.DTOs
{
    public class DMEProgressCountDTO
    {

  
        [Key]
        public string hc_id { get; set; }
        public string? NAme_eng { get; set; }
        
        public string District_ID { get; set; }
        public string? Districtname { get; set; }
        public Int32 ToBeTender1001 { get; set; }
        public Int32 TenderProcess2001 { get; set; }
        public Int32 AccWorkOrder3001 { get; set; }
        public Int32 Completed4001 { get; set; }
        public Int32 Running5001 { get; set; }
        public Int32 LandIssue6001 { get; set; }
        public Int32 RetunDept8001 { get; set; }
        public Int32 Total { get; set; }
        public string? DivisionID { get; set; }
    }
}
