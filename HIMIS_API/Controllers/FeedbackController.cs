using HIMIS_API.Data;
using HIMIS_API.Models.DTOs;
using HIMIS_API.Models.Feedback;
using HIMIS_API.Models.WebCGMSC;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace HIMIS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly DbContextWeb _context;

        public FeedbackController(DbContextWeb context)
        {
            _context = context;
        }

        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] SendOtpRequestDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.MobileNumber))
                return BadRequest("Mobile number is required.");

            // Generate random 6-digit OTP
            var random = new Random();
            var otp = random.Next(100000, 999999).ToString();

            var otpRecord = new MobileVerificationOTPDTO
            {
                MobileNumber = request.MobileNumber,
                OTP = otp,
                IsVerified = false,
                ExpiryTime = DateTime.Now.AddMinutes(5),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.MobileOtpsDbSet.Add(otpRecord);
            await _context.SaveChangesAsync();

            // TODO: Send OTP via SMS API here (e.g., Twilio, MSG91)
            Console.WriteLine($"[DEBUG] OTP for {request.MobileNumber}: {otp}");

            return Ok(new { message = "OTP sent successfully." });
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequestDTO request)
        {
            var otpRecord = await _context.MobileOtpsDbSet
                .Where(o => o.MobileNumber == request.MobileNumber && o.OTP == request.OTP)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();

            if (otpRecord == null || otpRecord.ExpiryTime < DateTime.Now)
                return BadRequest("Invalid or expired OTP.");

            otpRecord.IsVerified = true;
            otpRecord.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Mobile number verified successfully." });
        }

        // 3️⃣ Submit Feedback
        [HttpPost("submit")]
        public async Task<IActionResult> SubmitFeedback([FromBody] SubmitFeedbackRequestDTO request)
        {
            var isVerified = await _context.MobileOtpsDbSet
                .AnyAsync(o => o.MobileNumber == request.MobileNumber && o.IsVerified == true);

            if (!isVerified)
                return BadRequest("Mobile number not verified.");

            var feedback = new FeedbackWebDTO
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Address = request.Address,
                MobileNumber = request.MobileNumber,
                City = request.City,
                Subject = request.Subject,
                FeedbackTypeId = request.FeedbackTypeId,
                AttachmentPath = request.AttachmentPath,
                Comments = request.Comments,
                TopicId = request.TopicId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.FeedbacksDbSet.Add(feedback);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Feedback submitted successfully." });
        }



    }
}
