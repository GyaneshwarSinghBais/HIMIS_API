using System.ComponentModel.DataAnnotations;

namespace HIMIS_API.Models.Feedback
{
    public class MobileVerificationOTPDTO
    {
        [Key]
        public int? OTPId { get; set; }
        public string? MobileNumber { get; set; }
        public string? OTP { get; set; }
        public bool? IsVerified { get; set; }
        public DateTime? ExpiryTime { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
