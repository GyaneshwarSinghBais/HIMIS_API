namespace HIMIS_API.Models.EMS
{
    public class GetEqpRCDTO
    {
        public int? contract_item_id { get; set; }
        public string? item_codeE { get; set; }
        public string? item_nameE { get; set; }

        // ✅ Use decimal? since SQL is returning decimal
        public decimal? basic_rate { get; set; }
        public decimal? percentage { get; set; }
        public decimal? single_unit_price { get; set; }

        public string? model { get; set; }
        public string? contract_date { get; set; }
        public int? contract_duration { get; set; }
        public string? contract_end_date { get; set; }
        public string? name { get; set; }
        public string? tender_no { get; set; }
        public int? tender_id { get; set; }
        public string? webSiteUploadID { get; set; }
        public string? file_name { get; set; }
        public string? upload_folder_name { get; set; }
        public int? item_id { get; set; }
        public bool? is_extended { get; set; }
        public DateTime? contract_new_end_date { get; set; }
    }
}
