using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SkillSwap.Models
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext ( DbContextOptions<ApplicationDbContext> options) : base(options)
        { }
        public DbSet<Profile> Profiles { get; set; }
        protected override void OnModelCreating ( ModelBuilder builder )
        {
            base.OnModelCreating(builder);
            builder.Entity<Profile>(entity => { entity.ToTable("tbl_Profile"); entity.HasKey(e => e.Id); });
        }
    }
}
