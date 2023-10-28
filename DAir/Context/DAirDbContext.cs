using Microsoft.EntityFrameworkCore;
using DAir.Models;

namespace DAir.Context
{
    public class DAirDbContext: DbContext
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

        public DAirDbContext(DbContextOptions<DAirDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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

            // Employee and Rating (as Rater)
            modelBuilder.Entity<Employee>()
                .HasMany(e => e.RatingsGiven)
                .WithOne(r => r.Rater)
                .HasForeignKey(r => r.RaterID);

            // Pilot and Rating (as Ratee)
            modelBuilder.Entity<Pilot>()
                .HasMany(p => p.RatingsReceived)
                .WithOne(r => r.Ratee)
                .HasForeignKey(r => r.RateeID);

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
            // Assuming Crew class has navigation properties for each role, e.g., PilotEmployee, CoPilotEmployee, etc.
            modelBuilder.Entity<Crew>()
                .HasOne(c => c.PilotEmployee)
                .WithMany()
                .HasForeignKey(c => c.Pilot);

            modelBuilder.Entity<Crew>()
                .HasOne(c => c.CoPilotEmployee)
                .WithMany()
                .HasForeignKey(c => c.CoPilot);

            // ... similar for Pursuer and FlightAttendant
        }
    }
}
