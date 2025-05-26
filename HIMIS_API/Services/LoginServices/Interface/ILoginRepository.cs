using HIMIS_API.Models;
using Microsoft.AspNetCore.Mvc;

namespace HIMIS_API.Services.LoginServices.Interface
{
    public interface ILoginRepository
    {
        Task<IEnumerable<LoginAdmin>> GetAdminLoginDDLAsync();
        Task<(bool success, AdminUserModel user)> LoginAsync(long agencyId, string password);
        Task<(bool success, FieldLogin user)> FieldLoginAsync(string agencyId, string password);
      //  Task<IEnumerable> LoginField(FieldLogin model);
    }
}
