namespace RecipeService.Core.Models.Exceptions;

public abstract class AppException(string error) : System.Exception(error);
