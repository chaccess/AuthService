using Application.Common.Compiled;
using Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Application.Services.UserService.Models;

public class CreateUserRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [Phone]
    public string Phone { get; set; }

    public UserRole? Role { get; set; } = UserRole.User;
}
