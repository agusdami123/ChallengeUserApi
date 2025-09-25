using System.ComponentModel.DataAnnotations;
using Demo.Domain;

namespace Demo.Contracts;

public record UsuarioDto(
    int Id,
    string Descripcion,
    TipoUsuario Tipo,
    string CorreoElectronico,
    string Telefono,
    bool Activo
);

public class UsuarioCreateUpdateDto
{
    [Required, StringLength(200)]
    public string Descripcion { get; set; } = default!;

    [Required]
    public TipoUsuario Tipo { get; set; }

    [Required, StringLength(200)]
    [RegularExpression(@"^[^\s@]+@[^\s@]+\.(com|[A-Za-z]{2,})$",
        ErrorMessage = "Correo inválido (debe tener @ y terminar con .com o un TLD válido).")]
    public string CorreoElectronico { get; set; } = default!;

    [Required, RegularExpression(@"^\d{7,15}$",
        ErrorMessage = "El teléfono debe tener solo dígitos (7 a 15).")]
    public string Telefono { get; set; } = default!;

    public bool Activo { get; set; }
}
