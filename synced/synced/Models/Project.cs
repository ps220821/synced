namespace synced.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int OwnerId { get; set; }
        public DateTime Start_Date { get; set; }
        public DateTime End_Date { get; set; }
    }
}
