using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using synced.Core.Results;
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

        public async Task OnGetAsync()
        {
            ProjectId = HttpContext.Session.GetInt32("ProjectId") ?? 0;

            OperationResult<List<UserDto>> resultUser = await _projectUserService.GetAllUsers(ProjectId);

            if (resultUser.Succeeded && resultUser.Data != null)
            {
                Users = resultUser.Data;
            }
            else
            {
                TempData["ErrorMessage"] = resultUser.Message ?? "Failed to load users.";
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ProjectId = HttpContext.Session.GetInt32("ProjectId") ?? 0;

            if (!string.IsNullOrEmpty(NewUserEmail))
            {
                int userId = _userService.GetUserBYEmail(NewUserEmail);

                if (userId > 0 && ProjectId > 0)
                {
                    OperationResult<bool> result = await _projectUserService.AddUserToProject(ProjectId, userId);

                    if (result.Succeeded && result.Data)
                    {
                        TempData["SuccessMessage"] = $"User '{NewUserEmail}' added to project successfully!";
                        return RedirectToPage();
                    }
                    else
                    {
                        TempData["ErrorMessage"] = result.Message ?? $"Failed to add user '{NewUserEmail}' to project.";
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

        public async Task<IActionResult> OnPostDeleteAsync(int userId)
        {
            ProjectId = HttpContext.Session.GetInt32("ProjectId") ?? 0;

            OperationResult<bool> result = await _projectUserService.RemoveUserFromProject(userId, ProjectId);

            if (result.Succeeded && result.Data)
            {
                return RedirectToPage();
            }

            TempData["ErrorMessage"] = result.Message ?? "Something went wrong while removing user. Try again later.";
            return Page();
        }
    }
}