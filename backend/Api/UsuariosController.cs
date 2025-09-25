using Demo.Contracts;
using Demo.Domain;
using Demo.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Demo.Api;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UsuarioDto>>> GetAll()
    {
        var data = await db.Usuarios
            .Select(u => new UsuarioDto(u.Id, u.Descripcion, u.Tipo, u.CorreoElectronico, u.Telefono, u.Activo))
            .ToListAsync();

        return Ok(data);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UsuarioDto>> GetById(int id)
    {
        var u = await db.Usuarios.FindAsync(id);
        if (u is null) return NotFound();

        return new UsuarioDto(u.Id, u.Descripcion, u.Tipo, u.CorreoElectronico, u.Telefono, u.Activo);
    }

    [HttpPost]
    public async Task<ActionResult<UsuarioDto>> Create([FromBody] UsuarioCreateUpdateDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var exists = await db.Usuarios.AnyAsync(x => x.CorreoElectronico == dto.CorreoElectronico);
        if (exists) return Conflict("Ya existe un usuario con ese correo.");

        var u = new Usuario
        {
            Descripcion = dto.Descripcion,
            Tipo = dto.Tipo,
            CorreoElectronico = dto.CorreoElectronico,
            Telefono = dto.Telefono,
            Activo = dto.Activo
        };

        db.Usuarios.Add(u);
        await db.SaveChangesAsync();

        var result = new UsuarioDto(u.Id, u.Descripcion, u.Tipo, u.CorreoElectronico, u.Telefono, u.Activo);
        return CreatedAtAction(nameof(GetById), new { id = u.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<UsuarioDto>> Update(int id, [FromBody] UsuarioCreateUpdateDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var u = await db.Usuarios.FindAsync(id);
        if (u is null) return NotFound();

        if (!string.Equals(u.CorreoElectronico, dto.CorreoElectronico, StringComparison.OrdinalIgnoreCase))
        {
            var duplicate = await db.Usuarios.AnyAsync(x => x.CorreoElectronico == dto.CorreoElectronico && x.Id != id);
            if (duplicate) return Conflict("Ya existe un usuario con ese correo.");
        }

        u.Descripcion = dto.Descripcion;
        u.Tipo = dto.Tipo;
        u.CorreoElectronico = dto.CorreoElectronico;
        u.Telefono = dto.Telefono;
        u.Activo = dto.Activo;

        await db.SaveChangesAsync();

        return new UsuarioDto(u.Id, u.Descripcion, u.Tipo, u.CorreoElectronico, u.Telefono, u.Activo);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var u = await db.Usuarios.FindAsync(id);
        if (u is null) return NotFound();

        db.Usuarios.Remove(u);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
