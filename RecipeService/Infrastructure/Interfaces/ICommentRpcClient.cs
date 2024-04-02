namespace RecipeService.Infrastructure.Interfaces;

public interface ICommentRpcClient {
    /// <summary>
    /// Returns the count of comments for a given recipe ID.
    /// </summary>
    /// <param name="recipeId">The ID of the recipe.</param>
    /// <returns>The amount of comments for the specified recipe ID.</returns>
    Task<int?> GetCommentCountByRecipeId(Guid recipeId);
    /// <summary>
    /// Returns the count of comments for a list of recipe IDs.
    /// </summary>
    /// <param name="ids">The list of recipe IDs.</param>
    /// <returns>A list of tuples, each containing a recipe ID and its corresponding comment count.</returns>
    Task<List<(Guid recipeId, int count)>> GetCommentCountsByRecipeIds(IEnumerable<Guid> ids);
}
