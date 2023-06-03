using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Abdulrhmaan.News.SQlServer;

public class NewsContext : IdentityDbContext<User>
{
    public virtual DbSet<User> Authors { get; set; }
    public virtual DbSet<News> News { get; set; }
    public virtual DbSet<Category> Categorys { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder OptionsBuilder)
    {
        if (OptionsBuilder.IsConfigured) return;
        OptionsBuilder.UseSqlServer("Server=DESKTOP-41ISMFG;Database=NewsDb;Integrated Security=True;TrustServerCertificate=True;");
        OptionsBuilder
              .EnableDetailedErrors()
              .EnableSensitiveDataLogging();
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");
            entity.Property(e => e.Id).ValueGeneratedNever();

        });

        modelBuilder.Entity<News>(entity =>
        {
            entity.ToTable("News");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();

        });

        //user-newsss
        modelBuilder.Entity<User>()
.HasMany(u => u.News)
.WithOne(n => n.User)
.HasForeignKey(n => n.UserId);

        //user-categorys
        modelBuilder.Entity<User>()
.HasMany(u => u.News)
.WithOne(n => n.User)
.HasForeignKey(n => n.UserId);

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Category");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
        });


        //category-newsss
        modelBuilder.Entity<Category>()
    .HasMany(c => c.News)
    .WithOne(n => n.Category)
    .HasForeignKey(n => n.CategoryId);

        modelBuilder.Entity<IdentityRole>().ToTable("Roles");
        modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
        modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
        modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
        modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");

    }


}
