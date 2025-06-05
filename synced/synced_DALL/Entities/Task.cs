using System;

namespace synced_DALL.Entities
{
    public enum Priorities
    {
        high,
        medium,
        low
    }

    public enum Status
    {
        todo,
        inprogress,
        done
    }

    public class Task
    {
        // ───────────── Backing fields ─────────────
        private string _title;
        private string _description;
        private DateTime _deadline;
        private int _projectId;
        private int? _userId;
        private User _user;

        // ───────────── Properties ─────────────
        public int Id { get; private set; }
        public Status Status { get; private set; }
        public Priorities Priority { get; private set; }
        public Project Project { get; private set; }

        public string Title
        {
            get => _title;
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Title cannot be empty.");
                _title = value.Trim();
            }
        }

        public string Description
        {
            get => _description;
            private set
            {
                _description = value?.Trim();
            }
        }
        public User User
        {
            get => _user;
            private set
            {
                _user = value;
            }
        }

        public DateTime Deadline
        {
            get => _deadline;
            private set
            {
                _deadline = value;
            }
        }

        public int? UserId
        {
            get => _userId;
            private set
            {
                _userId = value;
            }
        }


        public int ProjectId
        {
            get => _projectId;
            private set
            {
                if (value <= 0)
                    throw new ArgumentException("ProjectId must be a positive integer.");
                _projectId = value;
            }
        }


        public static Task Create(
            string title,
            string description,
            Status status,
            Priorities priority,
            DateTime deadline,
            int? userId,
            int projectId)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty.");
            if (projectId <= 0)
                throw new ArgumentException("ProjectId must be a positive integer.");

            return new Task
            {
                Title = title,
                Description = description,
                Status = status,
                Priority = priority,
                Deadline = deadline,
                UserId = userId,
                ProjectId = projectId
            };
        }

        internal Task(
            int id,
            string title,
            string description,
            Status status,
            Priorities priority,
            DateTime deadline,
            int? userId,
            int projectId)
        {
            Id = id;
            _title = title;
            _description = description;
            Status = status;
            Priority = priority;
            _deadline = deadline;
            _userId = userId;
            _projectId = projectId;
        }

        protected Task() { }

        // ───────────── Internal rehydrate‐factory ─────────────
        internal static Task Rehydrate(
            int id,
            string title,
            string description,
            Status status,
            Priorities priority,
            DateTime deadline,
            int? userId,
            int projectId,
            User? AssginedUser,
            Project? project)
        {
            var t = new Task(
                id,
                title,
                description,
                status,
                priority,
                deadline,
                userId,
                projectId
            );

            if (AssginedUser != null)
            {
                typeof(Task).GetProperty(nameof(User))
                             .SetValue(t, AssginedUser);
            }

            if (project != null)
            {
                typeof(Task).GetProperty(nameof(Project))
                             .SetValue(t, project);
            }

            return t;
        }

        public static Task FromExisting(
            int id,
            string title,
            string description,
            Status status,
            Priorities priority,
            DateTime deadline,
            int? userId,
            int projectId)
        {
            var temp = Create(title, description, status, priority, deadline, userId, projectId);
            temp.Id = id;
            return temp;
        }
    }
}
