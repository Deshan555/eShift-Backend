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
    public class LorryController : ControllerBase
    {
        private readonly EShiftDbContext _context;
        private readonly ILogger<LorryController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LorryController(EShiftDbContext context, ILogger<LorryController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetTraceId() => _httpContextAccessor.HttpContext?.TraceIdentifier ?? System.Guid.NewGuid().ToString();

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<Lorry>>>> GetAll()
        {
            var data = await _context.Lorries.ToListAsync();
            var response = new ApiResponse<IEnumerable<Lorry>>
            {
                Status = StatusCodes.Status200OK,
                Message = "Lorries fetched successfully.",
                TraceId = GetTraceId(),
                Data = data
            };
            _logger.LogInformation("Fetched all lorries. TraceId: {TraceId}", response.TraceId);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Lorry>>> Get(int id)
        {
            var entity = await _context.Lorries.FindAsync(id);
            var traceId = GetTraceId();
            if (entity == null)
            {
                _logger.LogWarning("Lorry not found. Id: {Id}, TraceId: {TraceId}", id, traceId);
                return NotFound(new ApiResponse<Lorry> { Status = StatusCodes.Status404NotFound, Message = "Lorry not found.", TraceId = traceId, Data = null });
            }
            _logger.LogInformation("Fetched lorry. Id: {Id}, TraceId: {TraceId}", id, traceId);
            return Ok(new ApiResponse<Lorry> { Status = StatusCodes.Status200OK, Message = "Lorry fetched successfully.", TraceId = traceId, Data = entity });
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<Lorry>>> Post(Lorry entity)
        {
            _context.Lorries.Add(entity);
            await _context.SaveChangesAsync();
            var traceId = GetTraceId();
            _logger.LogInformation("Lorry created. Id: {Id}, TraceId: {TraceId}", entity.LorryId, traceId);
            var response = new ApiResponse<Lorry> { Status = StatusCodes.Status201Created, Message = "Lorry created successfully.", TraceId = traceId, Data = entity };
            return CreatedAtAction(nameof(Get), new { id = entity.LorryId }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Lorry entity)
        {
            var traceId = GetTraceId();
            if (id != entity.LorryId)
            {
                _logger.LogWarning("Lorry update failed. Id mismatch. TraceId: {TraceId}", traceId);
                return BadRequest(new ApiResponse<Lorry> { Status = StatusCodes.Status400BadRequest, Message = "Lorry ID mismatch.", TraceId = traceId, Data = null });
            }
            _context.Entry(entity).State = EntityState.Modified;
            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Lorries.Any(e => e.LorryId == id))
                {
                    _logger.LogWarning("Lorry not found for update. Id: {Id}, TraceId: {TraceId}", id, traceId);
                    return NotFound(new ApiResponse<Lorry> { Status = StatusCodes.Status404NotFound, Message = "Lorry not found.", TraceId = traceId, Data = null });
                }
                else { throw; }
            }
            _logger.LogInformation("Lorry updated. Id: {Id}, TraceId: {TraceId}", id, traceId);
            return Ok(new ApiResponse<Lorry> { Status = StatusCodes.Status200OK, Message = "Lorry updated successfully.", TraceId = traceId, Data = entity });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.Lorries.FindAsync(id);
            var traceId = GetTraceId();
            if (entity == null)
            {
                _logger.LogWarning("Lorry not found for delete. Id: {Id}, TraceId: {TraceId}", id, traceId);
                return NotFound(new ApiResponse<Lorry> { Status = StatusCodes.Status404NotFound, Message = "Lorry not found.", TraceId = traceId, Data = null });
            }
            _context.Lorries.Remove(entity);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Lorry deleted. Id: {Id}, TraceId: {TraceId}", id, traceId);
            return Ok(new ApiResponse<Lorry> { Status = StatusCodes.Status200OK, Message = "Lorry deleted successfully.", TraceId = traceId, Data = entity });
        }
    }
}
