using Microsoft.AspNetCore.Mvc;
using synced.Models;

namespace synced.ViewComponents
{
    public class ProjectCardViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(int id, string name, string description, int ownerId, int currentUserId)
        {
            var model = new ProjectCardModel
            {
                Id = id,
                Name = name,
                Description = description,
                OwnerId = ownerId,
                CurrentUserId = currentUserId
            };
            return View("ProjectCard", model);
        }
    }
}