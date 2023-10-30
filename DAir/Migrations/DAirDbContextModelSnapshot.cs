﻿// <auto-generated />
using DAir.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DAir.Migrations
{
    [DbContext(typeof(DAirDbContext))]
    partial class DAirDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("DAir.Models.CabinMember", b =>
                {
                    b.Property<int>("CabinMemberID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CabinMemberID"));

                    b.Property<string>("Certification")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("EmployeeID")
                        .HasColumnType("int");

                    b.Property<string>("GeoLocation")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CabinMemberID");

                    b.HasIndex("EmployeeID");

                    b.ToTable("CabinMembers");
                });

            modelBuilder.Entity("DAir.Models.Crew", b =>
                {
                    b.Property<int>("CrewID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CrewID"));

                    b.Property<int>("CoPilot")
                        .HasColumnType("int");

                    b.Property<int>("FlightAttendant")
                        .HasColumnType("int");

                    b.Property<int>("Pilot")
                        .HasColumnType("int");

                    b.Property<int>("Pursuer")
                        .HasColumnType("int");

                    b.HasKey("CrewID");

                    b.HasIndex("CoPilot");

                    b.HasIndex("FlightAttendant");

                    b.HasIndex("Pilot");

                    b.HasIndex("Pursuer");

                    b.ToTable("Crews");
                });

            modelBuilder.Entity("DAir.Models.Employee", b =>
                {
                    b.Property<int>("EmployeeID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("EmployeeID"));

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("EmployeeID");

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("DAir.Models.Flight", b =>
                {
                    b.Property<string>("FlightCode")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AircraftID")
                        .HasColumnType("int");

                    b.Property<string>("ArrivalAirport")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DepartureAirport")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ScheduledArrivalTime")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ScheduledDepartureTime")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("State")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("FlightCode");

                    b.ToTable("Flights");
                });

            modelBuilder.Entity("DAir.Models.FlightSchedule", b =>
                {
                    b.Property<string>("FlightCode")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("EmployeeID")
                        .HasColumnType("int");

                    b.HasKey("FlightCode", "EmployeeID");

                    b.HasIndex("EmployeeID");

                    b.ToTable("FlightSchedules");
                });

            modelBuilder.Entity("DAir.Models.Languages", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<int>("CabinMemberID")
                        .HasColumnType("int");

                    b.Property<string>("Language")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.HasIndex("CabinMemberID");

                    b.ToTable("Languages");
                });

            modelBuilder.Entity("DAir.Models.Pilot", b =>
                {
                    b.Property<int>("PilotID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PilotID"));

                    b.Property<string>("Certification")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("EmployeeID")
                        .HasColumnType("int");

                    b.Property<string>("GeoLocation")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Rank")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PilotID");

                    b.HasIndex("EmployeeID");

                    b.ToTable("Pilots");
                });

            modelBuilder.Entity("DAir.Models.Rating", b =>
                {
                    b.Property<int>("RaterID")
                        .HasColumnType("int");

                    b.Property<int>("RateeID")
                        .HasColumnType("int");

                    b.Property<int>("RatingValue")
                        .HasColumnType("int");

                    b.HasKey("RaterID", "RateeID");

                    b.HasIndex("RateeID");

                    b.ToTable("Ratings");
                });

            modelBuilder.Entity("DAir.Models.CabinMember", b =>
                {
                    b.HasOne("DAir.Models.Employee", "Employee")
                        .WithMany("CabinMembers")
                        .HasForeignKey("EmployeeID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Employee");
                });

            modelBuilder.Entity("DAir.Models.Crew", b =>
                {
                    b.HasOne("DAir.Models.Employee", "CoPilotEmployee")
                        .WithMany()
                        .HasForeignKey("CoPilot")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("DAir.Models.Employee", "FlightAttendantEmployee")
                        .WithMany()
                        .HasForeignKey("FlightAttendant")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("DAir.Models.Employee", "PilotEmployee")
                        .WithMany()
                        .HasForeignKey("Pilot")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("DAir.Models.Employee", "PursuerEmployee")
                        .WithMany()
                        .HasForeignKey("Pursuer")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("CoPilotEmployee");

                    b.Navigation("FlightAttendantEmployee");

                    b.Navigation("PilotEmployee");

                    b.Navigation("PursuerEmployee");
                });

            modelBuilder.Entity("DAir.Models.FlightSchedule", b =>
                {
                    b.HasOne("DAir.Models.Employee", "Employee")
                        .WithMany("FlightSchedules")
                        .HasForeignKey("EmployeeID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DAir.Models.Flight", "Flight")
                        .WithMany("FlightSchedules")
                        .HasForeignKey("FlightCode")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Employee");

                    b.Navigation("Flight");
                });

            modelBuilder.Entity("DAir.Models.Languages", b =>
                {
                    b.HasOne("DAir.Models.CabinMember", "CabinMember")
                        .WithMany("Languages")
                        .HasForeignKey("CabinMemberID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CabinMember");
                });

            modelBuilder.Entity("DAir.Models.Pilot", b =>
                {
                    b.HasOne("DAir.Models.Employee", "Employee")
                        .WithMany("Pilots")
                        .HasForeignKey("EmployeeID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Employee");
                });

            modelBuilder.Entity("DAir.Models.Rating", b =>
                {
                    b.HasOne("DAir.Models.Pilot", "Ratee")
                        .WithMany("RatingsReceived")
                        .HasForeignKey("RateeID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("DAir.Models.Employee", "Rater")
                        .WithMany("RatingsGiven")
                        .HasForeignKey("RaterID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Ratee");

                    b.Navigation("Rater");
                });

            modelBuilder.Entity("DAir.Models.CabinMember", b =>
                {
                    b.Navigation("Languages");
                });

            modelBuilder.Entity("DAir.Models.Employee", b =>
                {
                    b.Navigation("CabinMembers");

                    b.Navigation("FlightSchedules");

                    b.Navigation("Pilots");

                    b.Navigation("RatingsGiven");
                });

            modelBuilder.Entity("DAir.Models.Flight", b =>
                {
                    b.Navigation("FlightSchedules");
                });

            modelBuilder.Entity("DAir.Models.Pilot", b =>
                {
                    b.Navigation("RatingsReceived");
                });
#pragma warning restore 612, 618
        }
    }
}
