using Application.Dtos.Improvements.Requests;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Infrastructure.Controllers;

[Route("api")]
[ApiController]
public class ImprovementController(IImprovementService improvementService) : Controller
{
    [HttpGet("[controller]s/Next")]
    [Authorize]
    public async Task<IActionResult> GetNextImprovements(Guid idUser, CancellationToken ct)
    {
        var result = await improvementService.GetNextImprovement(idUser, ct);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPost("[controller]/Buy")]
    [Authorize]
    public async Task<IActionResult> BuyImprovement(BuyImprovementRequest buyImprovementRequest, CancellationToken ct)
    {
        var result = await improvementService.BuyImprovement(buyImprovementRequest, ct);
        return StatusCode((int)result.StatusCode, result);
    }
}
