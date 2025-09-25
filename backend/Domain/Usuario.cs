namespace Demo.Domain;

public enum TipoUsuario
{
    Administrador,
    Cliente,
    Agente
}

public class Usuario
{
    public int Id { get; set; }
    public string Descripcion { get; set; } = default!;
    public TipoUsuario Tipo { get; set; }
    public string CorreoElectronico { get; set; } = default!;
    public string Telefono { get; set; } = default!;
    public bool Activo { get; set; }
}
