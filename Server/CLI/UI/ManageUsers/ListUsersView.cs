using Entities;
using RepositoryContracts;

namespace CLI.UI.ManageUsers;

public class ListUsersView
{
    private readonly IUserRepository userRepository;

    public ListUsersView(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public void ListUsers()
    {
        Console.WriteLine();
        Console.WriteLine("--- All Users ---");

        foreach (User user in userRepository.GetMany())
        {
            Console.WriteLine($"[{user.UserId}] {user.UserName}");
        }
    }

    public void SearchUsersByName()
    {
        Console.Write("Enter part of username to search for: ");
        string search = Console.ReadLine() ?? "";

        var matches = userRepository.GetMany()
            .Where(u => u.UserName.Contains(search, StringComparison.OrdinalIgnoreCase));

        Console.WriteLine();
        Console.WriteLine("--- Matching Users ---");

        if (!matches.Any())
        {
            Console.WriteLine("No users found.");
            return;
        }

        foreach (User user in matches)
        {
            Console.WriteLine($"[{user.UserId}] {user.UserName}");
        }
    }
}