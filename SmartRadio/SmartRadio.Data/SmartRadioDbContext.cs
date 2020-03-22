using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartRadio.Data.Models;

namespace SmartRadio.Data
{
    public class SmartRadioDbContext : IdentityDbContext<User>
    {
        public DbSet<UserFollower> Following { get; set; }

        public DbSet<SongData> Songs { get; set; }

        public DbSet<UserSong> UserSongs { get; set; }

        public DbSet<SongFingerprint> Fingerprints { get; set; }

        public SmartRadioDbContext(DbContextOptions<SmartRadioDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserFollower>()
                .HasOne(uf => uf.User1)
                .WithMany(u => u.Following)
                .HasForeignKey(uf => uf.Id1);

            builder.Entity<UserFollower>()
                .HasOne(uf => uf.User2)
                .WithMany()
                .HasForeignKey(uf => uf.Id2);

            builder.Entity<UserFollower>()
                .HasKey(uf => new {uf.Id1, uf.Id2});

            builder.Entity<UserSong>()
                .HasOne(us => us.Listener)
                .WithMany()
                .HasForeignKey(us => us.ListenerId);

            builder.Entity<UserSong>()
                .HasOne(us => us.Song)
                .WithMany()
                .HasForeignKey(us => us.SongId);

            builder.Entity<UserSong>()
                .HasKey(us => new {us.ListenerId, us.SongId, us.Date});

            builder.Entity<SongFingerprint>()
                .HasOne(sf => sf.Song)
                .WithMany(sd => sd.Fingerprints)
                .HasForeignKey(sf => sf.SongId);
        }
    }
}
