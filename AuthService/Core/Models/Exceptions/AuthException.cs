namespace AuthService.Core.Models.Exceptions;

public class AuthException : AppException
{
    public AuthException() : base("Something went wrong")
    {
    }
    public AuthException(string error) : base(error)
    {
    }
}
