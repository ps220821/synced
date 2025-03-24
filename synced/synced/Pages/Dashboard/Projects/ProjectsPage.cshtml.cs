using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using synced_BBL.Dtos;
using synced_BBL.Interfaces;  // Assuming this contains your project DTOs

namespace synced.Pages.Dashboard.Projects
{
    public class ProjectsPageModel : PageModel
    {
        private readonly IProjectService _projectService;
        private readonly IProjectUserService _projectUserService;

        public int SessionUserId;
        public List<ProjectDto> Projects { get; private set; }

        [BindProperty]
        public ProjectDto NewProject { get; set; }

        public ProjectsPageModel(IProjectService projectService, IProjectUserService projectUserService)
        {
            _projectService = projectService;
            _projectUserService = projectUserService;
        }

        public IActionResult OnGet()
        {
            HttpContext.Session.Remove("ProjectId");
            // Try to get the UserId from the session
            SessionUserId = HttpContext.Session.GetInt32("UserId") ?? 0;

            if (SessionUserId == 0)
            {
                return RedirectToPage("/LoginPage");
            }

            Projects = _projectService.GetAllProjects(SessionUserId);

            return Page(); // Return the page with the projects
        }

        public IActionResult OnPostAdd()
        {
            if (ModelState.IsValid)
            {
                NewProject.Owner = HttpContext.Session.GetInt32("UserId") ?? 0;

                if (_projectService.CreateProject(NewProject))
                {
                    return RedirectToPage();
                }
            }
            return Page();
        }


        public IActionResult OnPostDelete(int projectId, int ownerId)
        {
            int userId = (int)HttpContext.Session.GetInt32("UserId");
            if (userId == ownerId)
            {
                if (_projectService.DeleteProject(projectId))
                {
                    return RedirectToPage();
                }
            }
            if (_projectUserService.RemoveUserFromProject(userId, projectId))
            {
                return RedirectToPage();

            }
            return Page();
        }
    }
}
