using System.ComponentModel.DataAnnotations;

namespace HIMIS_API.Models.EMS
{
    public class GetEqpTenderDTO
    {
        [Key]
        public string? Tender_No { get; set; }
        public int? Tender_Id { get; set; }
    }
}
