using ContactManager.Authorization;
using ContactManager.Data;
using ContactManager.Models;
using ContactManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ContactManager.Pages.Contacts
{
    #region snippet
    public class DetailsModel : DI_BasePageModel
    {
        IUserIdentityService _userIdentityService;
        UserManager<IdentityUser> _userManager;

        public DetailsModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager,
            IUserIdentityService userIdentity)
            : base(context, authorizationService, userManager)
        {
            _userManager = userManager;
            _userIdentityService = userIdentity;
        }

        public Contact Contact { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Contact = await Context.Contact.FirstOrDefaultAsync(m => m.ContactId == id);

            if (Contact == null)
            {
                return NotFound();
            }

            var isAuthorized = User.IsInRole(Constants.ContactManagersRole) ||
                               User.IsInRole(Constants.ContactAdministratorsRole);

            var currentUserId = UserManager.GetUserId(User);

            if (!isAuthorized
                && currentUserId != Contact.OwnerID
                && Contact.Status != ContactStatus.Approved)
            {
                return Forbid();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id, ContactStatus status)
        {
            var contact = await Context.Contact.FirstOrDefaultAsync(
                                                      m => m.ContactId == id);

            if (contact == null)
            {
                return NotFound();
            }

            var contactOperation = (status == ContactStatus.Approved)
                                                       ? ContactOperations.Approve
                                                       : ContactOperations.Reject;

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, contact,
                                        contactOperation);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }
            contact.Status = status;
            Context.Contact.Update(contact);
            await Context.SaveChangesAsync();

            // SaveAsAnIdentityUser
            if (status == ContactStatus.Approved) { 
                await _userIdentityService.SaveAsUserIdentity(_userManager, contact.Email, "SuperSecretPassw0rD!");
            }

            return RedirectToPage("./Index");
        }
    }
    #endregion
}
