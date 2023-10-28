using DAir.Context;
using DAir.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DAir.Controllers
{

    // Additional class definition for EmployeeFlightSchedule
    public class EmployeeFlightSchedule
    {
        public string EmployeeName { get; set; }
        public string Airport { get; set; }
        public int FlightCount { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class QueriesController : ControllerBase
    {
        private readonly DAirDbContext _context;

        public QueriesController(DAirDbContext context)
        {
            _context = context;
        }

        // Add your query methods here
        // Example: Get flight details by flight code
        [HttpGet("GetFlightDetails/{flightCode}")]
        public async Task<ActionResult<Flight>> GetFlightDetails(string flightCode)
        {
            var flight = await _context.Flights
                .Where(f => f.FlightCode == flightCode)
                .FirstOrDefaultAsync();

            if (flight == null)
            {
                return NotFound();
            }

            return flight;
        }
        // 2. Crew Members Available for Specific Aircraft at an Airport
        [HttpGet("GetCrewMembersForAircraftAtAirport/{aircraftType}/{airport}")]
        public async Task<ActionResult<List<string>>> GetCrewMembersForAircraftAtAirport(string aircraftType, string airport)
        {
            var pilots = await _context.Pilots
                .Where(p => p.Certification == aircraftType && p.GeoLocation == airport)
                .Select(p => p.Employee.FirstName + " " + p.Employee.LastName)
                .ToListAsync();

            var cabinMembers = await _context.CabinMembers
                .Where(cm => cm.Certification == aircraftType && cm.GeoLocation == airport)
                .Select(cm => cm.Employee.FirstName + " " + cm.Employee.LastName)
                .ToListAsync();

            var crewMembers = pilots.Concat(cabinMembers).Distinct().ToList();

            return crewMembers;
        }

        // 3. Count of Canceled Flights
        [HttpGet("GetCanceledFlightsCount")]
        public async Task<ActionResult<int>> GetCanceledFlightsCount()
        {
            int canceledFlightsCount = await _context.Flights
                .CountAsync(f => f.State == "Canceled");

            return canceledFlightsCount;
        }

        // 4. Flight Schedule per Employee by Airport - Simplified
        [HttpGet("GetFlightScheduleByEmployee")]
        public async Task<ActionResult<IEnumerable<EmployeeFlightSchedule>>> GetFlightScheduleByEmployee()
        {
            var flightSchedules = await _context.FlightSchedules
                .Include(fs => fs.Employee)
                .Include(fs => fs.Flight)
                .GroupBy(fs => new { fs.Employee.FirstName, fs.Employee.LastName, fs.Flight.DepartureAirport })
                .Select(group => new EmployeeFlightSchedule
                {
                    EmployeeName = group.Key.FirstName + " " + group.Key.LastName,
                    Airport = group.Key.DepartureAirport,
                    FlightCount = group.Count()
                })
                .OrderBy(efs => efs.EmployeeName)
                .ThenBy(efs => efs.Airport)
                .ToListAsync();

            return flightSchedules;
        }




        // 5. Average Rating Given by a Pilot
        [HttpGet("GetAverageRatingByPilot/{pilotName}")]
        public async Task<ActionResult<double>> GetAverageRatingByPilot(string pilotName)
        {
            var pilot = await _context.Pilots
                .Include(p => p.Employee)
                .Where(p => p.Employee.FirstName + " " + p.Employee.LastName == pilotName)
                .FirstOrDefaultAsync();

            if (pilot == null)
            {
                return NotFound("Pilot not found.");
            }

            var averageRating = await _context.Ratings
                .Where(r => r.RateeID == pilot.PilotID)
                .DefaultIfEmpty()
                .AverageAsync(r => r == null ? 0 : r.RatingValue);

            return averageRating;
        }

        // 6. Languages Spoken by a Cabin Crew Member
        [HttpGet("GetLanguagesByCrewMember/{crewMemberName}")]
        public async Task<ActionResult<List<string>>> GetLanguagesByCrewMember(string crewMemberName)
        {
            var crewMemberLanguages = await _context.CabinMembers
                .Include(cm => cm.Employee)
                .Include(cm => cm.Languages)
                .Where(cm => cm.Employee.FirstName + " " + cm.Employee.LastName == crewMemberName)
                .SelectMany(cm => cm.Languages)
                .Select(l => l.Language) // Adjusted to the correct property name of Languages
                .ToListAsync();

            return crewMemberLanguages;
        }
        // 7. Pilot with Highest Average Rating
        [HttpGet("GetPilotWithHighestAverageRating")]
        public async Task<ActionResult<string>> GetPilotWithHighestAverageRating()
        {
            var highestRatedPilot = await _context.Ratings
                .GroupBy(r => r.RateeID)
                .Select(group => new
                {
                    PilotID = group.Key,
                    AverageRating = group.Average(r => r.RatingValue)
                })
                .OrderByDescending(r => r.AverageRating)
                .Select(r => r.PilotID)
                .FirstOrDefaultAsync();

            var pilotName = await _context.Pilots
                .Where(p => p.PilotID == highestRatedPilot)
                .Select(p => p.Employee.FirstName + " " + p.Employee.LastName)
                .FirstOrDefaultAsync();

            return pilotName;
        }

    }
}