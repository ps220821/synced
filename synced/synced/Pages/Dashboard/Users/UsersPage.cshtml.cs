using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using synced_BBL.Dtos;
using synced_BBL.Interfaces;
using synced_BBL.Services;

namespace synced.Pages.Dashboard.Users
{
    public class UsersPageModel : PageModel
    {
        public int ProjectId { get; set; }
        private readonly IUserService _userService;
        private readonly IProjectUserService _projectUserService;
        public List<UserDto> Users { get; set; } = new List<UserDto>();

        [BindProperty]
        public string NewUserEmail { get; set; }

        public UsersPageModel(IProjectUserService projectUserService, IUserService userService)
        {
            _projectUserService = projectUserService;
            _userService = userService;
        }

        public void OnGet()
        {
            ProjectId = HttpContext.Session.GetInt32("ProjectId") ?? 0;
            Users = _projectUserService.GetAllUsers(ProjectId);
        }

        public IActionResult OnPostAsync()
        {
            ProjectId = HttpContext.Session.GetInt32("ProjectId") ?? 0;

            if (!string.IsNullOrEmpty(NewUserEmail))
            {
                int userId = _userService.GetUserBYEmail(NewUserEmail);
                if (userId > 0 && ProjectId > 0)
                {
                    if (_projectUserService.AddUserToProject(ProjectId, userId))
                    {
                        TempData["SuccessMessage"] = $"User '{NewUserEmail}' added to project successfully!";
                        return RedirectToPage();
                    }
                    else
                    {
                        TempData["ErrorMessage"] = $"Failed to add user '{NewUserEmail}' to project.";
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = $"No user found with email '{NewUserEmail}'.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Please enter a valid email address.";
            }
            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int userId)
        {
            ProjectId = HttpContext.Session.GetInt32("ProjectId") ?? 0;
            if (this._projectUserService.RemoveUserFromProject(userId, ProjectId))
            {
                return RedirectToPage();
            }
            else
            {
                TempData["ErrorMessage"] = $"Some thing went wrong while removing user try again later";
            }
            return Page();
        }
    }
}