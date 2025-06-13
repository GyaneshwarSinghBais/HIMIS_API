namespace HIMIS_API.Models.EMS
{
    public class EqToBeTenderDetailDTO
    {
        public int? INDENT_CONSOLIDATION_ID { get; set; }
        public string? indent_con_no { get; set; }
        public string? CONSOLIDATED_DATE { get; set; }
        public string? description { get; set; }
        public int? item_id { get; set; }
        public int? indent_cons_items_id { get; set; }
        public string? item_code { get; set; }
        public string? item_code_as_per_tender { get; set; }
        public string? item_name { get; set; }
        public string? item_desc { get; set; }
        public Int32? PROPOSED_QTY { get; set; }
        public Int32? FINAL_QTY { get; set; }
        public double? IndentValue { get; set; }
        public string? YEAR { get; set; }
        public int? USER_ID { get; set; }
        public int? DIRECTORATE_ID { get; set; }
        public int? FINANCIAL_YEAR_ID { get; set; }
        public string? facility_aut_name { get; set; }
        public string? facility_aut_code { get; set; }
        public string? EStatus { get; set; }
        public string? uploadStatus { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
