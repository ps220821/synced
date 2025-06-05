using System;

namespace synced_DALL.Entities
{
    public enum Roles
    {
        admin,
        member,
        viewer
    }

    public class ProjectUsers
    {
        // ───────────── Backing fields ─────────────
        private int _projectId;
        private int _userId;

        // ───────────── Properties ─────────────
        public int Id { get; private set; }
        public User User { get; private set; }
        public Roles Role { get; private set; }
        public Project Project { get; private set; }



        protected ProjectUsers() { }

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

       

        public static ProjectUsers Create(
            int projectId,
            int userId,
            Roles role)
        {
            if (projectId <= 0)
                throw new ArgumentException("ProjectId must be a positive integer.");
            if (userId <= 0)
                throw new ArgumentException("UserId must be a positive integer.");

            return new ProjectUsers
            {
                ProjectId = projectId,
                UserId = userId,
                Role = role
            };
        }

        // ───────────── Internal rehydration‐constructor ─────────────
        internal ProjectUsers(
            int id,
            int projectId,
            int userId,
            Roles role)
        {
            Id = id;
            _projectId = projectId;
            _userId = userId;
            Role = role;
        }


        // ───────────── Internal Rehydrate‐factory ─────────────
        internal static ProjectUsers Rehydrate(
            int id,
            int projectId,
            int userId,
            Roles role,
            Project project,
            User user)
        {
            ProjectUsers pu = new ProjectUsers(
                id,
                projectId,
                userId,
                role
            );

            if (project != null)
            {
                typeof(ProjectUsers)
                    .GetProperty(nameof(Project))
                    .SetValue(pu, project);
            }

            if (user != null)
            {
                typeof(ProjectUsers)
                    .GetProperty(nameof(User))
                    .SetValue(pu, user);
            }

            return pu;
        }
    }
}
