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
    public class AssistantController : ControllerBase
    {
        private readonly EShiftDbContext _context;
        private readonly ILogger<AssistantController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AssistantController(EShiftDbContext context, ILogger<AssistantController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetTraceId() => _httpContextAccessor.HttpContext?.TraceIdentifier ?? System.Guid.NewGuid().ToString();

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<Assistant>>>> GetAll()
        {
            var data = await _context.Assistants.ToListAsync();
            var response = new ApiResponse<IEnumerable<Assistant>>
            {
                Status = StatusCodes.Status200OK,
                Message = "Assistants fetched successfully.",
                TraceId = GetTraceId(),
                Data = data
            };
            _logger.LogInformation("Fetched all assistants. TraceId: {TraceId}", response.TraceId);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Assistant>>> Get(int id)
        {
            var entity = await _context.Assistants.FindAsync(id);
            var traceId = GetTraceId();
            if (entity == null)
            {
                _logger.LogWarning("Assistant not found. Id: {Id}, TraceId: {TraceId}", id, traceId);
                return NotFound(new ApiResponse<Assistant> { Status = StatusCodes.Status404NotFound, Message = "Assistant not found.", TraceId = traceId, Data = null });
            }
            _logger.LogInformation("Fetched assistant. Id: {Id}, TraceId: {TraceId}", id, traceId);
            return Ok(new ApiResponse<Assistant> { Status = StatusCodes.Status200OK, Message = "Assistant fetched successfully.", TraceId = traceId, Data = entity });
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<Assistant>>> Post(Assistant entity)
        {
            _context.Assistants.Add(entity);
            await _context.SaveChangesAsync();
            var traceId = GetTraceId();
            _logger.LogInformation("Assistant created. Id: {Id}, TraceId: {TraceId}", entity.AssistantId, traceId);
            var response = new ApiResponse<Assistant> { Status = StatusCodes.Status201Created, Message = "Assistant created successfully.", TraceId = traceId, Data = entity };
            return CreatedAtAction(nameof(Get), new { id = entity.AssistantId }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Assistant entity)
        {
            var traceId = GetTraceId();
            if (id != entity.AssistantId)
            {
                _logger.LogWarning("Assistant update failed. Id mismatch. TraceId: {TraceId}", traceId);
                return BadRequest(new ApiResponse<Assistant> { Status = StatusCodes.Status400BadRequest, Message = "Assistant ID mismatch.", TraceId = traceId, Data = null });
            }
            _context.Entry(entity).State = EntityState.Modified;
            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Assistants.Any(e => e.AssistantId == id))
                {
                    _logger.LogWarning("Assistant not found for update. Id: {Id}, TraceId: {TraceId}", id, traceId);
                    return NotFound(new ApiResponse<Assistant> { Status = StatusCodes.Status404NotFound, Message = "Assistant not found.", TraceId = traceId, Data = null });
                }
                else { throw; }
            }
            _logger.LogInformation("Assistant updated. Id: {Id}, TraceId: {TraceId}", id, traceId);
            return Ok(new ApiResponse<Assistant> { Status = StatusCodes.Status200OK, Message = "Assistant updated successfully.", TraceId = traceId, Data = entity });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.Assistants.FindAsync(id);
            var traceId = GetTraceId();
            if (entity == null)
            {
                _logger.LogWarning("Assistant not found for delete. Id: {Id}, TraceId: {TraceId}", id, traceId);
                return NotFound(new ApiResponse<Assistant> { Status = StatusCodes.Status404NotFound, Message = "Assistant not found.", TraceId = traceId, Data = null });
            }
            _context.Assistants.Remove(entity);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Assistant deleted. Id: {Id}, TraceId: {TraceId}", id, traceId);
            return Ok(new ApiResponse<Assistant> { Status = StatusCodes.Status200OK, Message = "Assistant deleted successfully.", TraceId = traceId, Data = entity });
        }
    }
}
