using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using synced.Core.Results;
using synced_BBL.Dtos;
using synced_BBL.Interfaces;
using synced_BBL.Services;
using synced_DALL.Entities;


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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!string.IsNullOrEmpty(newUser.email) && !string.IsNullOrEmpty(newUser.password))
            {
                // Use await here to get the OperationResult<int>
                OperationResult<int> userId = await _userService.LoginUser(newUser);

                // Check if login was successful
                if (userId.Succeeded)
                {
                    // Store user id in session if login successful
                    HttpContext.Session.SetInt32("UserId", userId.Data);
                    await HttpContext.Session.CommitAsync();  // Commit session changes

                    return RedirectToPage("/Dashboard/Projects/ProjectsPage");
                }
                else
                {
                    // Display error message if login fails
                    ErrorMessage = userId.Message ?? "Invalid login attempt.";
                    return Page();
                }
            }
            return Page();
        }
    }
}