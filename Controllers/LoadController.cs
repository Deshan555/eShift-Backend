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
    public class LoadController : ControllerBase
    {
        private readonly EShiftDbContext _context;
        private readonly ILogger<LoadController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoadController(EShiftDbContext context, ILogger<LoadController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetTraceId() => _httpContextAccessor.HttpContext?.TraceIdentifier ?? System.Guid.NewGuid().ToString();

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<Load>>>> GetAll()
        {
            var data = await _context.Loads.ToListAsync();
            var response = new ApiResponse<IEnumerable<Load>>
            {
                Status = StatusCodes.Status200OK,
                Message = "Loads fetched successfully.",
                TraceId = GetTraceId(),
                Data = data
            };
            _logger.LogInformation("Fetched all loads. TraceId: {TraceId}", response.TraceId);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Load>>> Get(int id)
        {
            var entity = await _context.Loads.FindAsync(id);
            var traceId = GetTraceId();
            if (entity == null)
            {
                _logger.LogWarning("Load not found. Id: {Id}, TraceId: {TraceId}", id, traceId);
                return NotFound(new ApiResponse<Load> { Status = StatusCodes.Status404NotFound, Message = "Load not found.", TraceId = traceId, Data = null });
            }
            _logger.LogInformation("Fetched load. Id: {Id}, TraceId: {TraceId}", id, traceId);
            return Ok(new ApiResponse<Load> { Status = StatusCodes.Status200OK, Message = "Load fetched successfully.", TraceId = traceId, Data = entity });
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<Load>>> Post(Load entity)
        {
            _context.Loads.Add(entity);
            await _context.SaveChangesAsync();
            var traceId = GetTraceId();
            _logger.LogInformation("Load created. Id: {Id}, TraceId: {TraceId}", entity.LoadId, traceId);
            var response = new ApiResponse<Load> { Status = StatusCodes.Status201Created, Message = "Load created successfully.", TraceId = traceId, Data = entity };
            return CreatedAtAction(nameof(Get), new { id = entity.LoadId }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Load entity)
        {
            var traceId = GetTraceId();
            if (id != entity.LoadId)
            {
                _logger.LogWarning("Load update failed. Id mismatch. TraceId: {TraceId}", traceId);
                return BadRequest(new ApiResponse<Load> { Status = StatusCodes.Status400BadRequest, Message = "Load ID mismatch.", TraceId = traceId, Data = null });
            }
            _context.Entry(entity).State = EntityState.Modified;
            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Loads.Any(e => e.LoadId == id))
                {
                    _logger.LogWarning("Load not found for update. Id: {Id}, TraceId: {TraceId}", id, traceId);
                    return NotFound(new ApiResponse<Load> { Status = StatusCodes.Status404NotFound, Message = "Load not found.", TraceId = traceId, Data = null });
                }
                else { throw; }
            }
            _logger.LogInformation("Load updated. Id: {Id}, TraceId: {TraceId}", id, traceId);
            return Ok(new ApiResponse<Load> { Status = StatusCodes.Status200OK, Message = "Load updated successfully.", TraceId = traceId, Data = entity });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.Loads.FindAsync(id);
            var traceId = GetTraceId();
            if (entity == null)
            {
                _logger.LogWarning("Load not found for delete. Id: {Id}, TraceId: {TraceId}", id, traceId);
                return NotFound(new ApiResponse<Load> { Status = StatusCodes.Status404NotFound, Message = "Load not found.", TraceId = traceId, Data = null });
            }
            _context.Loads.Remove(entity);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Load deleted. Id: {Id}, TraceId: {TraceId}", id, traceId);
            return Ok(new ApiResponse<Load> { Status = StatusCodes.Status200OK, Message = "Load deleted successfully.", TraceId = traceId, Data = entity });
        }
    }
}
