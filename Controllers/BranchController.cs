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
    public class BranchController : ControllerBase
    {
        private readonly EShiftDbContext _context;
        private readonly ILogger<BranchController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BranchController(EShiftDbContext context, ILogger<BranchController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetTraceId() => _httpContextAccessor.HttpContext?.TraceIdentifier ?? System.Guid.NewGuid().ToString();

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<Branch>>>> GetAll()
        {
            var data = await _context.Branches.ToListAsync();
            var response = new ApiResponse<IEnumerable<Branch>>
            {
                Status = StatusCodes.Status200OK,
                Message = "Branches fetched successfully.",
                TraceId = GetTraceId(),
                Data = data
            };
            _logger.LogInformation("Fetched all branches. TraceId: {TraceId}", response.TraceId);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Branch>>> Get(int id)
        {
            var entity = await _context.Branches.FindAsync(id);
            var traceId = GetTraceId();
            if (entity == null)
            {
                _logger.LogWarning("Branch not found. Id: {Id}, TraceId: {TraceId}", id, traceId);
                return NotFound(new ApiResponse<Branch> { Status = StatusCodes.Status404NotFound, Message = "Branch not found.", TraceId = traceId, Data = null });
            }
            _logger.LogInformation("Fetched branch. Id: {Id}, TraceId: {TraceId}", id, traceId);
            return Ok(new ApiResponse<Branch> { Status = StatusCodes.Status200OK, Message = "Branch fetched successfully.", TraceId = traceId, Data = entity });
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<Branch>>> Post(Branch entity)
        {
            _context.Branches.Add(entity);
            await _context.SaveChangesAsync();
            var traceId = GetTraceId();
            _logger.LogInformation("Branch created. Id: {Id}, TraceId: {TraceId}", entity.BranchId, traceId);
            var response = new ApiResponse<Branch> { Status = StatusCodes.Status201Created, Message = "Branch created successfully.", TraceId = traceId, Data = entity };
            return CreatedAtAction(nameof(Get), new { id = entity.BranchId }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Branch entity)
        {
            var traceId = GetTraceId();
            if (id != entity.BranchId)
            {
                _logger.LogWarning("Branch update failed. Id mismatch. TraceId: {TraceId}", traceId);
                return BadRequest(new ApiResponse<Branch> { Status = StatusCodes.Status400BadRequest, Message = "Branch ID mismatch.", TraceId = traceId, Data = null });
            }
            _context.Entry(entity).State = EntityState.Modified;
            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Branches.Any(e => e.BranchId == id))
                {
                    _logger.LogWarning("Branch not found for update. Id: {Id}, TraceId: {TraceId}", id, traceId);
                    return NotFound(new ApiResponse<Branch> { Status = StatusCodes.Status404NotFound, Message = "Branch not found.", TraceId = traceId, Data = null });
                }
                else { throw; }
            }
            _logger.LogInformation("Branch updated. Id: {Id}, TraceId: {TraceId}", id, traceId);
            return Ok(new ApiResponse<Branch> { Status = StatusCodes.Status200OK, Message = "Branch updated successfully.", TraceId = traceId, Data = entity });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.Branches.FindAsync(id);
            var traceId = GetTraceId();
            if (entity == null)
            {
                _logger.LogWarning("Branch not found for delete. Id: {Id}, TraceId: {TraceId}", id, traceId);
                return NotFound(new ApiResponse<Branch> { Status = StatusCodes.Status404NotFound, Message = "Branch not found.", TraceId = traceId, Data = null });
            }
            _context.Branches.Remove(entity);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Branch deleted. Id: {Id}, TraceId: {TraceId}", id, traceId);
            return Ok(new ApiResponse<Branch> { Status = StatusCodes.Status200OK, Message = "Branch deleted successfully.", TraceId = traceId, Data = entity });
        }
    }
}
