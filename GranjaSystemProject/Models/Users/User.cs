using System.ComponentModel.DataAnnotations;

namespace GranjaSystemProject.Models.Users;
public class User
{
    [Key]
    public int Id { get; set; }
    [Required, MaxLength(100)]
    public string Name { get; set; }
    [Required]
    public DateTime BirthDate { get; set; }
    [Required, EmailAddress, MaxLength(100)]
    public string Email { get; set; }
    [Required]
    public string PasswordHash { get; set; } 
    [MaxLength(14)]
    public string? Cpf { get; set; }
    [Required]
    public UserType Type { get; set; }
    [MaxLength(2)]
    public string? State { get; set; }
    [MaxLength(60)]
    public string? City { get; set; }
    public string? Address { get; set; }
    [MaxLength(20)]
    public string? Phone { get; set; }
    public int FailedLoginAttempts { get; set; } = 0;
    public DateTime? LockoutEnd { get; set; }
}
public enum UserType
{
    Administrador,
    Farmer
}
