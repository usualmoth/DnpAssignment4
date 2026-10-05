using Entities;
using RepositoryContracts;

namespace CLI.UI.ManageComments;

public class ManageCommentView
{
    private readonly ICommentRepository commentRepository;
    private readonly CreateCommentView createCommentView;

    public ManageCommentView(ICommentRepository commentRepository, IPostRepository postRepository, IUserRepository userRepository)
    {
        this.commentRepository = commentRepository;
        createCommentView = new CreateCommentView(commentRepository, postRepository, userRepository);
    }

    public async Task ShowAsync()
    {
        bool back = false;
        while (!back)
        {
            Console.WriteLine();
            Console.WriteLine("--- Manage Comments ---");
            Console.WriteLine("1. Add comment to post");
            Console.WriteLine("2. Delete comment");
            Console.WriteLine("3. View comments by user ID");
            Console.WriteLine("0. Back");
            Console.Write("Choose an option: ");

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1": await createCommentView.CreateCommentAsync(); break;
                case "2": await DeleteCommentAsync(); break;
                case "3": ViewCommentsByUser(); break;
                case "0": back = true; break;
                default: Console.WriteLine("Invalid option."); break;
            }
        }
    }

    private async Task DeleteCommentAsync()
    {
        Console.Write("Enter ID of comment to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        try
        {
            await commentRepository.DeleteAsync(id);
            Console.WriteLine("Comment deleted.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private void ViewCommentsByUser()
    {
        Console.Write("Enter user ID: ");
        if (!int.TryParse(Console.ReadLine(), out int userId))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        var comments = commentRepository.GetMany().Where(c => c.UserId == userId);

        Console.WriteLine();
        Console.WriteLine($"--- Comments by User {userId} ---");

        if (!comments.Any())
        {
            Console.WriteLine("No comments found.");
            return;
        }

        foreach (Comment comment in comments)
        {
            Console.WriteLine($"[{comment.CommentId}] on Post {comment.PostId}: {comment.Body}");
        }
    }
}