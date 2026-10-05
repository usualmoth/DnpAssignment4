using Entities;
using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class ManagePostsView
{
    private readonly IPostRepository postRepository;
    private readonly CreatePostView createPostView;
    private readonly ListPostsView listPostsView;
    private readonly SinglePostView singlePostView;

    public ManagePostsView(IPostRepository postRepository, IUserRepository userRepository, ICommentRepository commentRepository)
    {
        this.postRepository = postRepository;
        createPostView = new CreatePostView(postRepository, userRepository);
        listPostsView = new ListPostsView(postRepository);
        singlePostView = new SinglePostView(postRepository, commentRepository);
    }

    public async Task ShowAsync()
    {
        bool back = false;
        while (!back)
        {
            Console.WriteLine();
            Console.WriteLine("--- Manage Posts ---");
            Console.WriteLine("1. Create new post");
            Console.WriteLine("2. Update post");
            Console.WriteLine("3. Delete post");
            Console.WriteLine("4. View posts overview");
            Console.WriteLine("5. View specific post");
            Console.WriteLine("6. View posts by user ID");
            Console.WriteLine("0. Back");
            Console.Write("Choose an option: ");

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1": await createPostView.CreatePostAsync(); break;
                case "2": await UpdatePostAsync(); break;
                case "3": await DeletePostAsync(); break;
                case "4": listPostsView.ListPosts(); break;
                case "5": await singlePostView.ShowPostAsync(); break;
                case "6": listPostsView.ListPostsByUser(); break;
                case "0": back = true; break;
                default: Console.WriteLine("Invalid option."); break;
            }
        }
    }

    private async Task UpdatePostAsync()
    {
        Console.Write("Enter ID of post to update: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        try
        {
            Post existing = await postRepository.GetSingleAsync(id);

            Console.Write($"Enter new title (leave blank to keep '{existing.Title}'): ");
            string? newTitle = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newTitle))
            {
                existing.Title = newTitle;
            }

            Console.Write("Enter new body (leave blank to keep current): ");
            string? newBody = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newBody))
            {
                existing.Body = newBody;
            }

            await postRepository.UpdateAsync(existing);
            Console.WriteLine("Post updated.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private async Task DeletePostAsync()
    {
        Console.Write("Enter ID of post to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        try
        {
            await postRepository.DeleteAsync(id);
            Console.WriteLine("Post deleted.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}