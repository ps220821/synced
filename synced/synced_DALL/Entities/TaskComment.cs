using System;

namespace synced_DALL.Entities
{
    public class TaskComment
    {
        // ───────────── Backing fields ─────────────
        private string _comment;
        private DateTime _createdAt;
        private int _userId;
        private int _taskId;

        // ───────────── Properties ─────────────
        public int Id { get; private set; }
        public Task Task { get; private set; }
        public User User { get; private set; }


        public int UserId
        {
            get => _userId;
            private set
            {
                if (value <= 0)
                    throw new ArgumentException("UserId must be a positive integer.");
                _userId = value;
            }
        }


        public int TaskId
        {
            get => _taskId;
            private set
            {
                if (value <= 0)
                    throw new ArgumentException("TaskId must be a positive integer.");
                _taskId = value;
            }
        }


        public string Comment
        {
            get => _comment;
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Comment cannot be empty.");
                _comment = value.Trim();
            }
        }

        public DateTime CreatedAt
        {
            get => _createdAt;
            private set
            {
                _createdAt = value;
            }
        }

        // ───────────── Factory for “new” TaskComment ─────────────
        public static TaskComment Create(
            int userId,
            int taskId,
            string comment,
            DateTime createdAt)
        {
            if (string.IsNullOrWhiteSpace(comment))
                throw new ArgumentException("Comment cannot be empty.");
            if (userId <= 0)
                throw new ArgumentException("UserId must be a positive integer.");
            if (taskId <= 0)
                throw new ArgumentException("TaskId must be a positive integer.");

            return new TaskComment
            {
                UserId = userId,
                TaskId = taskId,
                Comment = comment,
                CreatedAt = createdAt
            };
        }

        // ───────────── Internal rehydration‐constructor ─────────────
        internal TaskComment(
            int id,
            int userId,
            int taskId,
            string comment,
            DateTime createdAt)
        {
            Id = id;
            _userId = userId;
            _taskId = taskId;
            _comment = comment;
            _createdAt = createdAt;
        }

        protected TaskComment() { }

        // ───────────── Internal Rehydrate‐factory ─────────────
        internal static TaskComment Rehydrate(
            int id,
            int userId,
            int taskId,
            string comment,
            DateTime createdAt,
            User ownerUser,
            Task task)
        {
            TaskComment tc = new TaskComment(
                id,
                userId,
                taskId,
                comment,
                createdAt
            );

            if (ownerUser != null)
            {
                typeof(TaskComment)
                    .GetProperty(nameof(User))
                    .SetValue(tc, ownerUser);
            }

            if (task != null)
            {
                typeof(TaskComment)
                    .GetProperty(nameof(Task))
                    .SetValue(tc, task);
            }

            return tc;
        }
    }
}
