using Microsoft.AspNetCore.Mvc;
using DAir.Models;
using DAir.Services;

namespace DAir.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogController : ControllerBase
    {
        private readonly LogService _logService;

        public LogController(LogService logService)
        {
            _logService = logService;
        }

        [HttpGet]
        public async Task<ActionResult<List<LogEntry>>> GetAllLogs()
        {
            return await _logService.GetAllLogsAsync();
        }

        [HttpGet("{operation}")]
        public async Task<ActionResult<List<LogEntry>>> GetByOperation(string operation)
        {
            Console.WriteLine("operation = " + operation);
            return await _logService.GetByOperationAsync(operation);
        }
    }

}
