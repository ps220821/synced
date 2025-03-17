using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using synced_BBL.Interfaces;

namespace synced.Pages.Dashboard
{
    public class IndexModel : PageModel
    {
        private readonly ITaskService _taskService; 

        [BindProperty(SupportsGet = true)]
        public int? Id { get; set; }

        public IndexModel(ITaskService taskService)
        {
            _taskService = taskService;
        }


        public void OnGet()
        {
            if(this.Id.HasValue)
            {
                this._taskService.GetAllTasks((int)this.Id.Value);
            }
        }
    }
}
