using Demo.Domain;
using Microsoft.EntityFrameworkCore;

namespace Demo.Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Usuario>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Descripcion).HasMaxLength(200).IsRequired();
            e.Property(x => x.CorreoElectronico).HasMaxLength(200).IsRequired();
            e.Property(x => x.Telefono).HasMaxLength(20).IsRequired();
            e.Property(x => x.Tipo).HasConversion<string>().IsRequired();
            e.HasIndex(x => x.CorreoElectronico).IsUnique();
        });
    }
}
