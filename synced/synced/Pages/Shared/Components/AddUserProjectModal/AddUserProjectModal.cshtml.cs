using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using synced.Models;
using velocitaApi.Mappers;

namespace synced.Pages.Shared.Components.AddUserProjectModal
{
    public class AddUserProjectModal : ViewComponent
    {
        public IViewComponentResult Invoke(int? projectId = null)
        {
            return View("AddUserProjectModal"); // Explicitly specify the view name
        }
    }

}
