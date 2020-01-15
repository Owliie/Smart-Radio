using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartRadio.Data.Models;

namespace SmartRadio.Data
{
    public class SmartRadioDbContext : IdentityDbContext<User>
    {
        public DbSet<UserFriend> Friends { get; set; }

        public DbSet<SongData> Songs { get; set; }

        public SmartRadioDbContext(DbContextOptions<SmartRadioDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserFriend>()
                .HasOne(uf => uf.User1)
                .WithMany(u => u.Friends)
                .HasForeignKey(uf => uf.Id1);

            builder.Entity<UserFriend>()
                .HasOne(uf => uf.User2)
                .WithMany()
                .HasForeignKey(uf => uf.Id2);

            builder.Entity<UserFriend>()
                .HasKey(uf => new {uf.Id1, uf.Id2});

            builder.Entity<SongData>()
                .HasOne(sd => sd.Listener)
                .WithMany()
                .HasForeignKey(sd => sd.ListenerId);
        }
    }
}
