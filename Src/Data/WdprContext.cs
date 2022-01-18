using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using Wdpr_Groep_E.Models;

namespace Wdpr_Groep_E.Data
{
    public class WdprContext : IdentityDbContext<AppUser>
    {
        public WdprContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<SignUp> SignUps { get; set; }
        public DbSet<SignUpChild> SignUpChildren {get;set;}

        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<ChatUser> ChatUsers { get; set; }

        public DbSet<Caregiver> Caregivers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ChatUser>().HasKey(cu => new { cu.ChatId, cu.UserId });
            builder.Entity<SignUpChild>().HasOne(s => s.Signup).WithMany(s => s.Children).HasForeignKey(s => s.Subject).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<SignUp>().HasMany(s => s.Children).WithOne(s => s.Signup).HasForeignKey(s => s.Subject).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
