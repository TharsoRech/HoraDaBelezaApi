using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace HoraDaBeleza.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiController : ControllerBase
{
    protected int UsuarioId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    protected string UsuarioTipo =>
        User.FindFirstValue(ClaimTypes.Role) ?? "";

    protected bool IsProprietario => UsuarioTipo == "Proprietario";
    protected bool IsAdmin        => UsuarioTipo == "Admin";
    protected bool IsProfissional => UsuarioTipo == "Profissional";
    protected bool IsCliente      => UsuarioTipo == "Cliente";
}
