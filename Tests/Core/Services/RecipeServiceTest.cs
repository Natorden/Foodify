using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Xunit;
using Moq;
using RecipeService.Core.Models.Dtos;
using RecipeService.Infrastructure.Interfaces;

namespace Tests.Core.Services {
    [TestSubject(typeof(RecipeService.Core.Services.RecipeService))]
    public class RecipeServiceTest {
        private readonly RecipeService.Core.Services.RecipeService _recipeService;
        private readonly Mock<IRecipeRepository> _recipeRepositoryMock;
        private readonly Mock<IProfileRpcClient> _profileRpcClientMock;
        private readonly Mock<ICommentRpcClient> _commentRpcClientMock;

        private RecipeServiceTest()
        {
            _recipeRepositoryMock = new Mock<IRecipeRepository>();
            _profileRpcClientMock = new Mock<IProfileRpcClient>();
            _commentRpcClientMock = new Mock<ICommentRpcClient>();
            _recipeService = new RecipeService.Core.Services.RecipeService(_recipeRepositoryMock.Object, _profileRpcClientMock.Object, _commentRpcClientMock.Object);
        }

        [Fact]
        public async Task GetRecipeDtoById_ValidGuid_ReturnsRecipeDto()
        {
            //Arrange
            var testRecipeId = Guid.NewGuid();
            var testRecipeDto = new RecipeDto {
                Title = "Test recipe",
                Info = "test description"
            };
            _recipeRepositoryMock.Setup(rr => rr.GetRecipeDtoById(testRecipeId)).ReturnsAsync(testRecipeDto);

            //Act
            var result = await _recipeService.GetRecipeDtoById(testRecipeId);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(testRecipeDto, result);
        }

        // ... Add more tests for other methods and edge cases here...

    }
}