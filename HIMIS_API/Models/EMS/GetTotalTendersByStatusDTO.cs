using System.ComponentModel.DataAnnotations;

namespace HIMIS_API.Models.EMS
{
    public class GetTotalTendersByStatusDTO
    {
        [Key]
        public int? Csid { get; set; }
        public string? CStatus { get; set; }
        public int? NoofTender { get; set; }
        public decimal? TenderValue { get; set; }
    }
}
