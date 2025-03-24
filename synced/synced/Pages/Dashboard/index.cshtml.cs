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
            public TaskGroupDto _tasks;

            [BindProperty(SupportsGet = true)]
            public int? ProjectId { get; set; }  // Changed from Id to ProjectId


            [BindProperty]
            public AddTaskCardModel TaskCard { get; set; } 

            public IndexModel(ITaskService taskService)
            {
                _taskService = taskService;
                this._tasks = new TaskGroupDto();
            }


            public void OnGet()
            {
                if(this.ProjectId.HasValue)
                {
                    _tasks = this._taskService.GetAllTasks((int)this.ProjectId.Value);
                }
                if (_tasks == null)
                {
                    _tasks = new TaskGroupDto();  // Or any appropriate default collection
                }

                HttpContext.Session.SetInt32("ProjectId", (int)ProjectId);
            }

            public IActionResult OnPostAsync()
            {
                TaskDto taskDto = Mapper.MapCreate<TaskDto>(this.TaskCard);
                taskDto.Deadline = new DateTime(1753, 1, 1);
                taskDto.Project_id = this.ProjectId.Value;

                if (TaskCard.Id <= 0)
                {
                    if (CreatTask(taskDto)) return RedirectToPage();

                }
            

                if (UpdateTask(taskDto)) return RedirectToPage();


                return Page();
            }

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
