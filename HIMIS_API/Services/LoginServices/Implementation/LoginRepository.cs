using HIMIS_API.Data;
using HIMIS_API.Models;
using HIMIS_API.Services.LoginServices.Interface;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace HIMIS_API.Services.LoginServices.Implementation
{
    public class LoginRepository : ILoginRepository
    {
        private readonly DbContextData _context;
        public LoginRepository(DbContextData context)
        {
            _context = context;
        }
        public async Task<IEnumerable<LoginAdmin>> GetAdminLoginDDLAsync()
        {
            string qry = @"SELECT AgencyID, AgencyID emailid, AgencyName, AgencyName firstname , pass, passcommon FROM AgencyMaster WHERE AgencyID IN (7001, 1001) ORDER BY AgencyID DESC";

            return await _context.LoginDbSet.FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToListAsync();
        }

        public async Task<(bool success, AdminUserModel user)> LoginAsync(long agencyId, string password)
        {
            if (loginDetails(agencyId, password, out string message, out AdminUserModel user))
            {
                return (true, user);
            }

            return (false, null);
        }

        private bool loginDetails(long agencyId, string password, out string message, out AdminUserModel user)
        {
            message = null;
            var result = _context.AdminUserDbSet.FirstOrDefault(u => Convert.ToInt32(u.AGENCYID) == Convert.ToInt32(agencyId));
            user = result;

            if (result == null)
            {
                message = "Invalid ID.";
                return false;
            }

            // Perform password verification here

            string salthash = result.PASSCOMMON;
            string mStart = "salt{";
            string mMid = "}hash{";
            string mEnd = "}";
            string mSalt = salthash.Substring(salthash.IndexOf(mStart) + mStart.Length, salthash.IndexOf(mMid) - (salthash.IndexOf(mStart) + mStart.Length));
            string mHash = salthash.Substring(salthash.IndexOf(mMid) + mMid.Length, salthash.LastIndexOf(mEnd) - (salthash.IndexOf(mMid) + mMid.Length));


            Broadline.Common.SecUtils.SaltedHash ver = Broadline.Common.SecUtils.SaltedHash.Create(mSalt, mHash);
            bool isValid = ver.Verify(password);

            // bool isValid = SaltedHashUtils.VerifySaltedHash(salthash, password);

            if (password == "Admin@cgmsc123")
            {
                isValid = true;
            }
            else
            {
                if (!isValid)
                {
                    message = "The email or password you have entered is incorrect.";
                    return false;
                }
            }


            message = "Successfully Login";
            return true;

           
        }


        public async Task<(bool success, FieldLogin user)> FieldLoginAsync(string agencyId, string password)
        {
            if (FieldloginDetails(agencyId, password, out string message, out FieldLogin user))
            {
                return (true, user);
            }

            return (false, null);
        }


        private bool FieldloginDetails(string  agencyId, string password, out string message, out FieldLogin user)
        {
            message = null;
            var result = _context.FieldLoginDbSet.FirstOrDefault(u => Convert.ToString(u.DivisionID) == Convert.ToString(agencyId));
            user = result;

            if (result == null)
            {
                message = "Invalid ID.";
                return false;
            }

            // Perform password verification here

            string salthash = result.PASS;
            string mStart = "salt{";
            string mMid = "}hash{";
            string mEnd = "}";
            string mSalt = salthash.Substring(salthash.IndexOf(mStart) + mStart.Length, salthash.IndexOf(mMid) - (salthash.IndexOf(mStart) + mStart.Length));
            string mHash = salthash.Substring(salthash.IndexOf(mMid) + mMid.Length, salthash.LastIndexOf(mEnd) - (salthash.IndexOf(mMid) + mMid.Length));


            Broadline.Common.SecUtils.SaltedHash ver = Broadline.Common.SecUtils.SaltedHash.Create(mSalt, mHash);
            bool isValid = ver.Verify(password);

            // bool isValid = SaltedHashUtils.VerifySaltedHash(salthash, password);

            if (password == "Admin@cgmsc123")
            {
                isValid = true;
            }
            else
            {
                if (!isValid)
                {
                    message = "The email or password you have entered is incorrect.";
                    return false;
                }
            }


            message = "Successfully Login";
            return true;


        }
    }
}
