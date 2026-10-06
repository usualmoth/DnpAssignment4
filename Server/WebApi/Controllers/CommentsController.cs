using ApiContracts.Comments;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentRepository commentRepository;
    private readonly IUserRepository userRepository;

    public CommentsController(ICommentRepository commentRepository, IUserRepository userRepository)
    {
        this.commentRepository = commentRepository;
        this.userRepository = userRepository;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CommentDto>> GetSingle(int id)
    {
        try
        {
            Comment comment = await commentRepository.GetSingleAsync(id);
            return Ok(MapToDto(comment));
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet]
    public ActionResult<IEnumerable<CommentDto>> GetMany(
        [FromQuery] int? userId,
        [FromQuery] string? userName,
        [FromQuery] int? postId)
    {
        IQueryable<Comment> comments = commentRepository.GetMany();

        if (userId is not null)
        {
            comments = comments.Where(c => c.UserId == userId);
        }

        if (!string.IsNullOrWhiteSpace(userName))
        {
            List<int> matchingUserIds = userRepository.GetMany()
                .Where(u => u.UserName.Contains(userName, StringComparison.OrdinalIgnoreCase))
                .Select(u => u.UserId)
                .ToList();

            comments = comments.Where(c => matchingUserIds.Contains(c.UserId));
        }

        if (postId is not null)
        {
            comments = comments.Where(c => c.PostId == postId);
        }

        List<CommentDto> dtos = comments.Select(MapToDto).ToList();
        return Ok(dtos);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteComment(int id)
    {
        try
        {
            await commentRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    private CommentDto MapToDto(Comment comment)
    {
        User? author = userRepository.GetMany().SingleOrDefault(u => u.UserId == comment.UserId);

        return new CommentDto
        {
            CommentId = comment.CommentId,
            Body = comment.Body,
            PostId = comment.PostId,
            UserId = comment.UserId,
            AuthorUserName = author?.UserName
        };
    }
}