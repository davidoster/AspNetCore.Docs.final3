using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ContactManager.Services
{
    public interface IUserIdentityService
    {
        Task<string> SaveAsUserIdentity(UserManager<IdentityUser> userManager, string email, string password);
    }
}
