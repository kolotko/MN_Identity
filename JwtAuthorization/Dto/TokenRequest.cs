namespace JwtAuthorization.Dto;

public record TokenRequest(string Token, string RefreshToken);