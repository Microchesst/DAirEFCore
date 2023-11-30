using Microsoft.EntityFrameworkCore;
using DAir.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;


namespace DAir.Context
{
    public class DAirDbContext : IdentityDbContext<ApiUser>
    {
        // DbSet properties
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Pilot> Pilots { get; set; }
        public DbSet<CabinMember> CabinMembers { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Languages> Languages { get; set; }
        public DbSet<Crew> Crews { get; set; }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<FlightSchedule> FlightSchedules { get; set; }
        public DbSet<Certification> Certifications { get; set; }
        public DbSet<Conflict> Conflicts { get; set; }
        public DbSet<ApiUser> ApiUsers { get; set; }




        public DAirDbContext(DbContextOptions<DAirDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Employee and Pilot
            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Pilots)
                .WithOne(p => p.Employee)
                .HasForeignKey(p => p.EmployeeID);

            // Employee and CabinMember
            modelBuilder.Entity<Employee>()
                .HasMany(e => e.CabinMembers)
                .WithOne(cm => cm.Employee)
                .HasForeignKey(cm => cm.EmployeeID);

            // Employee and FlightSchedule
            modelBuilder.Entity<Employee>()
                .HasMany(e => e.FlightSchedules)
                .WithOne(fs => fs.Employee)
                .HasForeignKey(fs => fs.EmployeeID);

            // Rating configuration
            modelBuilder.Entity<Rating>()
                .HasKey(r => new { r.RaterID, r.RateeID });

            modelBuilder.Entity<Rating>()
                .HasOne(r => r.Rater)
                .WithMany(e => e.RatingsGiven)
                .HasForeignKey(r => r.RaterID)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            modelBuilder.Entity<Rating>()
                .HasOne(r => r.Ratee)
                .WithMany(p => p.RatingsReceived)
                .HasForeignKey(r => r.RateeID)
                .OnDelete(DeleteBehavior.Restrict);

            // CabinMember and Language
            modelBuilder.Entity<CabinMember>()
                .HasMany(cm => cm.Languages)
                .WithOne(l => l.CabinMember)
                .HasForeignKey(l => l.CabinMemberID);

            // Flight and FlightSchedule
            modelBuilder.Entity<Flight>()
                .HasMany(f => f.FlightSchedules)
                .WithOne(fs => fs.Flight)
                .HasForeignKey(fs => fs.FlightCode);

            // Crew and Employee (for different roles)
            modelBuilder.Entity<Crew>()
                .HasOne(c => c.PilotEmployee)
                .WithMany()
                .HasForeignKey(c => c.Pilot)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Crew>()
                .HasOne(c => c.CoPilotEmployee)
                .WithMany()
                .HasForeignKey(c => c.CoPilot)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Crew>()
                .HasOne(c => c.PursuerEmployee)
                .WithMany()
                .HasForeignKey(c => c.Pursuer)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Crew>()
                .HasOne(c => c.FlightAttendantEmployee)
                .WithMany()
                .HasForeignKey(c => c.FlightAttendant)
                .OnDelete(DeleteBehavior.Restrict);

            // Composite Key for FlightSchedule
            modelBuilder.Entity<FlightSchedule>()
                .HasKey(fs => new { fs.FlightCode, fs.EmployeeID });

            // Composite Key for Rating
            modelBuilder.Entity<Rating>()
                .HasKey(r => new { r.RaterID, r.RateeID });

        }
    }
}
