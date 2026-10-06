namespace ApiContracts.Comments;

public class CommentDto
{
    public int CommentId { get; set; }
    public string Body { get; set; } 
    public int PostId { get; set; }
    public int UserId { get; set; }
    public string? AuthorUserName { get; set; }
}