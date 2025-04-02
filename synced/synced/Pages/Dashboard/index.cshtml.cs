    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.IdentityModel.Tokens;
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


            public void OnGet(int projectId)
            {
                if(this.ProjectId.HasValue)
                {
                ProjectId = projectId;
                ViewData["ProjectId"] = projectId; // Slaat de ID op voor de layout.
                _tasks = _taskService.GetAllTasks(projectId);
            }
                if (_tasks == null)
                {
                    _tasks = new TaskGroupDto();  // Or any appropriate default collection
                }

                HttpContext.Session.SetInt32("ProjectId", (int)ProjectId);
            }

            public IActionResult OnPostAsync()
            {
                TaskDto taskDto = Mapper.MapCreate<TaskDto>(this.TaskCard.Task);
                taskDto.Deadline = new DateTime(1753, 1, 1);
                taskDto.Project_id = this.ProjectId.Value;

                if (TaskCard.Task.Id <= 0)
                {
                    if (CreatTask(taskDto)) return RedirectToPage();

                }
                if (UpdateTask(taskDto)) return RedirectToPage();

                return Page();
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

        [HttpPost]
        public bool CreatTask(TaskDto taskDto)
            {
                return this._taskService.CreateTask(taskDto);
            }

            public bool UpdateTask(TaskDto taskDto)
            {
                return this._taskService.UpdateTask(taskDto);
            }


        }
    }
