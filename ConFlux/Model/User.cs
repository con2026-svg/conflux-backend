namespace ConFlux.Model
{
    namespace YourApp.Models
    {
        public class User
        {
            public Guid Id { get; set; }
            public string Email { get; set; } = null!;
            public byte[] PasswordHash { get; set; } = null!;
            public byte[] PasswordSalt { get; set; } = null!;
            public bool IsActive { get; set; }
            public DateTime CreatedAt { get; set; }

            public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        }
    }

}
