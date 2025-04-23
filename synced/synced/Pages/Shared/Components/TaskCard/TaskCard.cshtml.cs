using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using synced.Models;
using synced_BBL.Dtos;

namespace synced.Pages.Shared.Components.TaskCard
{
    public class TaskCardViewComponent : ViewComponent
    {
      
        public IViewComponentResult Invoke(TaskDto task)
        {
            var model = new TaskCardModel
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = (Status)task.Status,
                Priority = (Priorities) task.Priority,
                Deadline = task.Deadline,
                User_id = task.UserId,
                Project_id = task.ProjectId,
            };

            return View("TaskCard", model);
        }
        public void OnGet()
        {
        }
    }
}
