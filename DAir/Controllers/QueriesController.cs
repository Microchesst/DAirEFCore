using DAir.Context;
using DAir.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
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
    [Authorize]
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
                var timestamp = new DateTimeOffset(DateTime.UtcNow);
                var logInfo = new { Operation = "Get", Timestamp = timestamp };

                _logger.LogInformation("Get called {@Loginfo} ", logInfo);

                var flight = await _context.Flights
                    .Where(f => f.FlightCode == flightCode)
                    .FirstOrDefaultAsync();

                if (flight == null)
                {
                    _logger.LogWarning("Flight not found for {FlightCode}", flightCode);
                    return NotFound();
                }
                return flight;
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred processing your request.");
            }
        }

        [HttpGet("GetCrewMembersForAircraftAtAirport/{aircraftType}/{airport}")]
        public async Task<ActionResult<List<string>>> GetCrewMembersForAircraftAtAirport(string aircraftType, string airport)
        {
            try
            {
                var timestamp = new DateTimeOffset(DateTime.UtcNow);
                var logInfo = new { Operation = "Get", Timestamp = timestamp };

                _logger.LogInformation("Get called {@Loginfo} ", logInfo);

                var query = from pilot in _context.Pilots
                            join certification in _context.Certifications
                            on pilot.PilotID equals certification.PilotID
                            where pilot.GeoLocation == airport && certification.CertificationName == aircraftType
                            select new
                            {
                                Name = pilot.Employee.FirstName + " " + pilot.Employee.LastName,
                                AircraftTypeName = aircraftType,
                                LicenseNumber = certification.LicenseNumber
                            };

                var crewMembers = await query.ToListAsync();

                return Ok(crewMembers);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred processing your request.");
            }
        }

        [HttpGet("GetCanceledFlightsCount")]
        public async Task<ActionResult<int>> GetCanceledFlightsCount()
        {
            try
            {
                var timestamp = new DateTimeOffset(DateTime.UtcNow);
                var logInfo = new { Operation = "Get" , Timestamp = timestamp};

                _logger.LogInformation("Get called {@Loginfo} ", logInfo);

                int canceledFlightsCount = await _context.Flights
                    .CountAsync(f => f.State == "Canceled");

                return canceledFlightsCount;
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred processing your request.");
            }
        }

        [HttpGet("GetFlightScheduleByEmployee/{employeeName}")]
        public async Task<ActionResult<IEnumerable<EmployeeFlightSchedule>>> GetFlightScheduleByEmployee(string employeeName)
        {
            try
            {
                var timestamp = new DateTimeOffset(DateTime.UtcNow);
                var logInfo = new { Operation = "Get", Timestamp = timestamp };

                _logger.LogInformation("Get called {@Loginfo} ", logInfo);

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
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred processing your request.");
            }
        }

        // 5. Average Rating Given by a Pilot
        [HttpGet("GetAverageRatingByPilot/{pilotName}")]
        public async Task<ActionResult<double>> GetAverageRatingByPilot(string pilotName)
        {
            try
            {
                var timestamp = new DateTimeOffset(DateTime.UtcNow);
                var logInfo = new { Operation = "Get", Timestamp = timestamp };

                _logger.LogInformation("Get called {@Loginfo} ", logInfo);

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
                        // Calculate the average rating for the pilot
                        double averageRating = await _context.Ratings
                            .Where(r => r.RateeID == pilotId)
                            .Select(r => r.RatingValue)
                            .AverageAsync();

                         return averageRating;
                    }
                    else
                    {
                        return NotFound("Pilot not found.");
                    }
                }
                else
                {
                    return NotFound("Pilot not found with that name.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred processing your request.");
            }
        }

        // 6. Languages Spoken by a Cabin Crew Member
        [HttpGet("GetLanguagesByCrewMember/{crewMemberName}")]
        public async Task<ActionResult<List<string>>> GetLanguagesByCrewMember(string crewMemberName)
        {
            try
            {
                var timestamp = new DateTimeOffset(DateTime.UtcNow);
                var logInfo = new { Operation = "Get", Timestamp = timestamp };

                _logger.LogInformation("Get called {@Loginfo} ", logInfo);

                var crewMemberLanguages = await _context.CabinMembers
                    .Include(cm => cm.Employee)
                    .Include(cm => cm.Languages)
                    .Where(cm => cm.Employee.FirstName + " " + cm.Employee.LastName == crewMemberName)
                    .SelectMany(cm => cm.Languages)
                    .Select(l => l.Language)
                    .ToListAsync();

                return crewMemberLanguages;
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred processing your request.");
            }
        }

        //// 7. Pilot with Highest Average Rating
        //[HttpGet("GetPilotWithHighestAverageRating")]
        //public async Task<ActionResult<string>> GetPilotWithHighestAverageRating()
        //{
        //    try
        //    {
        //        _logger.LogInformation("Request to get the pilot with the highest average rating");

        //        var highestRatedPilot = await _context.Ratings
        //            .GroupBy(r => r.RateeID)
        //            .Select(group => new
        //            {
        //                PilotID = group.Key,
        //                AverageRating = group.Average(r => r.RatingValue)
        //            })
        //            .OrderByDescending(r => r.AverageRating)
        //            .Select(r => r.PilotID)
        //            .FirstOrDefaultAsync();

        //        if (highestRatedPilot.HasValue)
        //        {
        //            var pilotName = await _context.Pilots
        //                .Where(p => p.PilotID == highestRatedPilot)
        //                .Select(p => p.Employee.FirstName + " " + p.Employee.LastName)
        //                .FirstOrDefaultAsync();

        //            _logger.LogInformation("Pilot with the highest average rating retrieved: {PilotName}", pilotName);
        //            return pilotName;
        //        }
        //        else
        //        {
        //            _logger.LogWarning("No pilot found with ratings");
        //            return NotFound("No pilot found with ratings.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error occurred while getting the pilot with the highest average rating");
        //        return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred processing your request.");
        //    }
        //}
    }
}
