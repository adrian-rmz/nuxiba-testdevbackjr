using Microsoft.EntityFrameworkCore;
using NuxibaEvaluation.Api.Models;

namespace NuxibaEvaluation.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Login> Logins => Set<Login>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Area> Areas => Set<Area>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Login>(entity =>
        {
            entity.ToTable("ccloglogin");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.UserId).HasColumnName("User_id");
            entity.Property(x => x.Fecha).HasColumnName("fecha");

            entity.HasIndex(x => new { x.UserId, x.Fecha });
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("ccUsers");
            entity.HasKey(x => x.UserId);

            entity.Property(x => x.UserId).HasColumnName("User_id");
            entity.Property(x => x.TipoUserId).HasColumnName("TipoUser_id");
            entity.Property(x => x.IDArea).HasColumnName("IDArea");
        });

        modelBuilder.Entity<Area>(entity =>
        {
            entity.ToTable("ccRIACat_Areas");
            entity.HasKey(x => x.IDArea);

            entity.Property(x => x.IDArea).HasColumnName("IDArea");
        });
    }
}