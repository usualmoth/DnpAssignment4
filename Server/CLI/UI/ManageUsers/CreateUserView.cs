using Entities;
using RepositoryContracts;

namespace CLI.UI.ManageUsers;

public class CreateUserView
{
    private readonly IUserRepository userRepository;

    public CreateUserView(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task CreateUserAsync()
    {
        Console.WriteLine();
        Console.WriteLine("--- Create New User ---");

        Console.Write("Enter username: ");
        string? userName = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(userName))
        {
            Console.WriteLine("Username cannot be empty.");
            return;
        }

        bool taken = userRepository.GetMany().Any(u => u.UserName == userName);
        if (taken)
        {
            Console.WriteLine("Username is already taken.");
            return;
        }

        Console.Write("Enter password: ");
        string? password = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(password))
        {
            Console.WriteLine("Password cannot be empty.");
            return;
        }

        User newUser = new User { UserName = userName, Password = password };
        User created = await userRepository.AddAsync(newUser);

        Console.WriteLine($"User created with ID {created.UserId}.");
    }
}