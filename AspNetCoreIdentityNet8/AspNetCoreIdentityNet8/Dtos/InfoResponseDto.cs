namespace AspNetCoreIdentityNet8.Dtos;

public record InfoResponseDto
{
    public string Email { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}