using System;

namespace synced_DALL.Entities
{
    public class User
    {
        // ───────────── Backing fields ─────────────
        private string _email;
        private string _password;

        // ───────────── Properties ─────────────
        public int Id { get; private set; }
        public string Username { get; private set; }
        public string Firstname { get; private set; }
        public string Lastname { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public string Email
        {
            get => _email;
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Email cannot be empty.");
                _email = value.Trim();
            }
        }

        public string Password
        {
            get => _password;
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Password cannot be empty.");
                _password = value;
            }
        }

        // ───────────── Factory ─────────────
        public static User Create(
            string username,
            string firstname,
            string lastname,
            string email,
            string password)
        {
            if (email == null)
                throw new ArgumentException("Email cannot be null.");
            if (password == null)
                throw new ArgumentException("Password cannot be null.");
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty.");
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty.");

            return new User
            {
                Username = username?.Trim(),
                Firstname = firstname?.Trim(),
                Lastname = lastname?.Trim(),
                Email = email,
                Password = password,
                CreatedAt = DateTime.UtcNow
            };
        }

        // ───────────── Internal rehydration constructor ─────────────
        internal User(
            int id,
            string username,
            string firstname,
            string lastname,
            string email,
            string password,
            DateTime createdAt)
        {
            Id = id;
            Username = username;
            Firstname = firstname;
            Lastname = lastname;
            _email = email;
            _password = password;
            CreatedAt = createdAt;
        }

        // Parameterloze constructor voor ORM / mapping‐tools
        protected User() { }

        internal static User Rehydrate(
           int id,
           string username,
           string firstname,
           string lastname,
           string email,
           string password,
           DateTime createdAt)
        {
            var u = new User
            {
                _email = email,
                _password = password,
                Username = username,
                Firstname = firstname,
                Lastname = lastname,
                CreatedAt = createdAt
            };
            u.Id = id;
            return u;
        }

        public bool VerifyPassword(string attempt)
        {
            if (attempt == null) return false;
            return attempt == Password;
        }
    }
}
