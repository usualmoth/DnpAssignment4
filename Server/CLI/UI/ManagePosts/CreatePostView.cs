using Entities;
using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class CreatePostView
{
    private readonly IPostRepository postRepository;
    private readonly IUserRepository userRepository;

    public CreatePostView(IPostRepository postRepository, IUserRepository userRepository)
    {
        this.postRepository = postRepository;
        this.userRepository = userRepository;
    }

    public async Task CreatePostAsync()
    {
        Console.WriteLine();
        Console.WriteLine("--- Create New Post ---");

        Console.Write("Enter title: ");
        string? title = Console.ReadLine();

        Console.Write("Enter body: ");
        string? body = Console.ReadLine();

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

        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(body))
        {
            Console.WriteLine("Title and body cannot be empty.");
            return;
        }

        Post newPost = new Post { Title = title, Body = body, UserId = userId };
        Post created = await postRepository.AddAsync(newPost);

        Console.WriteLine($"Post created with ID {created.PostId}.");
    }
}