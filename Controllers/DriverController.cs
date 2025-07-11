using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using eShift.Data;
using eShift.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eShift.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriverController : ControllerBase
    {
        private readonly EShiftDbContext _context;
        private readonly ILogger<DriverController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DriverController(EShiftDbContext context, ILogger<DriverController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetTraceId() => _httpContextAccessor.HttpContext?.TraceIdentifier ?? System.Guid.NewGuid().ToString();

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<Driver>>>> GetAll()
        {
            var data = await _context.Drivers.ToListAsync();
            var response = new ApiResponse<IEnumerable<Driver>>
            {
                Status = StatusCodes.Status200OK,
                Message = "Drivers fetched successfully.",
                TraceId = GetTraceId(),
                Data = data
            };
            _logger.LogInformation("Fetched all drivers. TraceId: {TraceId}", response.TraceId);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Driver>>> Get(int id)
        {
            var entity = await _context.Drivers.FindAsync(id);
            var traceId = GetTraceId();
            if (entity == null)
            {
                _logger.LogWarning("Driver not found. Id: {Id}, TraceId: {TraceId}", id, traceId);
                return NotFound(new ApiResponse<Driver> { Status = StatusCodes.Status404NotFound, Message = "Driver not found.", TraceId = traceId, Data = null });
            }
            _logger.LogInformation("Fetched driver. Id: {Id}, TraceId: {TraceId}", id, traceId);
            return Ok(new ApiResponse<Driver> { Status = StatusCodes.Status200OK, Message = "Driver fetched successfully.", TraceId = traceId, Data = entity });
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<Driver>>> Post(Driver entity)
        {
            _context.Drivers.Add(entity);
            await _context.SaveChangesAsync();
            var traceId = GetTraceId();
            _logger.LogInformation("Driver created. Id: {Id}, TraceId: {TraceId}", entity.DriverId, traceId);
            var response = new ApiResponse<Driver> { Status = StatusCodes.Status201Created, Message = "Driver created successfully.", TraceId = traceId, Data = entity };
            return CreatedAtAction(nameof(Get), new { id = entity.DriverId }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Driver entity)
        {
            var traceId = GetTraceId();
            if (id != entity.DriverId)
            {
                _logger.LogWarning("Driver update failed. Id mismatch. TraceId: {TraceId}", traceId);
                return BadRequest(new ApiResponse<Driver> { Status = StatusCodes.Status400BadRequest, Message = "Driver ID mismatch.", TraceId = traceId, Data = null });
            }
            _context.Entry(entity).State = EntityState.Modified;
            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Drivers.Any(e => e.DriverId == id))
                {
                    _logger.LogWarning("Driver not found for update. Id: {Id}, TraceId: {TraceId}", id, traceId);
                    return NotFound(new ApiResponse<Driver> { Status = StatusCodes.Status404NotFound, Message = "Driver not found.", TraceId = traceId, Data = null });
                }
                else { throw; }
            }
            _logger.LogInformation("Driver updated. Id: {Id}, TraceId: {TraceId}", id, traceId);
            return Ok(new ApiResponse<Driver> { Status = StatusCodes.Status200OK, Message = "Driver updated successfully.", TraceId = traceId, Data = entity });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.Drivers.FindAsync(id);
            var traceId = GetTraceId();
            if (entity == null)
            {
                _logger.LogWarning("Driver not found for delete. Id: {Id}, TraceId: {TraceId}", id, traceId);
                return NotFound(new ApiResponse<Driver> { Status = StatusCodes.Status404NotFound, Message = "Driver not found.", TraceId = traceId, Data = null });
            }
            _context.Drivers.Remove(entity);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Driver deleted. Id: {Id}, TraceId: {TraceId}", id, traceId);
            return Ok(new ApiResponse<Driver> { Status = StatusCodes.Status200OK, Message = "Driver deleted successfully.", TraceId = traceId, Data = entity });
        }
    }
}
