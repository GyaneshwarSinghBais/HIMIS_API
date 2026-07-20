using System.ComponentModel.DataAnnotations;

namespace HIMIS_API.Models.LandIssue
{
    public class FeedbackSuggestion
    {
        [Key]
        public int FeedbackID { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string MobileNumber { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public int ComplainTypeID { get; set; }
        public int ComplainID { get; set; }
        public string Comments { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public string? PdfFilePath { get; set; }

        // Navigation Properties
        //public ComplainType? ComplainType { get; set; }
        //public Complain? Complain { get; set; }
    }
}
