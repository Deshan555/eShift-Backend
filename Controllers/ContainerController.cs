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
    public class ContainerController : ControllerBase
    {
        private readonly EShiftDbContext _context;
        private readonly ILogger<ContainerController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ContainerController(EShiftDbContext context, ILogger<ContainerController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetTraceId() => _httpContextAccessor.HttpContext?.TraceIdentifier ?? System.Guid.NewGuid().ToString();

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<Container>>>> GetAll()
        {
            var data = await _context.Containers.ToListAsync();
            var response = new ApiResponse<IEnumerable<Container>>
            {
                Status = StatusCodes.Status200OK,
                Message = "Containers fetched successfully.",
                TraceId = GetTraceId(),
                Data = data
            };
            _logger.LogInformation("Fetched all containers. TraceId: {TraceId}", response.TraceId);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Container>>> Get(int id)
        {
            var entity = await _context.Containers.FindAsync(id);
            var traceId = GetTraceId();
            if (entity == null)
            {
                _logger.LogWarning("Container not found. Id: {Id}, TraceId: {TraceId}", id, traceId);
                return NotFound(new ApiResponse<Container> { Status = StatusCodes.Status404NotFound, Message = "Container not found.", TraceId = traceId, Data = null });
            }
            _logger.LogInformation("Fetched container. Id: {Id}, TraceId: {TraceId}", id, traceId);
            return Ok(new ApiResponse<Container> { Status = StatusCodes.Status200OK, Message = "Container fetched successfully.", TraceId = traceId, Data = entity });
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<Container>>> Post(Container entity)
        {
            _context.Containers.Add(entity);
            await _context.SaveChangesAsync();
            var traceId = GetTraceId();
            _logger.LogInformation("Container created. Id: {Id}, TraceId: {TraceId}", entity.ContainerId, traceId);
            var response = new ApiResponse<Container> { Status = StatusCodes.Status201Created, Message = "Container created successfully.", TraceId = traceId, Data = entity };
            return CreatedAtAction(nameof(Get), new { id = entity.ContainerId }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Container entity)
        {
            var traceId = GetTraceId();
            if (id != entity.ContainerId)
            {
                _logger.LogWarning("Container update failed. Id mismatch. TraceId: {TraceId}", traceId);
                return BadRequest(new ApiResponse<Container> { Status = StatusCodes.Status400BadRequest, Message = "Container ID mismatch.", TraceId = traceId, Data = null });
            }
            _context.Entry(entity).State = EntityState.Modified;
            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Containers.Any(e => e.ContainerId == id))
                {
                    _logger.LogWarning("Container not found for update. Id: {Id}, TraceId: {TraceId}", id, traceId);
                    return NotFound(new ApiResponse<Container> { Status = StatusCodes.Status404NotFound, Message = "Container not found.", TraceId = traceId, Data = null });
                }
                else { throw; }
            }
            _logger.LogInformation("Container updated. Id: {Id}, TraceId: {TraceId}", id, traceId);
            return Ok(new ApiResponse<Container> { Status = StatusCodes.Status200OK, Message = "Container updated successfully.", TraceId = traceId, Data = entity });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.Containers.FindAsync(id);
            var traceId = GetTraceId();
            if (entity == null)
            {
                _logger.LogWarning("Container not found for delete. Id: {Id}, TraceId: {TraceId}", id, traceId);
                return NotFound(new ApiResponse<Container> { Status = StatusCodes.Status404NotFound, Message = "Container not found.", TraceId = traceId, Data = null });
            }
            _context.Containers.Remove(entity);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Container deleted. Id: {Id}, TraceId: {TraceId}", id, traceId);
            return Ok(new ApiResponse<Container> { Status = StatusCodes.Status200OK, Message = "Container deleted successfully.", TraceId = traceId, Data = entity });
        }
    }
}
