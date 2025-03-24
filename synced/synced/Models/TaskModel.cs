namespace synced.Models
{
    public enum Priorities
    {
        high,
        medium,
        low,
    }

    public enum Status
    {
        todo,
        inprogress,
        done
    }

    public class TaskModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public Status Status { get; set; }

        public Priorities Priority { get; set; }

        public DateTime Deadline { get; set; }

        public int User_id { get; set; }

        public int Project_id { get; set; }
    }
}
