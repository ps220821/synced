using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using synced.Core.Results;
using synced.Models;
using synced_BBL.Dtos;
using synced_BBL.Interfaces;
using synced_DALL.Entities;
using velocitaApi.Mappers;

namespace synced.Pages.Dashboard
{
    public class IndexModel : PageModel
    {
        private readonly ITaskService _taskService;
        private readonly ITaskCommentService _taskCommentService;

        public TaskGroupDto _tasks;

        [BindProperty(SupportsGet = true)]
        public int? ProjectId { get; set; }  // Changed from Id to ProjectId


        [BindProperty]
        public AddTaskViewModel TaskCard { get; set; }

        public IndexModel(ITaskService taskService, ITaskCommentService taskCommentService)
        {
            this._taskService = taskService;
            this._taskCommentService = taskCommentService;
            this._tasks = new TaskGroupDto();
            this.TaskCard = new AddTaskViewModel();
        }


        public async System.Threading.Tasks.Task OnGetAsync(int projectId)
        {
            if (this.ProjectId.HasValue)
            {
                ProjectId = projectId;
                ViewData["ProjectId"] = projectId; // Slaat de ID op voor de layout.
                OperationResult<TaskGroupDto> result = await _taskService.GetAllTasks(projectId);

                if (result.Succeeded)
                {
                    _tasks = result.Data;
                }
                else if (!result.Message.IsNullOrEmpty())
                {
                    TempData["ErrorMessage"] = result.Message;
                }
                else
                {
                    _tasks = new TaskGroupDto();
                }
            }

            if (_tasks == null)
            {
                _tasks = new TaskGroupDto();  // Of een andere standaardwaarde
            }

             HttpContext.Session.SetInt32("ProjectId", (int)ProjectId);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            TaskDto taskDto = Mapper.MapCreate<TaskDto>(this.TaskCard.Task);
            taskDto.Deadline = new DateTime(1753, 1, 1);
            taskDto.ProjectId = this.ProjectId.Value;

            if (TaskCard.Task.Id <= 0)
            {
                OperationResult<bool> createResult = await CreateTask(taskDto);
                if (createResult.Succeeded) return RedirectToPage();
                TempData["ErrorMessage"] = createResult.Message;
            }
            else
            {
                OperationResult<bool> updateResult = await UpdateTask(taskDto);
                if (updateResult.Succeeded) return RedirectToPage();
                TempData["ErrorMessage"] = updateResult.Message;
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostComment(int taskId, string commentText)
        {
            if (!string.IsNullOrEmpty(commentText) && taskId > 0)
            {
                var currentUserId = HttpContext.Session.GetInt32("UserId") ?? 0;
                TaskCommentDto NewComment = new TaskCommentDto
                {
                    Id = taskId,
                    TaskId = taskId,
                    UserId = currentUserId,
                    Comment = commentText,
                    CreatedAt = DateTime.Now,
                };

                await _taskCommentService.AddComment(NewComment);

                var result = await _taskCommentService.GetTaskComments(taskId);

                if (result.Succeeded)
                {
                    var updatedComments = result.Data;
                    return Partial("/Pages/Shared/Components/AddTaskModal/CommentsList.cshtml", updatedComments);
                }
                return BadRequest(result.Message);
            }
            return BadRequest("Invalid input.");
        }


        public async Task<OperationResult<bool>> CreateTask(TaskDto taskDto)
        {
            return await this._taskService.CreateTask(taskDto);
        }

        public async Task<OperationResult<bool>> UpdateTask(TaskDto taskDto)
        {
            return await this._taskService.UpdateTask(taskDto);
        }


    }
}