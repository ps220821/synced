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
        private readonly IUserService _userService;

        public LoginPageModel(IUserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        public LoginDto newUser { get; set; } = new LoginDto();
        public string ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!string.IsNullOrEmpty(newUser.email) && !string.IsNullOrEmpty(newUser.password))
            {
                int userId = _userService.LoginUser(newUser);
                if (userId != 0)
                {
                    HttpContext.Session.SetInt32("UserId", userId);
                    HttpContext.Session.CommitAsync().Wait();
                    return RedirectToPage("/Dashboard/Projects/ProjectsPage");
                }
                else
                {
                    ErrorMessage = "Wrong email or password";
                    return Page();
                }
            }
            else
            {
                ErrorMessage = "Email or password need to be filled in";
                return Page();
            }
        }
    }
}
