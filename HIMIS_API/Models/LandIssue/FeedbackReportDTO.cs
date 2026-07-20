namespace HIMIS_API.Models.LandIssue
{
    public class FeedbackReportDTO
    {
        public int? FeedbackID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? MobileNumber { get; set; }
        public string? City { get; set; }
        public string? Subject { get; set; }
        public string? TypeName { get; set; }       // ComplainType Name
        public string? ComplainName { get; set; }   // Complain Name
        public string? Comments { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? PdfFilePath { get; set; }         // internal path
                                                         //public string? FileDownloadUrl { get; set; }  // for frontend
        public string? divname_en { get; set; }
        public string? work_text { get; set; }
       
    }
}
