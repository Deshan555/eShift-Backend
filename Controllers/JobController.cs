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
    public class JobController : ControllerBase
    {
        private readonly EShiftDbContext _context;
        private readonly ILogger<JobController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JobController(EShiftDbContext context, ILogger<JobController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetTraceId() => _httpContextAccessor.HttpContext?.TraceIdentifier ?? System.Guid.NewGuid().ToString();

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<Job>>>> GetAll()
        {
            var data = await _context.Jobs.ToListAsync();
            var response = new ApiResponse<IEnumerable<Job>>
            {
                Status = StatusCodes.Status200OK,
                Message = "Jobs fetched successfully.",
                TraceId = GetTraceId(),
                Data = data
            };
            _logger.LogInformation("Fetched all jobs. TraceId: {TraceId}", response.TraceId);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Job>>> Get(int id)
        {
            var entity = await _context.Jobs.FindAsync(id);
            var traceId = GetTraceId();
            if (entity == null)
            {
                _logger.LogWarning("Job not found. Id: {Id}, TraceId: {TraceId}", id, traceId);
                return NotFound(new ApiResponse<Job> { Status = StatusCodes.Status404NotFound, Message = "Job not found.", TraceId = traceId, Data = null });
            }
            _logger.LogInformation("Fetched job. Id: {Id}, TraceId: {TraceId}", id, traceId);
            return Ok(new ApiResponse<Job> { Status = StatusCodes.Status200OK, Message = "Job fetched successfully.", TraceId = traceId, Data = entity });
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<Job>>> Post(Job entity)
        {
            _context.Jobs.Add(entity);
            await _context.SaveChangesAsync();

            var trip = new Trip
            {
                JobId = entity.JobId,
                ScheduledDate = entity.Date,
                Status = "PENDING",
                LorryId = null,
                DriverId = null,
                AssistantId = null,
                ContainerId = null
            };
            _context.Trips.Add(trip);
            await _context.SaveChangesAsync();

            var load = new Load
            {
                JobId = entity.JobId,
                TripId = trip.TripId,
                Description = "Auto-generated load for job " + entity.JobId,
                Weight = 0,
                Volume = 0,
                MeaterReadingStart = 0,
                MeaterReadingEnd = 0
            };
            _context.Loads.Add(load);
            await _context.SaveChangesAsync();

            var traceId = GetTraceId();
            _logger.LogInformation("Job, Trip, and Load created. JobId: {JobId}, TripId: {TripId}, LoadId: {LoadId}, TraceId: {TraceId}", entity.JobId, trip.TripId, load.LoadId, traceId);
            var response = new ApiResponse<Job> { Status = StatusCodes.Status201Created, Message = "Job, Trip, and Load created successfully.", TraceId = traceId, Data = entity };
            return CreatedAtAction(nameof(Get), new { id = entity.JobId }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Job entity)
        {
            var traceId = GetTraceId();
            if (id != entity.JobId)
            {
                _logger.LogWarning("Job update failed. Id mismatch. TraceId: {TraceId}", traceId);
                return BadRequest(new ApiResponse<Job> { Status = StatusCodes.Status400BadRequest, Message = "Job ID mismatch.", TraceId = traceId, Data = null });
            }
            _context.Entry(entity).State = EntityState.Modified;
            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Jobs.Any(e => e.JobId == id))
                {
                    _logger.LogWarning("Job not found for update. Id: {Id}, TraceId: {TraceId}", id, traceId);
                    return NotFound(new ApiResponse<Job> { Status = StatusCodes.Status404NotFound, Message = "Job not found.", TraceId = traceId, Data = null });
                }
                else { throw; }
            }
            _logger.LogInformation("Job updated. Id: {Id}, TraceId: {TraceId}", id, traceId);
            return Ok(new ApiResponse<Job> { Status = StatusCodes.Status200OK, Message = "Job updated successfully.", TraceId = traceId, Data = entity });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.Jobs.FindAsync(id);
            var traceId = GetTraceId();
            if (entity == null)
            {
                _logger.LogWarning("Job not found for delete. Id: {Id}, TraceId: {TraceId}", id, traceId);
                return NotFound(new ApiResponse<Job> { Status = StatusCodes.Status404NotFound, Message = "Job not found.", TraceId = traceId, Data = null });
            }
            _context.Jobs.Remove(entity);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Job deleted. Id: {Id}, TraceId: {TraceId}", id, traceId);
            return Ok(new ApiResponse<Job> { Status = StatusCodes.Status200OK, Message = "Job deleted successfully.", TraceId = traceId, Data = entity });
        }

        [HttpGet("details/{jobId}")]
        public async Task<ActionResult<ApiResponse<object>>> GetJobFullDetails(int jobId)
        {
            var job = await _context.Jobs.FindAsync(jobId);
            if (job == null)
            {
                var traceId = GetTraceId();
                _logger.LogWarning("Job not found for details. Id: {Id}, TraceId: {TraceId}", jobId, traceId);
                return NotFound(new ApiResponse<object> { Status = StatusCodes.Status404NotFound, Message = "Job not found.", TraceId = traceId, Data = null });
            }

            var jobStops = await _context.JobStops.Where(js => js.JobId == jobId).ToListAsync();
            var trip = await _context.Trips.FirstOrDefaultAsync(t => t.JobId == jobId);
            var load = await _context.Loads.FirstOrDefaultAsync(l => l.JobId == jobId);
            var customer = await _context.Customers.FindAsync(job.CustomerId);

            var traceIdDetails = GetTraceId();
            var result = new
            {
                Job = job,
                JobStops = jobStops,
                Trip = trip,
                Load = load,
                Customer = customer
            };
            _logger.LogInformation("Fetched job details for Id: {Id}, TraceId: {TraceId}", jobId, traceIdDetails);
            return Ok(new ApiResponse<object>
            {
                Status = StatusCodes.Status200OK,
                Message = "Job details fetched successfully.",
                TraceId = traceIdDetails,
                Data = result
            });
        }

        [HttpGet("by-user/{userId}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<Job>>>> GetJobsByUserId(int userId)
        {
            var traceId = GetTraceId();
            // Assuming CustomerId is the user id; adjust property if your model differs
            var jobs = await _context.Jobs.Where(j => j.CustomerId == userId).ToListAsync();

            if (jobs == null || jobs.Count == 0)
            {
                _logger.LogWarning("No jobs found for user. UserId: {UserId}, TraceId: {TraceId}", userId, traceId);
                return NotFound(new ApiResponse<IEnumerable<Job>>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "No jobs found for the given user.",
                    TraceId = traceId,
                    Data = null
                });
            }

            _logger.LogInformation("Fetched jobs for user. UserId: {UserId}, TraceId: {TraceId}", userId, traceId);
            return Ok(new ApiResponse<IEnumerable<Job>>
            {
                Status = StatusCodes.Status200OK,
                Message = "Jobs fetched successfully.",
                TraceId = traceId,
                Data = jobs
            });
        }
    }
}
