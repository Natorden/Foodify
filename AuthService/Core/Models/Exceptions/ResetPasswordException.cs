namespace AuthService.Core.Models.Exceptions;

public class ResetPasswordException : AppException
{
    public ResetPasswordException(string error) : base(error)
    {
    }
}
