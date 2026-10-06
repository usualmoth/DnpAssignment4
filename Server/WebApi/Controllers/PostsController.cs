using ApiContracts.Comments;
using ApiContracts.Posts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostRepository postRepository;
    private readonly IUserRepository userRepository;
    private readonly ICommentRepository commentRepository;

    public PostsController(IPostRepository postRepository, IUserRepository userRepository, ICommentRepository commentRepository)
    {
        this.postRepository = postRepository;
        this.userRepository = userRepository;
        this.commentRepository = commentRepository;
    }

    [HttpPost]
    public async Task<ActionResult<PostDto>> AddPost([FromBody] CreatePostDto request)
    {
        bool userExists = userRepository.GetMany().Any(u => u.UserId == request.UserId);
        if (!userExists)
        {
            return BadRequest($"No user found with ID {request.UserId}.");
        }

        Post post = new Post
        {
            Title = request.Title,
            Body = request.Body,
            UserId = request.UserId
        };

        Post created = await postRepository.AddAsync(post);

        PostDto dto = MapToDto(created, includeComments: false);
        return Created($"/posts/{dto.PostId}", dto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdatePost(int id, [FromBody] UpdatePostDto request)
    {
        try
        {
            Post existing = await postRepository.GetSingleAsync(id);
            existing.Title = request.Title;
            existing.Body = request.Body;

            await postRepository.UpdateAsync(existing);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeletePost(int id)
    {
        try
        {
            await postRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PostDto>> GetSingle(int id, [FromQuery] bool includeComments = false)
    {
        try
        {
            Post post = await postRepository.GetSingleAsync(id);
            return Ok(MapToDto(post, includeComments));
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet]
    public ActionResult<IEnumerable<PostDto>> GetMany(
        [FromQuery] string? titleContains,
        [FromQuery] int? userId,
        [FromQuery] string? userName)
    {
        IQueryable<Post> posts = postRepository.GetMany();

        if (!string.IsNullOrWhiteSpace(titleContains))
        {
            posts = posts.Where(p => p.Title.Contains(titleContains, StringComparison.OrdinalIgnoreCase));
        }

        if (userId is not null)
        {
            posts = posts.Where(p => p.UserId == userId);
        }

        if (!string.IsNullOrWhiteSpace(userName))
        {
            List<int> matchingUserIds = userRepository.GetMany()
                .Where(u => u.UserName.Contains(userName, StringComparison.OrdinalIgnoreCase))
                .Select(u => u.UserId)
                .ToList();

            posts = posts.Where(p => matchingUserIds.Contains(p.UserId));
        }

        List<PostDto> dtos = posts.Select(p => MapToDto(p, includeComments: false)).ToList();
        return Ok(dtos);
    }

    [HttpGet("{postId}/comments")]
    public ActionResult<IEnumerable<CommentDto>> GetCommentsForPost(int postId)
    {
        bool postExists = postRepository.GetMany().Any(p => p.PostId == postId);
        if (!postExists)
        {
            return NotFound($"Post with ID '{postId}' not found");
        }

        List<CommentDto> dtos = commentRepository.GetMany()
            .Where(c => c.PostId == postId)
            .Select(MapCommentToDto)
            .ToList();

        return Ok(dtos);
    }

    [HttpPost("{postId}/comments")]
    public async Task<ActionResult<CommentDto>> AddCommentToPost(int postId, [FromBody] CreateCommentDto request)
    {
        bool postExists = postRepository.GetMany().Any(p => p.PostId == postId);
        if (!postExists)
        {
            return NotFound($"Post with ID '{postId}' not found");
        }

        bool userExists = userRepository.GetMany().Any(u => u.UserId == request.UserId);
        if (!userExists)
        {
            return BadRequest($"No user found with ID {request.UserId}.");
        }

        Comment comment = new Comment
        {
            Body = request.Body,
            PostId = postId,
            UserId = request.UserId
        };

        Comment created = await commentRepository.AddAsync(comment);
        return Created($"/comments/{created.CommentId}", MapCommentToDto(created));
    }

    private PostDto MapToDto(Post post, bool includeComments)
    {
        User? author = userRepository.GetMany().SingleOrDefault(u => u.UserId == post.UserId);

        PostDto dto = new PostDto
        {
            PostId = post.PostId,
            Title = post.Title,
            Body = post.Body,
            UserId = post.UserId,
            AuthorUserName = author?.UserName
        };

        if (includeComments)
        {
            dto.Comments = commentRepository.GetMany()
                .Where(c => c.PostId == post.PostId)
                .Select(MapCommentToDto)
                .ToList();
        }

        return dto;
    }

    private CommentDto MapCommentToDto(Comment comment)
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