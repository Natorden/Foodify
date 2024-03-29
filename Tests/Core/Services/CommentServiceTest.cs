using CommentService.Core.Models.BindingModels;
using CommentService.Infrastructure.Interfaces;
using Moq;
using Xunit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommentService.Core.Models.Entities;
using JetBrains.Annotations;
using Shared.Models;

namespace Tests.Core.Services;

[TestSubject(typeof(CommentService.Core.Services.CommentService))]
public class CommentServiceTest {
    private readonly Mock<ICommentRepository> _commentRepoMock = new();
    private readonly Mock<IProfileRpcClient> _profileClientMock = new();

    [Fact]
    public async Task GetCommentsByRecipeId_Test()
    {
        // Arrange
        var recipeId = Guid.NewGuid();
        var comments = new List<Comment> {
            new() {
                UserId = Guid.NewGuid(),
                Content = "test"
            }
        };
        var userProfile = new SharedUserProfileDto {
            Id = comments[0].UserId,
            UserName = "testuser"
        };
        _commentRepoMock.Setup(x => x.GetCommentsByRecipeId(recipeId)).ReturnsAsync(comments);
        _profileClientMock.Setup(x => x.GetUserProfilesByIds(new List<Guid> {
            comments[0].UserId
        })).ReturnsAsync(new List<SharedUserProfileDto> {
            userProfile
        });

        // Act
        var service = new CommentService.Core.Services.CommentService(_commentRepoMock.Object, _profileClientMock.Object);
        var result = await service.GetCommentsByRecipeId(recipeId);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(comments[0].UserId, result[0].UserId);
        Assert.Equal(userProfile.Id, result[0].User.Id);
    }

    [Fact]
    public async Task GetCommentById_Test()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        var comment = new Comment {
            UserId = Guid.NewGuid(),
            Content = "test"
        };
        var userProfile = new SharedUserProfileDto {
            Id = comment.UserId,
            UserName = "testuser"
        };
        _commentRepoMock.Setup(x => x.GetCommentById(commentId)).ReturnsAsync(comment);
        _profileClientMock.Setup(x => x.GetUserProfileById(comment.UserId)).ReturnsAsync(userProfile);

        // Act
        var service = new CommentService.Core.Services.CommentService(_commentRepoMock.Object, _profileClientMock.Object);
        var result = await service.GetCommentById(commentId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(comment.UserId, result.UserId);
        Assert.Equal(userProfile.Id, result.User.Id);
    }

    [Fact]
    public async Task CreateComment_Test()
    {
        // Arrange
        var model = new CommentPostBindingModel {
            Content = "test"
        };
        var commentId = Guid.NewGuid();
        var comment = new Comment {
            UserId = Guid.NewGuid(),
            Content = "test"
        };
        _commentRepoMock.Setup(x => x.CreateComment(model)).ReturnsAsync(commentId);
        _commentRepoMock.Setup(x => x.GetCommentById(commentId)).ReturnsAsync(comment);

        // Act
        var service = new CommentService.Core.Services.CommentService(_commentRepoMock.Object, _profileClientMock.Object);
        var result = await service.CreateComment(model);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(comment.UserId, result.UserId);
    }
}