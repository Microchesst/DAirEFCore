using DAir.Context;
using DAir.Models;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Policy = "adminPolicy")]
    [ApiController]
    public class QueriesController : ControllerBase
    {
        private readonly DAirDbContext _context;
        private readonly ILogger<QueriesController> _logger;


        public QueriesController(DAirDbContext context, ILogger<QueriesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("GetFlightDetails/{flightCode}")]
        public async Task<ActionResult<Flight>> GetFlightDetails(string flightCode)
        {
            try
            {
                _logger.LogInformation("Request to get flight details for {FlightCode}", flightCode);

                var flight = await _context.Flights
                    .Where(f => f.FlightCode == flightCode)
                    .FirstOrDefaultAsync();

                if (flight == null)
                {
                    _logger.LogWarning("Flight not found for {FlightCode}", flightCode);
                    return NotFound();
                }

                _logger.LogInformation("Flight details retrieved for {FlightCode}", flightCode);
                return flight;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting flight details for {FlightCode}", flightCode);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred processing your request.");
            }
        }

        // 2. Crew Members Available for Specific Aircraft at an Airport
        [HttpGet("GetCrewMembersForAircraftAtAirport/{aircraftType}/{airport}")]
        public async Task<ActionResult<List<string>>> GetCrewMembersForAircraftAtAirport(string aircraftType, string airport)
        {
            var query = from pilot in _context.Pilots
                        join certification in _context.Certifications
                        on pilot.PilotID equals certification.PilotID
                        where pilot.GeoLocation == airport && certification.CertificationName == aircraftType
                        select new
                        {
                            Name = pilot.Employee.FirstName + " " + pilot.Employee.LastName,
                            aircraftTypeName = aircraftType,
                            LicenseNumber = certification.LicenseNumber
                        };

            var crewMembers = await query.ToListAsync();

            return Ok(crewMembers);
        }

        // 3. Count of Canceled Flights
        [HttpGet("GetCanceledFlightsCount")]
        public async Task<ActionResult<int>> GetCanceledFlightsCount()
        {
            int canceledFlightsCount = await _context.Flights
                .CountAsync(f => f.State == "Canceled");

            return canceledFlightsCount;
        }

        // 4. Flight Schedule per Employee by Airport - Filtered by Employee Name
        [HttpGet("GetFlightScheduleByEmployee/{employeeName}")]
        public async Task<ActionResult<IEnumerable<EmployeeFlightSchedule>>> GetFlightScheduleByEmployee(string employeeName)
        {
            var nameParts = employeeName.Split(' ');
            var firstName = nameParts[0];
            var lastName = nameParts.Length > 1 ? nameParts[1] : "";

            var flightSchedules = await _context.FlightSchedules
                .Include(fs => fs.Employee)
                .Include(fs => fs.Flight)
                .Where(fs => fs.Employee.FirstName == firstName && fs.Employee.LastName == lastName)
                .GroupBy(fs => new { fs.Employee.FirstName, fs.Employee.LastName, fs.Flight.DepartureAirport })
                .Select(group => new EmployeeFlightSchedule
                {
                    EmployeeName = group.Key.FirstName + " " + group.Key.LastName,
                    Airport = group.Key.DepartureAirport,
                    FlightCount = group.Count()
                })
                .OrderBy(efs => efs.Airport)
                .ToListAsync();

            return flightSchedules;
        }





        // 5. Average Rating Given by a Pilot
        [HttpGet("GetAverageRatingByPilot/{pilotName}")]
        public async Task<ActionResult<List<string>>> GetAverageRatingByPilot(string pilotName)
        {
            // Find the EmployeeID of the given employee name
            int? employeeId = await _context.Employees
                .Where(e => e.FirstName + " " + e.LastName == pilotName)
                .Select(e => e.EmployeeID)
                .FirstOrDefaultAsync();

            if (employeeId.HasValue)
            {
                // Find the corresponding PilotID in the Pilots table
                int? pilotId = await _context.Pilots
                    .Where(p => p.EmployeeID == employeeId)
                    .Select(p => p.PilotID)
                    .FirstOrDefaultAsync();

                if (pilotId.HasValue)
                {
                    // List all EmployeeIDs associated with the PilotID in the Conflict table
                    List<int> employeeIds = await _context.Conflicts
                        .Where(c => c.PilotID == pilotId)
                        .Select(c => c.EmployeeID)
                        .ToListAsync();

                    // Get the names of employees with the retrieved IDs
                    List<string> employeeNames = await _context.Employees
                        .Where(e => employeeIds.Contains(e.EmployeeID))
                        .Select(e => e.FirstName + " " + e.LastName)
                        .ToListAsync();
                    return employeeNames;
                }
                else return NotFound("Pilot not found.");
            }
            else { return NotFound("Pilot not found with that name."); }
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