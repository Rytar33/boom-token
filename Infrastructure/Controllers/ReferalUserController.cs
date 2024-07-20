using Application.Dtos.ReferalsUsers.Requests;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Infrastructure.Controllers;

[ApiController]
[Route("api")]
public class ReferalUserController(IReferalUsersService referalUsersService) : Controller
{
    [HttpPatch("[controller]/Reward")]
    [Authorize]
    public async Task<IActionResult> GetReward(GetRewardRequest rewardRequest, CancellationToken ct)
    {
        var result = await referalUsersService.GetReward(rewardRequest, ct);
        return StatusCode((int)result.StatusCode, result);
    }
    
    [HttpGet("[controller]s")]
    [Authorize]
    public async Task<IActionResult> GetReferals(Guid idUser, CancellationToken ct)
    {
        var result = await referalUsersService.GetYourReferals(idUser, ct);
        return StatusCode((int)result.StatusCode, result);
    }
}