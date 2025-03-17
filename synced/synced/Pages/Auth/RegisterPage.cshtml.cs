using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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


        public IActionResult OnPost()
        {
            if (userDto != null)
            {
                if (this._userService.RegisterUser(userDto))
                {
                    return RedirectToPage("/LoginPage");  // Redirect to another page on success
                }

            }

            ErrorMessage = "Please fill out all the input fields";
             
            return Page();
        }
    }
}
