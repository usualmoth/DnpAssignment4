using ApiContracts.Users;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository userRepository;

    public UsersController(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> AddUser([FromBody] CreateUserDto request)
    {
        bool taken = userRepository.GetMany().Any(u => u.UserName == request.UserName);
        if (taken)
        {
            return Conflict($"Username '{request.UserName}' is already taken.");
        }

        User user = new User
        {
            UserName = request.UserName,
            Password = request.Password
        };

        User created = await userRepository.AddAsync(user);

        UserDto dto = MapToDto(created);
        return Created($"/users/{dto.UserId}", dto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateUser(int id, [FromBody] UpdateUserDto request)
    {
        try
        {
            User existing = await userRepository.GetSingleAsync(id);
            existing.UserName = request.UserName;

            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                existing.Password = request.Password;
            }

            await userRepository.UpdateAsync(existing);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUser(int id)
    {
        try
        {
            await userRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetSingle(int id)
    {
        try
        {
            User user = await userRepository.GetSingleAsync(id);
            return Ok(MapToDto(user));
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet]
    public ActionResult<IEnumerable<UserDto>> GetMany([FromQuery] string? userNameContains)
    {
        IQueryable<User> users = userRepository.GetMany();

        if (!string.IsNullOrWhiteSpace(userNameContains))
        {
            users = users.Where(u => u.UserName.Contains(userNameContains, StringComparison.OrdinalIgnoreCase));
        }

        List<UserDto> dtos = users.Select(MapToDto).ToList();
        return Ok(dtos);
    }

    private UserDto MapToDto(User user)
    {
        return new UserDto
        {
            UserId = user.UserId,
            UserName = user.UserName
        };
    }
}