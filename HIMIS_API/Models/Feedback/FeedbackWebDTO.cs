namespace HIMIS_API.Models.Feedback
{
    public class FeedbackWebDTO
    {
        public int? FeedbackId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? MobileNumber { get; set; }
        public string? City { get; set; }
        public string? Subject { get; set; }
        public int? FeedbackTypeId { get; set; }
        public string? AttachmentPath { get; set; }
        public string? Comments { get; set; }
        public int? TopicId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
