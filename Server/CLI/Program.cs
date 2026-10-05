using CLI.UI;
using FileRepositories;
using RepositoryContracts;

Console.WriteLine("Starting CLI Application...");

IUserRepository userRepository = new UserFileRepository();
ICommentRepository commentRepository = new CommentFileRepository();
IPostRepository postRepository = new PostFileRepository();

CliApp cliApp = new CliApp(userRepository, commentRepository, postRepository);
await cliApp.StartAsync();

Console.WriteLine("Press any key to exit...");
Console.ReadKey();