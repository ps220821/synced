using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using synced.Core.Results;
using synced_BBL.Dtos;
using synced_BBL.Interfaces;
using synced_BBL.Services;

namespace synced.Pages
{
    public class RegisterPageModel : PageModel
    {
        private IUserService _userService;

        public RegisterPageModel(IUserService userService)
        {
            this._userService = userService;
        }

        [BindProperty]
        public UserDto userDto { get; set; }
        public string ErrorMessage { get; set; }


        public void OnGet()
        {
        }


        public async Task<IActionResult> OnPost()
        {
            if (userDto != null)
            {
                OperationResult<int> result = await this._userService.RegisterUser(userDto);

                if (result.Succeeded)
                {
                    return RedirectToPage("/Auth/LoginPage");  // Redirect to another page on success
                }

                else
                {
                    // Als registratie niet succesvol is, toon een foutmelding
                    ErrorMessage = result.Message;
                }

            }
            else
            {
                ErrorMessage = "Please fill out all the input fields";

            }

            return Page();


        }
    }
}
