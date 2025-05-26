namespace HIMIS_API.Models.WebCGMSC
{
    public class MostVisitedContentDTO
    {
        public DateTime date_timestamp { get; set; }
        public string Content_Registration_Id { get; set; }
        public string Content_Subject { get; set; }
        public DateTime Content_Publising_Date { get; set; }
        public string ContentCategoryName { get; set; }
        public DateTime? Expiry_DateOnNotice_Board { get; set; }
        public string Url { get; set; }
    }
}
