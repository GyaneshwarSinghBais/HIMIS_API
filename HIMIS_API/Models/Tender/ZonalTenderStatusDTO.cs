using System.ComponentModel.DataAnnotations;

namespace HIMIS_API.Models.Tender
{
    public class ZonalTenderStatusDTO
    {
        [Key]
        public Int32? TID { get; set; }
        //public int? WorkId { get; set; }
        public string? TenderStatus { get; set; }
        public Int32? CntTender { get; set; }
    }
}
