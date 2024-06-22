namespace AspNetCoreIdentityNet8.Dtos;

public record CreateUserDto(
    string Email,
    string Password,
    string FirstName,
    string LastName
);