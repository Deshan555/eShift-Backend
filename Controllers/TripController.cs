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
    public class TripController : ControllerBase
    {
        private readonly EShiftDbContext _context;
        private readonly ILogger<TripController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TripController(EShiftDbContext context, ILogger<TripController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetTraceId() => _httpContextAccessor.HttpContext?.TraceIdentifier ?? System.Guid.NewGuid().ToString();

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<Trip>>>> GetAll()
        {
            var data = await _context.Trips.ToListAsync();
            var response = new ApiResponse<IEnumerable<Trip>>
            {
                Status = StatusCodes.Status200OK,
                Message = "Trips fetched successfully.",
                TraceId = GetTraceId(),
                Data = data
            };
            _logger.LogInformation("Fetched all trips. TraceId: {TraceId}", response.TraceId);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Trip>>> Get(int id)
        {
            var entity = await _context.Trips.FindAsync(id);
            var traceId = GetTraceId();
            if (entity == null)
            {
                _logger.LogWarning("Trip not found. Id: {Id}, TraceId: {TraceId}", id, traceId);
                return NotFound(new ApiResponse<Trip> { Status = StatusCodes.Status404NotFound, Message = "Trip not found.", TraceId = traceId, Data = null });
            }
            _logger.LogInformation("Fetched trip. Id: {Id}, TraceId: {TraceId}", id, traceId);
            return Ok(new ApiResponse<Trip> { Status = StatusCodes.Status200OK, Message = "Trip fetched successfully.", TraceId = traceId, Data = entity });
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<Trip>>> Post(Trip entity)
        {
            _context.Trips.Add(entity);
            await _context.SaveChangesAsync();
            var traceId = GetTraceId();
            _logger.LogInformation("Trip created. Id: {Id}, TraceId: {TraceId}", entity.TripId, traceId);
            var response = new ApiResponse<Trip> { Status = StatusCodes.Status201Created, Message = "Trip created successfully.", TraceId = traceId, Data = entity };
            return CreatedAtAction(nameof(Get), new { id = entity.TripId }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Trip entity)
        {
            var traceId = GetTraceId();
            if (id != entity.TripId)
            {
                _logger.LogWarning("Trip update failed. Id mismatch. TraceId: {TraceId}", traceId);
                return BadRequest(new ApiResponse<Trip> { Status = StatusCodes.Status400BadRequest, Message = "Trip ID mismatch.", TraceId = traceId, Data = null });
            }
            _context.Entry(entity).State = EntityState.Modified;
            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Trips.Any(e => e.TripId == id))
                {
                    _logger.LogWarning("Trip not found for update. Id: {Id}, TraceId: {TraceId}", id, traceId);
                    return NotFound(new ApiResponse<Trip> { Status = StatusCodes.Status404NotFound, Message = "Trip not found.", TraceId = traceId, Data = null });
                }
                else { throw; }
            }
            _logger.LogInformation("Trip updated. Id: {Id}, TraceId: {TraceId}", id, traceId);
            return Ok(new ApiResponse<Trip> { Status = StatusCodes.Status200OK, Message = "Trip updated successfully.", TraceId = traceId, Data = entity });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.Trips.FindAsync(id);
            var traceId = GetTraceId();
            if (entity == null)
            {
                _logger.LogWarning("Trip not found for delete. Id: {Id}, TraceId: {TraceId}", id, traceId);
                return NotFound(new ApiResponse<Trip> { Status = StatusCodes.Status404NotFound, Message = "Trip not found.", TraceId = traceId, Data = null });
            }
            _context.Trips.Remove(entity);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Trip deleted. Id: {Id}, TraceId: {TraceId}", id, traceId);
            return Ok(new ApiResponse<Trip> { Status = StatusCodes.Status200OK, Message = "Trip deleted successfully.", TraceId = traceId, Data = entity });
        }
    }
}
