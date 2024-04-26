namespace GymTracker.Infrastructure.Common;

public class AuthResponseDto
{
    public Guid UserId { get; set; }
    public string Token { get; set; }
}