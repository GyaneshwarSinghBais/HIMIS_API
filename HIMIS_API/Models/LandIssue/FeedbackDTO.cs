namespace HIMIS_API.Models.LandIssue
{
    public class FeedbackDTO
    {
        public int? FeedbackID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? MobileNumber { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Subject { get; set; }
        public int? ComplainTypeID { get; set; }
        public int? ComplainID { get; set; }
        public string? Comments { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? DivisionId { get; set; }   // <-- Added
        public string? Work_Id { get; set; }      // <-- Added
    }
}
