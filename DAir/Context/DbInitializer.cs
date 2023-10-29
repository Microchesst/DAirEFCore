﻿using DAir.Models;
using System;
using System.Collections.Generic; 
using System.Linq; 

namespace DAir.Context
{
    public static class DbInitializer
    {
        public static void Initialize(DAirDbContext context)
        {
            context.Database.EnsureCreated();

            // Clear existing data (Reseeding)
            context.Languages.RemoveRange(context.Languages);
            context.CabinMembers.RemoveRange(context.CabinMembers);
            context.Pilots.RemoveRange(context.Pilots);
            context.Employees.RemoveRange(context.Employees);
            context.Flights.RemoveRange(context.Flights);
            context.FlightSchedules.RemoveRange(context.FlightSchedules);
            context.Ratings.RemoveRange(context.Ratings);

            // Check if the database has been seeded already
            if (context.Employees.Any())
            {
                return;   // DB has been seeded
            }

            // Ensure all changes are committed before seeding new data
            context.SaveChanges();

            // Employees
            var employees = new List<Employee>
            {
                new Employee { FirstName = "Noah", LastName = "Smith", Role = "Pilot" },
                new Employee { FirstName = "Laerke", LastName = "Jensen", Role = "Cabin Crew" },
                new Employee { FirstName = "Anton", LastName = "Nielsen", Role = "Cabin Crew" },
                // Add more employees...
                new Employee { FirstName = "Eva", LastName = "Green", Role = "Pilot" },
                new Employee { FirstName = "Liam", LastName = "Brown", Role = "Cabin Crew" },
                // Adding more for diverse data
                new Employee { FirstName = "Olivia", LastName = "Williams", Role = "Pilot" },
                new Employee { FirstName = "Emma", LastName = "Johnson", Role = "Cabin Crew" },
                // ... more employees as needed
            };
            context.Employees.AddRange(employees);

            // Pilots
            var pilots = new List<Pilot>
            {
                new Pilot { Employee = employees[0], GeoLocation = "CPH", Certification = "Airbus 350", Rank = "Captain" },
                new Pilot { Employee = employees[3], GeoLocation = "SFO", Certification = "Boeing 777", Rank = "First Officer" },
                new Pilot { Employee = employees[5], GeoLocation = "CPH", Certification = "Airbus 320", Rank = "Captain" },
                // ... more pilots as needed
            };
            context.Pilots.AddRange(pilots);

            // Cabin Members
            var cabinMembers = new List<CabinMember>
            {
                new CabinMember { Employee = employees[1], GeoLocation = "CPH", Certification = "Safety", Languages = new List<Languages> { new Languages { Language = "Danish" }, new Languages { Language = "English" } } },
                new CabinMember { Employee = employees[2], GeoLocation = "CPH", Certification = "Service", Languages = new List<Languages> { new Languages { Language = "Danish" }, new Languages { Language = "French" } } },
                new CabinMember { Employee = employees[4], GeoLocation = "SFO", Certification = "Safety", Languages = new List<Languages> { new Languages { Language = "English" }, new Languages { Language = "Spanish" } } },
                new CabinMember { Employee = employees[6], GeoLocation = "SFO", Certification = "Service", Languages = new List<Languages> { new Languages { Language = "English" }, new Languages { Language = "German" } } },
                // ... more cabin members as needed
            };
            context.CabinMembers.AddRange(cabinMembers);
            context.SaveChanges();

            // Languages
            var languages = new List<Languages>
            {
                new Languages { Language = "Danish", CabinMemberID = cabinMembers[0].CabinMemberID },
                new Languages { Language = "English", CabinMemberID = cabinMembers[0].CabinMemberID },
                new Languages { Language = "Danish", CabinMemberID = cabinMembers[1].CabinMemberID },
                new Languages { Language = "French", CabinMemberID = cabinMembers[1].CabinMemberID },
                // ... additional languages for other cabin members
            };

            context.Languages.AddRange(languages);

            // Flights
            var flights = new List<Flight>
            {
                new Flight { FlightCode = "SK935", DepartureAirport = "CPH", ArrivalAirport = "SFO", ScheduledDepartureTime = new DateTime(2023, 10, 10, 16, 15, 0), ScheduledArrivalTime = new DateTime(2023, 10, 10, 18, 35, 0), State = "Scheduled" },
                new Flight { FlightCode = "LH123", DepartureAirport = "SFO", ArrivalAirport = "CPH", ScheduledDepartureTime = new DateTime(2023, 10, 11, 10, 0, 0), ScheduledArrivalTime = new DateTime(2023, 10, 11, 14, 30, 0), State = "Canceled" },
                new Flight { FlightCode = "BA456", DepartureAirport = "CPH", ArrivalAirport = "LHR", ScheduledDepartureTime = new DateTime(2023, 10, 12, 9, 0, 0), ScheduledArrivalTime = new DateTime(2023, 10, 12, 11, 20, 0), State = "Delayed" },
                // ... more flights as needed
            };
            context.Flights.AddRange(flights);

            // Flight Schedules
            var flightSchedules = new List<FlightSchedule>
            {
                new FlightSchedule { Flight = flights[0], Employee = employees[0] },
                new FlightSchedule { Flight = flights[0], Employee = employees[1] },
                new FlightSchedule { Flight = flights[1], Employee = employees[3] },
                new FlightSchedule { Flight = flights[2], Employee = employees[5] },
                new FlightSchedule { Flight = flights[2], Employee = employees[6] },
                // ... more flight schedules as needed
            };
            context.FlightSchedules.AddRange(flightSchedules);

            // Ratings
            var ratings = new List<Rating>
            {
                new Rating { Rater = employees[0], Ratee = pilots[1], RatingValue = 4 },
                new Rating { Rater = employees[0], Ratee = pilots[0], RatingValue = 3 },
                new Rating { Rater = employees[3], Ratee = pilots[0], RatingValue = 5 },
                // ... more ratings as needed
                new Rating { Rater = employees[5], Ratee = pilots[2], RatingValue = 3 },
                new Rating { Rater = employees[6], Ratee = pilots[1], RatingValue = 1 },
                // ... more ratings for robust data
            };
            context.Ratings.AddRange(ratings);

            context.SaveChanges();
        }
    }
}
