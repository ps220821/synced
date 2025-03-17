using Microsoft.AspNetCore.Mvc;
using synced.Models;

namespace synced.ViewComponents
{
    public class AddProjectModalViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var model = new Project();
            return View("AddProjectModal", model);
        }
    }
}