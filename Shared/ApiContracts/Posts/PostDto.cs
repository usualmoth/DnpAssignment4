using ApiContracts.Comments;

namespace ApiContracts.Posts;

public class PostDto
{
    public int PostId { get; set; }
    public string Title { get; set; } 
    public string Body { get; set; } 
    public int UserId { get; set; }
    public string? AuthorUserName { get; set; }
    public List<CommentDto>? Comments { get; set; }
}