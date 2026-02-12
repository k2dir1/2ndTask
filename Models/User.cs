namespace VH_2ND_TASK.Models;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty; //HASHED PASSWORD ICIN
}