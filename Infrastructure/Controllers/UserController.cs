using Application.Dtos.Users;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Infrastructure.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService userService) : Controller
{
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var response = await userService.GetDetailUser(id, ct);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpGet("[action]")]
    [Authorize]
    public async Task<IActionResult> GetList(CancellationToken ct)
    {
        var response = await userService.GetListUsers(ct);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpPost]
    public async Task<IActionResult> Registration(CreateUserRequest createUserRequest, CancellationToken ct)
    {
        var response = await userService.RegistrationUser(createUserRequest, ct);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> LogIn(long idTelegram, CancellationToken ct)
    {
        var response = await userService.LogIn(idTelegram, ct);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpPatch("[action]")]
    [Authorize]
    public async Task<IActionResult> Click(Guid id, CancellationToken ct)
    {
        var response = await userService.Click(id, ct);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpGet("RangTapRange")]
    [Authorize]
    public async Task<IActionResult> GetRangeRangTap(Guid idUser, CancellationToken ct)
    { 
        var response = await userService.GetRangeRang(idUser, ct);
        return StatusCode((int)response.StatusCode, response);
    }
    
    [HttpGet("Balance")]
    [Authorize]
    public async Task<IActionResult> GetBalance(Guid idUser, CancellationToken ct)
    { 
        var response = await userService.GetBalanceUser(idUser, ct);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpGet("ReferalLink")]
    [Authorize]
    public async Task<IActionResult> GetReferalLink(Guid idUser, CancellationToken ct)
    {
        var response = await userService.GetReferalLink(idUser, ct);
        return StatusCode((int)response.StatusCode, response);
    }
    
}
