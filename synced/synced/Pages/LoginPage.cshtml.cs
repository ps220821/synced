using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using synced_BBL.Dtos;
using synced_BBL.Interfaces;
using synced_BBL.Services;
using synced_DAL.Entities;


namespace synced.Pages
{
    public class LoginPageModel : PageModel
    {
        private IUserService _userService;

        public LoginPageModel(IUserService userService)
        {
            this._userService = userService;
        }

        [BindProperty]
        public LoginDto newUser { get; set; } = new LoginDto();
        public string ErrorMessage { get; set; }

        public void OnGet()
        {

        }

        public IActionResult OnPost()
        {
            // Handle login logic
            if (!newUser.email.IsNullOrEmpty() && !newUser.password.IsNullOrEmpty())  // Example credentials
            {
                int userId = _userService.LoginUser(newUser);

                if (userId != 0)
                {
                    HttpContext.Session.SetInt32("UserId", userId);
                    return RedirectToPage("/ProjectsPage");  // Redirect to another page on success
                }
                else
                {
                    ErrorMessage = "Wrong email or password";
                    return Page(); // Show the same page with an error message

                }

            }
            else
            {
                ErrorMessage = "Email or password need to filled in";
                return Page(); // Show the same page with an error message
            }
        }
    }
}
