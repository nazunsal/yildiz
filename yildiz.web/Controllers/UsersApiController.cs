using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using yildiz.DTO;
using yildiz.business.Abstract;

namespace yildiz.web.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = "Admin")]
public class UsersApiController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersApiController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{username}")]
    public async Task<IActionResult> GetByUsername(string username)
    {
        var user = await _userService.GetByUsernameAsync(username);

        if (user == null)
        {
            return NotFound();
        }

        var result = new UserDto
        {
            UserId = user.UserId,
            Name = user.Name,
            Username = user.Username,
            Email = user.Email,
            IsAdmin = user.IsAdmin
        };

        return Ok(result);
    }
}