namespace synced.Models
{
    public class ProjectCardModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Owner { get; set; }
        public int CurrentUserId { get; set; }
    }
}
