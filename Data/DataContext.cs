using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using RestApiDating.Models;

namespace RestApiDating.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
        : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // many to many relationship
            // pk: likerId, likedId
            builder.Entity<Like>()
                .HasKey(l => new { l.LikerId, l.LikedId });
            builder.Entity<Like>()
                .HasOne(l => l.Liker)
                .WithMany(l => l.Likes)
                .HasForeignKey(l => l.LikerId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Like>()
                .HasOne(l => l.Liked)
                .WithMany(l => l.Likers)
                .HasForeignKey(l => l.LikedId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Mensaje>()
                .HasOne(m => m.Emisor)
                .WithMany(e => e.MensajesEnviados)
                .HasForeignKey(m => m.EmisorId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Mensaje>()
                .HasOne( m => m.Receptor)
                .WithMany(r => r.MensajesRecibidos)
                .HasForeignKey(m => m.ReceptorId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public DbSet<Value> Values { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Foto> Fotos { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Mensaje> Mensajes { get; set; }
    }
}