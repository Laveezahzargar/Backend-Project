using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using backendProject.Types.Role;

namespace backendProject.Models.DomainModels
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; } = Guid.NewGuid();
        public string? Username { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public string? ProfilePicUrl { get; set; }
        public string? Phone { get; set; }
        public Role Role { get; set; } = Role.User;
        public ICollection<Address> Addresses { get; set; } = [];
        public Cart? Cart { get; set; }
        public ICollection<Order>? Orders { get; set; } = [];
    }
}