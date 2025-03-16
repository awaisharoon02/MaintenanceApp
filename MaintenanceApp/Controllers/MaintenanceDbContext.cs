using MaintenanceApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MaintenanceApp.Controllers
{
    public class MaintenanceDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<PhraseModel> phraseModel { get; set; }
        public DbSet<RequestMaintenanceModel> requestMaintenanceModel { get; set; }
       
        public MaintenanceDbContext(DbContextOptions<MaintenanceDbContext> options) : base(options)
        {
            

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PhraseModel>()
            .HasOne(p => p.RequestMaintenance) // Navigation property
            .WithMany(r => r.phraseModel) // Many-to-one relationship
            .HasForeignKey(p => p.RequestMaintenanceID) // Foreign key
            .OnDelete(DeleteBehavior.NoAction);
            base.OnModelCreating(modelBuilder);


            
        }


    }
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }

        // Navigation property for related maintenance requests
        public ICollection<RequestMaintenanceModel> MaintenanceRequests { get; set; }
    }
}
