using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using synced.Models;
using synced_BBL.Dtos;
using synced_BBL.Interfaces;
using synced_BBL.Services;
using velocitaApi.Mappers;

namespace synced.Pages.Shared.Components.AddTaskModal
{
    public class AddTaskModal : ViewComponent
    {
        private readonly IProjectUserService _projectUserService;
        private readonly ITaskCommentService _taskCommentService;

        public AddTaskCardModel taskCardModel;
        private List<UserDto> userDtoList;
        public List<TaskCommentExtendedDto> comments;

        public AddTaskModal(IProjectUserService projectUserService, ITaskCommentService taskCommentService)
        {
            this._projectUserService = projectUserService;
            this._taskCommentService = taskCommentService;
        }
     
        public IViewComponentResult Invoke(TaskCardModel? task, int? project_id)
        {
            AddTaskCardModel newTask = new AddTaskCardModel();

            if (task != null)
            {
                newTask = Mapper.MapCreate<AddTaskCardModel>(task);
                newTask.Id = task.Id;
            }

            int projectId = task?.Project_id ?? project_id ?? 0;
            this.userDtoList = _projectUserService.GetAllUsers(projectId);
            newTask.Users = userDtoList;

            int taskId = task?.Id ?? 0;
            comments = taskId > 0 ? _taskCommentService.GetTaskComments(taskId) : new List<TaskCommentExtendedDto>();

            // ✅ Ensure taskCardModel is properly initialized
            taskCardModel = newTask ?? new AddTaskCardModel();

            var model = new AddTaskViewModel
            {
                Task = taskCardModel, // Now, taskCardModel is guaranteed to be non-null
                Comments = comments
            };

            return View("AddTaskModal", model);
        }
    }
}
