using HoraDaBeleza.Domain.Enums;

namespace HoraDaBeleza.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Base64Image { get; set; } // Imagem em base64
    public string? Doc { get; set; } // CPF/CNPJ
    public DateTime? Dob { get; set; } // Data de nascimento
    public UserType Type { get; set; }
    public bool Active { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
