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

        [HttpGet("GetCrewMembersForAircraftAtAirport/{aircraftType}/{airport}")]
        public async Task<ActionResult<List<string>>> GetCrewMembersForAircraftAtAirport(string aircraftType, string airport)
        {
            try
            {
                _logger.LogInformation("Request to get crew members for {AircraftType} at {Airport}", aircraftType, airport);

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

                _logger.LogInformation("Crew members retrieved for {AircraftType} at {Airport}", aircraftType, airport);
                return Ok(crewMembers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting crew members for {AircraftType} at {Airport}", aircraftType, airport);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred processing your request.");
            }
        }

        [HttpGet("GetCanceledFlightsCount")]
        public async Task<ActionResult<int>> GetCanceledFlightsCount()
        {
            try
            {
                _logger.LogInformation("Request to get the count of canceled flights");

                int canceledFlightsCount = await _context.Flights
                    .CountAsync(f => f.State == "Canceled");

                _logger.LogInformation("Count of canceled flights retrieved: {Count}", canceledFlightsCount);
                return canceledFlightsCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting the count of canceled flights");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred processing your request.");
            }
        }

        [HttpGet("GetFlightScheduleByEmployee/{employeeName}")]
        public async Task<ActionResult<IEnumerable<EmployeeFlightSchedule>>> GetFlightScheduleByEmployee(string employeeName)
        {
            try
            {
                _logger.LogInformation("Request to get flight schedule by employee for {EmployeeName}", employeeName);

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

                _logger.LogInformation("Flight schedule retrieved for {EmployeeName}", employeeName);
                return flightSchedules;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting flight schedule for {EmployeeName}", employeeName);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred processing your request.");
            }
        }

        // 5. Average Rating Given by a Pilot
        [HttpGet("GetAverageRatingByPilot/{pilotName}")]
        public async Task<ActionResult<double>> GetAverageRatingByPilot(string pilotName)
        {
            try
            {
                _logger.LogInformation("Request to get the average rating for pilot: {PilotName}", pilotName);

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

                        _logger.LogInformation("Average rating retrieved for pilot: {PilotName}, Average Rating: {AverageRating}", pilotName, averageRating);
                        return averageRating;
                    }
                    else
                    {
                        _logger.LogWarning("Pilot not found for name: {PilotName}", pilotName);
                        return NotFound("Pilot not found.");
                    }
                }
                else
                {
                    _logger.LogWarning("Pilot not found with name: {PilotName}", pilotName);
                    return NotFound("Pilot not found with that name.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting average rating for pilot: {PilotName}", pilotName);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred processing your request.");
            }
        }

        // 6. Languages Spoken by a Cabin Crew Member
        [HttpGet("GetLanguagesByCrewMember/{crewMemberName}")]
        public async Task<ActionResult<List<string>>> GetLanguagesByCrewMember(string crewMemberName)
        {
            try
            {
                _logger.LogInformation("Request to get languages spoken by cabin crew member: {CrewMemberName}", crewMemberName);

                var crewMemberLanguages = await _context.CabinMembers
                    .Include(cm => cm.Employee)
                    .Include(cm => cm.Languages)
                    .Where(cm => cm.Employee.FirstName + " " + cm.Employee.LastName == crewMemberName)
                    .SelectMany(cm => cm.Languages)
                    .Select(l => l.Language)
                    .ToListAsync();

                _logger.LogInformation("Languages retrieved for cabin crew member: {CrewMemberName}", crewMemberName);
                return crewMemberLanguages;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting languages for cabin crew member: {CrewMemberName}", crewMemberName);
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
