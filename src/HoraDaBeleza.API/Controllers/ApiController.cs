using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace HoraDaBeleza.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiController : ControllerBase
{
    protected int UserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    protected string UserType =>
        User.FindFirstValue(ClaimTypes.Role) ?? "";

    protected bool IsOwner        => UserType == "Owner";
    protected bool IsAdmin         => UserType == "Admin";
    protected bool IsProfessional  => UserType == "Professional";
    protected bool IsClient        => UserType == "Client";
}
