using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database
{
    public class AuthDbContext(DbContextOptions<AuthDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<VerificationCode> VerificationCodes { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<UserInfo> UserInfos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User
            modelBuilder.Entity<User>().HasKey(x => x.Id);

            // VerificationCode
            modelBuilder.Entity<VerificationCode>().HasKey(x => x.Id);
            modelBuilder.Entity<VerificationCode>().HasOne<User>().WithMany().HasForeignKey(x => x.UserId);

            // RefreshToken
            modelBuilder.Entity<RefreshToken>().HasKey(x => x.Id);
            modelBuilder.Entity<RefreshToken>().HasOne<User>().WithMany().HasForeignKey(x => x.UserId);

            // UserInfo
            modelBuilder.Entity<UserInfo>().HasKey(x => x.Id);
            modelBuilder.Entity<UserInfo>().HasOne<User>().WithMany().HasForeignKey(x => x.UserId);
        }
    }
}
