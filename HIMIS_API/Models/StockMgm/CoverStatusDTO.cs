using System.ComponentModel.DataAnnotations;

namespace HIMIS_API.Models.StockMgm
{
    public class CoverStatusDTO
    {
        [Key]
        public int? CSID { get; set; }
        public string? CStatus { get; set; }
        public int? CntTender { get; set; }
        public decimal? tValue { get; set; }
    }
}
