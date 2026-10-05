using CLI.UI.ManageUsers;
using CLI.UI.ManagePosts;
using CLI.UI.ManageComments;
using RepositoryContracts;

namespace CLI.UI;

public class  CliApp
{
    private readonly ManageUsersView manageUsersView;
    private readonly ManagePostsView managePostsView;
    private readonly ManageCommentView manageCommentView;

    public CliApp(IUserRepository userRepository, ICommentRepository commentRepository, IPostRepository postRepository)
    {
        manageUsersView = new ManageUsersView(userRepository);
        managePostsView = new ManagePostsView(postRepository, userRepository, commentRepository);
        manageCommentView = new ManageCommentView(commentRepository, postRepository, userRepository);
    }

    public async Task StartAsync()
    {
        bool exit = false;
        while (!exit)
        {
            Console.WriteLine();
            Console.WriteLine("=== Main Menu ===");
            Console.WriteLine("1. Manage Users");
            Console.WriteLine("2. Manage Posts");
            Console.WriteLine("3. Manage Comments");
            Console.WriteLine("0. Exit");
            Console.Write("Choose an option: ");

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1": await manageUsersView.ShowAsync(); break;
                case "2": await managePostsView.ShowAsync(); break;
                case "3": await manageCommentView.ShowAsync(); break;
                case "0": exit = true; break;
                default: Console.WriteLine("Invalid option, try again."); break;
            }
        }
    }
}