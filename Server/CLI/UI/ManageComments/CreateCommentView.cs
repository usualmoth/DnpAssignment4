using Entities;
using RepositoryContracts;

namespace CLI.UI.ManageComments;

public class CreateCommentView
{
    private readonly ICommentRepository commentRepository;
    private readonly IPostRepository postRepository;
    private readonly IUserRepository userRepository;

    public CreateCommentView(ICommentRepository commentRepository, IPostRepository postRepository, IUserRepository userRepository)
    {
        this.commentRepository = commentRepository;
        this.postRepository = postRepository;
        this.userRepository = userRepository;
    }

    public async Task CreateCommentAsync()
    {
        Console.WriteLine();
        Console.WriteLine("--- Add Comment ---");

        Console.Write("Enter post ID: ");
        if (!int.TryParse(Console.ReadLine(), out int postId))
        {
            Console.WriteLine("Invalid post ID.");
            return;
        }

        bool postExists = postRepository.GetMany().Any(p => p.PostId == postId);
        if (!postExists)
        {
            Console.WriteLine($"No post found with ID {postId}.");
            return;
        }

        Console.Write("Enter your user ID: ");
        if (!int.TryParse(Console.ReadLine(), out int userId))
        {
            Console.WriteLine("Invalid user ID.");
            return;
        }

        bool userExists = userRepository.GetMany().Any(u => u.UserId == userId);
        if (!userExists)
        {
            Console.WriteLine($"No user found with ID {userId}.");
            return;
        }

        Console.Write("Enter comment body: ");
        string? body = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(body))
        {
            Console.WriteLine("Comment cannot be empty.");
            return;
        }

        Comment newComment = new Comment { Body = body, PostId = postId, UserId = userId };
        Comment created = await commentRepository.AddAsync(newComment);

        Console.WriteLine($"Comment added with ID {created.CommentId}.");
    }
}