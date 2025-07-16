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
    public class JobStopController : ControllerBase
    {
        private readonly EShiftDbContext _context;
        private readonly ILogger<JobStopController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JobStopController(EShiftDbContext context, ILogger<JobStopController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetTraceId() => _httpContextAccessor.HttpContext?.TraceIdentifier ?? System.Guid.NewGuid().ToString();

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<JobStop>>>> GetAll()
        {
            var data = await _context.JobStops.ToListAsync();
            var response = new ApiResponse<IEnumerable<JobStop>>
            {
                Status = StatusCodes.Status200OK,
                Message = "JobStops fetched successfully.",
                TraceId = GetTraceId(),
                Data = data
            };
            _logger.LogInformation("Fetched all job stops. TraceId: {TraceId}", response.TraceId);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<JobStop>>> Get(int id)
        {
            var entity = await _context.JobStops.FindAsync(id);
            var traceId = GetTraceId();
            if (entity == null)
            {
                _logger.LogWarning("JobStop not found. Id: {Id}, TraceId: {TraceId}", id, traceId);
                return NotFound(new ApiResponse<JobStop> { Status = StatusCodes.Status404NotFound, Message = "JobStop not found.", TraceId = traceId, Data = null });
            }
            _logger.LogInformation("Fetched job stop. Id: {Id}, TraceId: {TraceId}", id, traceId);
            return Ok(new ApiResponse<JobStop> { Status = StatusCodes.Status200OK, Message = "JobStop fetched successfully.", TraceId = traceId, Data = entity });
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<JobStop>>> Post(JobStop entity)
        {
            _context.JobStops.Add(entity);
            await _context.SaveChangesAsync();
            var traceId = GetTraceId();
            _logger.LogInformation("JobStop created. Id: {Id}, TraceId: {TraceId}", entity.StopId, traceId);
            var response = new ApiResponse<JobStop> { Status = StatusCodes.Status201Created, Message = "JobStop created successfully.", TraceId = traceId, Data = entity };
            return CreatedAtAction(nameof(Get), new { id = entity.StopId }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, JobStop entity)
        {
            var traceId = GetTraceId();
            if (id != entity.StopId)
            {
                _logger.LogWarning("JobStop update failed. Id mismatch. TraceId: {TraceId}", traceId);
                return BadRequest(new ApiResponse<JobStop> { Status = StatusCodes.Status400BadRequest, Message = "JobStop ID mismatch.", TraceId = traceId, Data = null });
            }
            _context.Entry(entity).State = EntityState.Modified;
            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.JobStops.Any(e => e.StopId == id))
                {
                    _logger.LogWarning("JobStop not found for update. Id: {Id}, TraceId: {TraceId}", id, traceId);
                    return NotFound(new ApiResponse<JobStop> { Status = StatusCodes.Status404NotFound, Message = "JobStop not found.", TraceId = traceId, Data = null });
                }
                else { throw; }
            }
            _logger.LogInformation("JobStop updated. Id: {Id}, TraceId: {TraceId}", id, traceId);
            return Ok(new ApiResponse<JobStop> { Status = StatusCodes.Status200OK, Message = "JobStop updated successfully.", TraceId = traceId, Data = entity });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.JobStops.FindAsync(id);
            var traceId = GetTraceId();
            if (entity == null)
            {
                _logger.LogWarning("JobStop not found for delete. Id: {Id}, TraceId: {TraceId}", id, traceId);
                return NotFound(new ApiResponse<JobStop> { Status = StatusCodes.Status404NotFound, Message = "JobStop not found.", TraceId = traceId, Data = null });
            }
            _context.JobStops.Remove(entity);
            await _context.SaveChangesAsync();
            _logger.LogInformation("JobStop deleted. Id: {Id}, TraceId: {TraceId}", id, traceId);
            return Ok(new ApiResponse<JobStop> { Status = StatusCodes.Status200OK, Message = "JobStop deleted successfully.", TraceId = traceId, Data = entity });
        }

[HttpGet("by-job/{jobId}")]
public async Task<ActionResult<ApiResponse<IEnumerable<JobStop>>>> GetByJobId(int jobId)
{
    var data = await _context.JobStops
        .Include(js => js.City)
        .Where(js => js.JobId == jobId)
        .ToListAsync();
    var traceId = GetTraceId();
    _logger.LogInformation("Fetched JobStops for JobId: {JobId}, TraceId: {TraceId}", jobId, traceId);
    return Ok(new ApiResponse<IEnumerable<JobStop>>
    {
        Status = StatusCodes.Status200OK,
        Message = data.Count == 0 ? "No JobStops found for the given JobId." : "JobStops fetched successfully.",
        TraceId = traceId,
        Data = data
    });
}


    }
}
