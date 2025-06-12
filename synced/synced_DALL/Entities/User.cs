using Microsoft.AspNetCore.Identity;
using System;

namespace synced_DALL.Entities
{
    public class User
    {
        // ───────────── Backing fields ─────────────
        private string _email;
        private string _password;
        private string _passwordHash;

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

        public string PasswordHash
        {
            get => _passwordHash;
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Password hash cannot be empty.");
                _passwordHash = value;
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
                PasswordHash = HashPassword(password),
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
            string passwordHash,
            DateTime createdAt)
        {
            Id = id;
            Username = username;
            Firstname = firstname;
            Lastname = lastname;
            _email = email;
            _passwordHash = passwordHash;
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
           string passwordHash,
           DateTime createdAt)
        {
            User u = new User
            {
                _email = email,
                _passwordHash = passwordHash,
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
            return HashPassword(attempt) == PasswordHash;
        }
        private static string HashPassword(string password)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
