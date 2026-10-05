using Entities;
using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class ListPostsView
{
    private readonly IPostRepository postRepository;

    public ListPostsView(IPostRepository postRepository)
    {
        this.postRepository = postRepository;
    }

    public void ListPosts()
    {
        Console.WriteLine();
        Console.WriteLine("--- Posts Overview ---");

        foreach (Post post in postRepository.GetMany())
        {
            Console.WriteLine($"[{post.PostId}] {post.Title}");
        }
    }

    public void ListPostsByUser()
    {
        Console.Write("Enter user ID: ");
        if (!int.TryParse(Console.ReadLine(), out int userId))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        var posts = postRepository.GetMany().Where(p => p.UserId == userId);

        Console.WriteLine();
        Console.WriteLine($"--- Posts by User {userId} ---");

        if (!posts.Any())
        {
            Console.WriteLine("No posts found.");
            return;
        }

        foreach (Post post in posts)
        {
            Console.WriteLine($"[{post.PostId}] {post.Title}");
        }
    }
}