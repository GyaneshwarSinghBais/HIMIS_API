namespace HIMIS_API.Models.StockMgm
{
    public class CoverStatusTenderDetailDTO
    {
        public int? tender_id { get; set; }
        public int? item_ID { get; set; }
        public string? categoryName { get; set; }
        public string? item_code { get; set; }
        public string? item_Name { get; set; }
        public double? estimated_cost { get; set; }
        public double? tender_quantity { get; set; }
        public double? TenderValue { get; set; }
    }

   
}
