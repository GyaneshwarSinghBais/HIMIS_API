using HIMIS_API.Data;
using HIMIS_API.Models.DTOs;
using HIMIS_API.Models.EMS;
using HIMIS_API.Models.Feedback;
using HIMIS_API.Models.WebCGMSC;
using HIMIS_API.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace HIMIS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly DbContextWeb _context;
        private readonly DbContextData _contextData;
        private readonly FacOperation _facOperation;

        public FeedbackController(DbContextWeb context, DbContextData contextData)
        {
            _context = context;
            _contextData = contextData;
            _facOperation = new FacOperation(_contextData);
        }

        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] SendOtpRequestDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.MobileNumber))
                return BadRequest("Mobile number is required.");

            var otp = _facOperation.sendOtpSms(request.MobileNumber);

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
        [HttpPost("SubmitFeedback")]
        public async Task<IActionResult> SubmitFeedback([FromForm] SubmitFeedbackRequestDTO request, IFormFile? file)
        {
            var isVerified = await _context.MobileOtpsDbSet
                .AnyAsync(o => o.MobileNumber == request.MobileNumber && o.IsVerified == true);

            if (!isVerified)
                return BadRequest("Mobile number not verified.");

            string? savedFilePath = null;
            if (file != null)
            {
                if (file.ContentType != "application/pdf")
                    return BadRequest("Only PDF files are allowed.");
                if (file.Length > 2 * 1024 * 1024)
                    return BadRequest("File size must not exceed 2MB.");

                var uploadDir = @"D:\IIS\Upload\Documents";
                if (!Directory.Exists(uploadDir))
                    Directory.CreateDirectory(uploadDir);

                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                var filePath = Path.Combine(uploadDir, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                savedFilePath = filePath;
            }

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
                AttachmentPath = savedFilePath,
                Comments = request.Comments,
                TopicId = request.TopicId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.FeedbacksDbSet.Add(feedback);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Feedback submitted successfully." });
        }

        //[HttpGet("DownloadSamplePdf")]
        //public IActionResult DownloadSamplePdf()
        //{
        //    var uploadDir = @"D:\IIS\Upload\Documents";
        //    var fileName = "ad165820-fcb0-4f63-9d3b-cadad7adac03_ARPIT THAKUR-2.pdf";
        //    var filePath = Path.Combine(uploadDir, fileName);

        //    if (!System.IO.File.Exists(filePath))
        //        return NotFound("File not found.");

        //    var fileBytes = System.IO.File.ReadAllBytes(filePath);
        //    return File(fileBytes, "application/pdf", fileName);
        //}


        //https://localhost:7247/api/Feedback/GetFeedBack?feedbackTypeId=1&topicId=3
        [HttpGet("GetFeedBack")]
        public async Task<ActionResult<IEnumerable<GetFeedBackDTO>>> GetFeedBack(Int32 feedbackTypeId, Int32 topicId)
        {
            string whfeedbackTypeId = "";
            string whtopicId = "";

            if (feedbackTypeId != 0)
            {
                whfeedbackTypeId = @"   and ft.FeedbackTypeId = "+ feedbackTypeId + "  ";
            }
            if (topicId != 0)
            {
                whtopicId = @"  and t.TopicId = "+ topicId + "  ";
            }


            string query = $@" 
SELECT f.FeedbackId,f.FirstName,f.LastName,f.Email,f.Address,f.MobileNumber,f.City,f.Subject,f.FeedbackTypeId,ft.feedbacktypename, f.AttachmentPath,f.Comments,f.TopicId,t.topicname,f.CreatedAt,f.UpdatedAt
  FROM FeedbackWeb f
  inner join FeedbackTypeMaster ft on ft.FeedbackTypeId = f.FeedbackTypeId
  inner join TopicMaster t on t.topicid = f.topicid
  where 1=1 "+ whfeedbackTypeId + @" "+ whtopicId + @" ";

            

            var result = await _context.GetFeedBackDbSet
                .FromSqlRaw(query)
                .ToListAsync();

            return Ok(result);
        }


    }
}
