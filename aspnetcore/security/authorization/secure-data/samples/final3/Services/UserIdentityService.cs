using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ContactManager.Services
{
    public class UserIdentityService : IUserIdentityService
    {
        public async Task<string> SaveAsUserIdentity(UserManager<IdentityUser> userManager, string email, string password)
        {
            System.Console.WriteLine("Bhka!");
            var user = await userManager.FindByNameAsync(email);
            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = email,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(user, password);
            }
            return user.Id;
        }
    }
}
