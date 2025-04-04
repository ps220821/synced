    using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.IdentityModel.Tokens;
using synced.Core.Results;
using synced.Models;
    using synced_BBL.Dtos;
    using synced_BBL.Interfaces;
using synced_DAL.Entities;
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


            public async void OnGet(int projectId)
            {
                if(this.ProjectId.HasValue)
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
                    _tasks = new TaskGroupDto();  // Or any appropriate default collection
                }

                HttpContext.Session.SetInt32("ProjectId", (int)ProjectId);
            }

        public async Task<IActionResult> OnPostAsync()
        {
            TaskDto taskDto = Mapper.MapCreate<TaskDto>(this.TaskCard.Task);
            taskDto.Deadline = new DateTime(1753, 1, 1);
            taskDto.Project_id = this.ProjectId.Value;

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
                    task_id = taskId,
                    user_id = currentUserId,
                    comment = commentText,
                    created_at = DateTime.Now,
                };

                this._taskCommentService.AddComment(NewComment);

                // Return alleen de comments partial
                var updatedComments = _taskCommentService.GetTaskComments(taskId);
                return Partial("/Pages/Shared/Components/AddTaskModal/CommentsList.cshtml", updatedComments);
            }
            return BadRequest();
        }


        public async Task<OperationResult<bool>> CreateTask(TaskDto taskDto)
        {
            // Call CreateTask in service
            return await this._taskService.CreateTask(taskDto);  // Returns the result of task creation
        }

        public async Task<OperationResult<bool>> UpdateTask(TaskDto taskDto)
        {
            // Call UpdateTask in service
            return await this._taskService.UpdateTask(taskDto);  // Returns the result of task update
        }


    }
}
