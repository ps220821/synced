namespace synced_DALL.Entities
{
    public class Project
    {
        // ───────────── Backing fields ─────────────
        private string _name;
        private string _description;
        private DateOnly _startDate;
        private DateOnly _endDate;
        private int _owner;

        // ───────────── Properties ─────────────
        public int Id { get; private set; }
        public User User { get; private set; }

        public string Name
        {
            get => _name;
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Name cannot be empty.");
                _name = value.Trim();
            }
        }
        public string Description
        {
            get => _description;
            private set => _description = value?.Trim();
        }

        public DateOnly Start_Date
        {
            get => _startDate;
            private set
            {
                _startDate = value;
                if (_endDate != default && _startDate > _endDate)
                    throw new ArgumentException("Start_Date cannot be after End_Date.");
            }
        }
        public DateOnly End_Date
        {
            get => _endDate;
            private set
            {
                _endDate = value;
                if (_startDate != default && _startDate > _endDate)
                    throw new ArgumentException("End_Date cannot be before Start_Date.");
            }
        }
        public int Owner
        {
            get => _owner;
            private set
            {
                if (value <= 0)
                    throw new ArgumentException("Owner must be a positive user ID.");
                _owner = value;
            }
        }


        // mapper
        public static Project Create(
            string name,
            string description,
            DateOnly startDate,
            DateOnly endDate,
            int ownerUserId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty.");
            if (ownerUserId <= 0)
                throw new ArgumentException("Owner must be a positive user ID.");
            if (startDate > endDate)
                throw new ArgumentException("Start_Date cannot be after End_Date.");

            return new Project
            {
                Name = name,
                Description = description,
                Start_Date = startDate,
                End_Date = endDate,
                Owner = ownerUserId
            };
        }

        internal Project(
            int id,
            string name,
            string description,
            DateOnly startDate,
            DateOnly endDate,
            int ownerUserId)
        {
            Id = id;
            _name = name;
            _description = description;
            _startDate = startDate;
            _endDate = endDate;
            _owner = ownerUserId;
        }

        protected Project() { }

        // ───────────── Internal rehydrate‐factory ─────────────
        internal static Project Rehydrate(
            int id,
            string name,
            string description,
            DateOnly startDate,
            DateOnly endDate,
            int ownerUserId,
            User ownerUser)
        {
            Project p = new Project(
                id,
                name,
                description,
                startDate,
                endDate,
                ownerUserId
            );
            typeof(Project)
                .GetProperty(nameof(Project.User))
                .SetValue(p, ownerUser);

            return p;
        }

    }
}
