namespace GrajaSistemProject.Models.User;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime BirthDate { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Cpf { get; set; }
    public UserType Type { get; set; }
    public string State { get; set; }
    public string City { get; set; }
    public string Address { get; set; }
    public string Phone { get; set; }

    public enum UserType
    {
        Administrador,
        Farmer
    }
}