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
    public class CityController : ControllerBase
    {
        private readonly EShiftDbContext _context;
        private readonly ILogger<CityController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CityController(EShiftDbContext context, ILogger<CityController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetTraceId() => _httpContextAccessor.HttpContext?.TraceIdentifier ?? System.Guid.NewGuid().ToString();

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<City>>>> GetAll()
        {
            var data = await _context.Cities.ToListAsync();
            var response = new ApiResponse<IEnumerable<City>>
            {
                Status = StatusCodes.Status200OK,
                Message = "Cities fetched successfully.",
                TraceId = GetTraceId(),
                Data = data
            };
            _logger.LogInformation("Fetched all cities. TraceId: {TraceId}", response.TraceId);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<City>>> Get(int id)
        {
            var entity = await _context.Cities.FindAsync(id);
            var traceId = GetTraceId();
            if (entity == null)
            {
                _logger.LogWarning("City not found. Id: {Id}, TraceId: {TraceId}", id, traceId);
                return NotFound(new ApiResponse<City> { Status = StatusCodes.Status404NotFound, Message = "City not found.", TraceId = traceId, Data = null });
            }
            _logger.LogInformation("Fetched city. Id: {Id}, TraceId: {TraceId}", id, traceId);
            return Ok(new ApiResponse<City> { Status = StatusCodes.Status200OK, Message = "City fetched successfully.", TraceId = traceId, Data = entity });
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<City>>> Post(City entity)
        {
            _context.Cities.Add(entity);
            await _context.SaveChangesAsync();
            var traceId = GetTraceId();
            _logger.LogInformation("City created. Id: {Id}, TraceId: {TraceId}", entity.CityId, traceId);
            var response = new ApiResponse<City> { Status = StatusCodes.Status201Created, Message = "City created successfully.", TraceId = traceId, Data = entity };
            return CreatedAtAction(nameof(Get), new { id = entity.CityId }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, City entity)
        {
            var traceId = GetTraceId();
            if (id != entity.CityId)
            {
                _logger.LogWarning("City update failed. Id mismatch. TraceId: {TraceId}", traceId);
                return BadRequest(new ApiResponse<City> { Status = StatusCodes.Status400BadRequest, Message = "City ID mismatch.", TraceId = traceId, Data = null });
            }
            _context.Entry(entity).State = EntityState.Modified;
            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Cities.Any(e => e.CityId == id))
                {
                    _logger.LogWarning("City not found for update. Id: {Id}, TraceId: {TraceId}", id, traceId);
                    return NotFound(new ApiResponse<City> { Status = StatusCodes.Status404NotFound, Message = "City not found.", TraceId = traceId, Data = null });
                }
                else { throw; }
            }
            _logger.LogInformation("City updated. Id: {Id}, TraceId: {TraceId}", id, traceId);
            return Ok(new ApiResponse<City> { Status = StatusCodes.Status200OK, Message = "City updated successfully.", TraceId = traceId, Data = entity });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.Cities.FindAsync(id);
            var traceId = GetTraceId();
            if (entity == null)
            {
                _logger.LogWarning("City not found for delete. Id: {Id}, TraceId: {TraceId}", id, traceId);
                return NotFound(new ApiResponse<City> { Status = StatusCodes.Status404NotFound, Message = "City not found.", TraceId = traceId, Data = null });
            }
            _context.Cities.Remove(entity);
            await _context.SaveChangesAsync();
            _logger.LogInformation("City deleted. Id: {Id}, TraceId: {TraceId}", id, traceId);
            return Ok(new ApiResponse<City> { Status = StatusCodes.Status200OK, Message = "City deleted successfully.", TraceId = traceId, Data = entity });
        }
    }
}
