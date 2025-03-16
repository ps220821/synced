using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using synced_BBL.Dtos;  // Assuming this contains your project DTOs
using synced_BBL.Interfaces;
using synced_BBL.Services;  // Assuming this contains the ProjectService interface

namespace synced.Pages
{
    public class ProjectsPageModel : PageModel
    {
        private readonly IProjectService _projectService;
        private readonly ProjectUserService _projectUserService;
        public int SessionUserId;
        public List<ProjectDto> Projects { get; private set; }

        [BindProperty]
        public ProjectDto NewProject { get; set; }

        public ProjectsPageModel(IProjectService projectService, ProjectUserService projectUserService)
        {
            _projectService = projectService;
            _projectUserService = projectUserService;
        }

        public IActionResult OnGet()
        {
            // Try to get the UserId from the session
            this.SessionUserId = HttpContext.Session.GetInt32("UserId") ?? 0;

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
                if (this._projectService.DeleteProject(projectId))
                {
                    return RedirectToPage();
                }
            }
            if (this._projectUserService.RemoveUserFromProject(userId, projectId))
            {
                    return RedirectToPage();

            }
            return Page();
        }
    }
}
