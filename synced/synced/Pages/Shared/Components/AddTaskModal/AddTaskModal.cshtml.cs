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
        private AddTaskCardModel taskCardModel;
        private List<UserDto> userDtoList;

        public AddTaskModal(IProjectUserService projectUserService)
        {
            this._projectUserService = projectUserService;
        }

        public IViewComponentResult Invoke(TaskCardModel? task, int? project_id)
        {
            AddTaskCardModel newTask = new AddTaskCardModel();

            if (task != null) // Fix: Ensure task is not null before mapping
            {
                newTask = Mapper.MapCreate<AddTaskCardModel>(task);
                newTask.Id = task.Id;
            }

            int projectId = task?.Project_id ?? project_id ?? 0; // Ensure project_id is not null
            this.userDtoList = _projectUserService.GetAllUsers(projectId);
            newTask.Users = userDtoList;

            return View("AddTaskModal", newTask ?? new AddTaskCardModel());
        }
    }
}
