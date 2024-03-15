namespace CommentService.Core.Models.Exceptions;

public class BadRequestException : AppException
{
    public BadRequestException() : base("Something went wrong")
    {
    }
    public BadRequestException(string error) : base(error)
    {
    }
}
