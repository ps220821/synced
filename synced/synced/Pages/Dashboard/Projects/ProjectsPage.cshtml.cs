using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using synced.Core.Results;
using synced_BBL.Dtos;
using synced_BBL.Interfaces;  // Assuming this contains your project DTOs

namespace synced.Pages.Dashboard.Projects
{
    public class ProjectsPageModel : PageModel
    {
        private readonly IProjectService _projectService;
        private readonly IProjectUserService _projectUserService;

        public int SessionUserId;
        public List<ProjectDto> Projects { get; private set; } = new();


        [BindProperty]
        public ProjectDto NewProject { get; set; }

        public ProjectsPageModel(IProjectService projectService, IProjectUserService projectUserService)
        {
            _projectService = projectService;
            _projectUserService = projectUserService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            HttpContext.Session.Remove("ProjectId");
            SessionUserId = HttpContext.Session.GetInt32("UserId") ?? 0;

            if (SessionUserId == 0)
            {
                return RedirectToPage("/LoginPage");
            }

            OperationResult<List<ProjectDto>> result = await _projectService.GetAllProjects(SessionUserId);

            if (result.Succeeded)
            {
                Projects = result.Data;
            }
            else
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "Could not get projects");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAdd()
        {
            if (ModelState.IsValid)
            {
                NewProject.OwnerId = HttpContext.Session.GetInt32("UserId") ?? 0;
                OperationResult<bool> result = await _projectService.CreateProject(this.NewProject);
                if (result.Succeeded)
                {
                    return RedirectToPage();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message ?? "Creating project failed.");
                }
            }
            return Page();
        }


        public async Task<IActionResult> OnPostDeleteAsync(int projectId, int ownerId)
        {
            int userId = (int)HttpContext.Session.GetInt32("UserId");
            if (userId == ownerId)
            {
                OperationResult<bool> result = await _projectService.DeleteProject(projectId);

                if (result.Succeeded)
                {
                    return RedirectToPage();
                }
                ModelState.AddModelError(string.Empty, result.Message ?? "Deleting project failed.");
            }
            OperationResult<bool> deleteResult = await _projectUserService.RemoveUserFromProject(userId, projectId);

            if (deleteResult.Succeeded)
            {
                return RedirectToPage();

            }
            return Page();
        }
    }
}
