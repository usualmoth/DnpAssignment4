using Entities;
using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class SinglePostView
{
    private readonly IPostRepository postRepository;
    private readonly ICommentRepository commentRepository;

    public SinglePostView(IPostRepository postRepository, ICommentRepository commentRepository)
    {
        this.postRepository = postRepository;
        this.commentRepository = commentRepository;
    }

    public async Task ShowPostAsync()
    {
        Console.Write("Enter post ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        try
        {
            Post post = await postRepository.GetSingleAsync(id);

            Console.WriteLine();
            Console.WriteLine($"Title: {post.Title}");
            Console.WriteLine($"Body: {post.Body}");
            Console.WriteLine();
            Console.WriteLine("--- Comments ---");

            var comments = commentRepository.GetMany().Where(c => c.PostId == id);

            if (!comments.Any())
            {
                Console.WriteLine("No comments yet.");
            }
            else
            {
                foreach (Comment comment in comments)
                {
                    Console.WriteLine($"- {comment.Body} (User {comment.UserId})");
                }
            }
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}