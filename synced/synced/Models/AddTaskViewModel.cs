using synced_BBL.Dtos;

namespace synced.Models
{
    public class AddTaskViewModel
    {
        public AddTaskCardModel Task { get; set; }
        public List<TaskCommentExtendedDto> Comments { get; set; }
        public TaskCommentDto NewComment { get; set; }
    }
}
