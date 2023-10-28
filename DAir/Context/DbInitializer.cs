using DAir.Models;

namespace DAir.Context
{
    public static class DbInitializer
    {
        public static void Initialize(DAirDbContext context)
        {
            context.Database.EnsureCreated();

            // Check if the database has been seeded already
            if (context.Employees.Any())
            {
                return;   // DB has been seeded
            }

            // Employees
            var employees = new List<Employee>
        {
            new Employee { FirstName = "Noah", LastName = "Smith", Role = "Pilot" },
            new Employee { FirstName = "Laerke", LastName = "Jensen", Role = "Cabin Crew" },
            new Employee { FirstName = "Anton", LastName = "Nielsen", Role = "Cabin Crew" }
            // Add more employees as needed
        };
            context.Employees.AddRange(employees);

            // Pilots
            var pilots = new List<Pilot>
        {
            new Pilot { Employee = employees[0], GeoLocation = "CPH", Certification = "Airbus 350", Rank = "Captain" },
            // Add more pilots as needed
        };
            context.Pilots.AddRange(pilots);

            // Cabin Members
            var cabinMembers = new List<CabinMember>
        {
            new CabinMember { Employee = employees[1], GeoLocation = "CPH", Certification = "Safety", Languages = new List<Languages> { new Languages { Language = "Danish" }, new Languages { Language = "English" } } },
            new CabinMember { Employee = employees[2], GeoLocation = "CPH", Certification = "Service", Languages = new List<Languages> { new Languages { Language = "Danish" }, new Languages { Language = "French" } } }
            // Add more cabin members as needed
        };
            context.CabinMembers.AddRange(cabinMembers);

            // Flights
            var flights = new List<Flight>
        {
            new Flight { FlightCode = "SK935", DepartureAirport = "CPH", ArrivalAirport = "SFO", ScheduledDepartureTime = new DateTime(2023, 10, 28, 16, 15, 0), ScheduledArrivalTime = new DateTime(2023, 10, 28, 18, 35, 0), State = "Scheduled", AircraftID = 1 },
            // Add more flights as needed
        };
            context.Flights.AddRange(flights);

            // Flight Schedules
            var flightSchedules = new List<FlightSchedule>
        {
            new FlightSchedule { Flight = flights[0], Employee = employees[0] },
            // Add more flight schedules as needed
        };
            context.FlightSchedules.AddRange(flightSchedules);

            // Ratings
            var ratings = new List<Rating>
        {
            new Rating { Rater = employees[0], Ratee = pilots[0], RatingValue = 4 },
            // Add more ratings as needed
        };
            context.Ratings.AddRange(ratings);

            context.SaveChanges();
        }
    }
}
