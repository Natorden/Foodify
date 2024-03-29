namespace RecipeService.Core.Models.Exceptions;

public class NotFoundException : AppException
{
    public NotFoundException() : base("Not found")
    {
    }
    public NotFoundException(string error) : base(error)
    {
    }
}
