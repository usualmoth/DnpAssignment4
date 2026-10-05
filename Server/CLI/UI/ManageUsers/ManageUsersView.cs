using Entities;
using RepositoryContracts;

namespace CLI.UI.ManageUsers;

public class ManageUsersView
{
    private readonly IUserRepository userRepository;
    private readonly CreateUserView createUserView;
    private readonly ListUsersView listUsersView;

    public ManageUsersView(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
        createUserView = new CreateUserView(userRepository);
        listUsersView = new ListUsersView(userRepository);
    }

    public async Task ShowAsync()
    {
        bool back = false;
        while (!back)
        {
            Console.WriteLine();
            Console.WriteLine("--- Manage Users ---");
            Console.WriteLine("1. Create new user");
            Console.WriteLine("2. Update user");
            Console.WriteLine("3. Delete user");
            Console.WriteLine("4. View all users");
            Console.WriteLine("5. Search users by username");
            Console.WriteLine("0. Back");
            Console.Write("Choose an option: ");

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1": await createUserView.CreateUserAsync(); break;
                case "2": await UpdateUserAsync(); break;
                case "3": await DeleteUserAsync(); break;
                case "4": listUsersView.ListUsers(); break;
                case "5": listUsersView.SearchUsersByName(); break;
                case "0": back = true; break;
                default: Console.WriteLine("Invalid option."); break;
            }
        }
    }

    private async Task UpdateUserAsync()
    {
        Console.Write("Enter ID of user to update: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        try
        {
            User existing = await userRepository.GetSingleAsync(id);

            Console.Write($"Enter new username (leave blank to keep '{existing.UserName}'): ");
            string? newUserName = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newUserName))
            {
                existing.UserName = newUserName;
            }

            Console.Write("Enter new password (leave blank to keep current): ");
            string? newPassword = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newPassword))
            {
                existing.Password = newPassword;
            }

            await userRepository.UpdateAsync(existing);
            Console.WriteLine("User updated.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private async Task DeleteUserAsync()
    {
        Console.Write("Enter ID of user to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        try
        {
            await userRepository.DeleteAsync(id);
            Console.WriteLine("User deleted.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}