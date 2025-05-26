using HIMIS_API.Data;
using HIMIS_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace HIMIS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Login1Controller : ControllerBase
    {
        private readonly DbContextData _context;
        public Login1Controller(DbContextData context) 
        {
            _context = context;
        }

        [HttpGet("getAdminLoginDDL")]
        public async Task<ActionResult<IEnumerable<LoginAdmin>>> getAdminLoginDDL()
        {
            string qry = @" select AgencyID,AgencyName,pass,passcommon from AgencyMaster
where AgencyID in (7001,1001) order by AgencyID desc ";

            var myList =  _context.LoginDbSet
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();

            return myList;
        }

        [HttpPost]
        public IActionResult Login(LoginAdmin model)
        {
          

            loginDetails(model.AGENCYID, model.PASSCOMMON, out string message, out AdminUserModel user);

            if (message == "Successfully Login")
            {
                //return Ok(message);
                return Ok(new { Message = message, UserInfo = user });
            }

            return BadRequest("Invalid credentials.");
        }

        private bool loginDetails(Int64 agencyid, string password, out string message, out AdminUserModel user)
        {
            message = null;

            //var result = _context.MasFacilityWards
            //    .FirstOrDefault(w => w.wardid == wardId);

            var result = _context.AdminUserDbSet
               .FirstOrDefault(u =>Convert.ToInt32(u.AGENCYID) ==Convert.ToInt32(agencyid));

            user = result;

            if (result == null)
            {
                message = "Invalid ID.";
                return false;
            }


            // Perform password verification
            string salthash = result.PASSCOMMON;
            string mStart = "salt{";
            string mMid = "}hash{";
            string mEnd = "}";
            string mSalt = salthash.Substring(salthash.IndexOf(mStart) + mStart.Length, salthash.IndexOf(mMid) - (salthash.IndexOf(mStart) + mStart.Length));
            string mHash = salthash.Substring(salthash.IndexOf(mMid) + mMid.Length, salthash.LastIndexOf(mEnd) - (salthash.IndexOf(mMid) + mMid.Length));


            Broadline.Common.SecUtils.SaltedHash ver = Broadline.Common.SecUtils.SaltedHash.Create(mSalt, mHash);
            bool isValid = ver.Verify(password);

            // bool isValid = SaltedHashUtils.VerifySaltedHash(salthash, password);

            if (!isValid)
            {
                message = "The email or password you have entered is incorrect.";
                return false;
            }
            message = "Successfully Login";
            return true;
        }
    }
}
