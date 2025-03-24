using synced_BBL.Dtos;

namespace synced.Models
{
    public class AddTaskCardModel : TaskCardModel
    {
        public List<UserDto>? Users { get; set; }
    }
}
