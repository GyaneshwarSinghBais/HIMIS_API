namespace HIMIS_API.Models.WebCGMSC
{
    public class CivilTenderDTO
    {
        public string Url { get; set; }
        public string Content_Registration_Id { get; set; }
        public string Attachment_Id { get; set; }
        public string Caption { get; set; }
        public string Content_Discription { get; set; }
        public string Subject { get; set; }
        public string Content_Subject { get; set; }
        public DateTime Content_Publising_Date { get; set; }
        public string? Expiry_Date_of { get; set; }
        public DateTime? Expiry_DateOnNotice_Board { get; set; }
        public string DisplayNew { get; set; }
    }
}
