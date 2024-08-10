namespace Application.DTOs.Responces;

public class LoginResponse
{
    public string Token { get; set; }
    public DateTime ExpireDate { get; set; }
}
