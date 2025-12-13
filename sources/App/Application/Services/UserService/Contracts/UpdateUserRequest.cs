using Domain.Entities;

namespace Application.Services.UserService.Models
{
    public class UpdateUserRequest
    {
        public Guid Id { get; set; }
        public string Email { get; set; }

        public string Phone { get; set; }

        public UserRole? Role { get; set; }
    }
}
