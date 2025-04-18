
using Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data.Contexts;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<UserEntity>(options)
{
    public virtual DbSet<ClientEntity> Clients { get; set; }
    public virtual DbSet<StatusEntity> Statuses { get; set; }
    public virtual DbSet<ProjectEntity> Projects { get; set; }
    public virtual DbSet<MemberEntity> Members { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure relationships
        builder.Entity<MemberEntity>()
            .HasOne(pm => pm.Project)
            .WithMany(p => p.projectMembers)
            .HasForeignKey(pm => pm.ProjectId);

        builder.Entity<MemberEntity>()
            .HasOne(pm => pm.User)
            .WithMany()
            .HasForeignKey(pm => pm.UserId);

        // Seed initial project statuses
        builder.Entity<StatusEntity>().HasData(
            new StatusEntity { Id = 1, StatusName = "started" },
            new StatusEntity { Id = 2, StatusName = "completed" }
        );
    }
}
