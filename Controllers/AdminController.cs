using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using eShift.Data;
using eShift.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace eShift.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly EShiftDbContext _context;
        private readonly ILogger<AdminController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AdminController(EShiftDbContext context, ILogger<AdminController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetTraceId()
        {
            return _httpContextAccessor.HttpContext?.TraceIdentifier ?? Guid.NewGuid().ToString();
        }

        // GET: api/Admin
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<Admin>>>> GetAdmins()
        {
            var admins = await _context.Admins.ToListAsync();
            var response = new ApiResponse<IEnumerable<Admin>>
            {
                Status = StatusCodes.Status200OK,
                Message = "Admins fetched successfully.",
                TraceId = GetTraceId(),
                Data = admins
            };
            _logger.LogInformation("Fetched all admins. TraceId: {TraceId}", response.TraceId);
            return Ok(response);
        }

        // GET: api/Admin/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Admin>>> GetAdmin(int id)
        {
            var admin = await _context.Admins.FindAsync(id);
            var traceId = GetTraceId();
            if (admin == null)
            {
                _logger.LogWarning("Admin not found. Id: {Id}, TraceId: {TraceId}", id, traceId);
                return NotFound(new ApiResponse<Admin>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Admin not found.",
                    TraceId = traceId,
                    Data = null
                });
            }
            _logger.LogInformation("Fetched admin. Id: {Id}, TraceId: {TraceId}", id, traceId);
            return Ok(new ApiResponse<Admin>
            {
                Status = StatusCodes.Status200OK,
                Message = "Admin fetched successfully.",
                TraceId = traceId,
                Data = admin
            });
        }

        // POST: api/Admin
        [HttpPost]
        public async Task<ActionResult<ApiResponse<Admin>>> PostAdmin(Admin admin)
        {
            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();
            var traceId = GetTraceId();
            _logger.LogInformation("Admin created. Id: {Id}, TraceId: {TraceId}", admin.AdminId, traceId);
            var response = new ApiResponse<Admin>
            {
                Status = StatusCodes.Status201Created,
                Message = "Admin created successfully.",
                TraceId = traceId,
                Data = admin
            };
            return CreatedAtAction(nameof(GetAdmin), new { id = admin.AdminId }, response);
        }

        // PUT: api/Admin/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAdmin(int id, Admin admin)
        {
            var traceId = GetTraceId();
            if (id != admin.AdminId)
            {
                _logger.LogWarning("Admin update failed. Id mismatch. TraceId: {TraceId}", traceId);
                return BadRequest(new ApiResponse<Admin>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Admin ID mismatch.",
                    TraceId = traceId,
                    Data = null
                });
            }
            _context.Entry(admin).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Admins.Any(e => e.AdminId == id))
                {
                    _logger.LogWarning("Admin not found for update. Id: {Id}, TraceId: {TraceId}", id, traceId);
                    return NotFound(new ApiResponse<Admin>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Admin not found.",
                        TraceId = traceId,
                        Data = null
                    });
                }
                else
                {
                    throw;
                }
            }
            _logger.LogInformation("Admin updated. Id: {Id}, TraceId: {TraceId}", id, traceId);
            return Ok(new ApiResponse<Admin>
            {
                Status = StatusCodes.Status200OK,
                Message = "Admin updated successfully.",
                TraceId = traceId,
                Data = admin
            });
        }

        // DELETE: api/Admin/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            var admin = await _context.Admins.FindAsync(id);
            var traceId = GetTraceId();
            if (admin == null)
            {
                _logger.LogWarning("Admin not found for delete. Id: {Id}, TraceId: {TraceId}", id, traceId);
                return NotFound(new ApiResponse<Admin>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Admin not found.",
                    TraceId = traceId,
                    Data = null
                });
            }
            _context.Admins.Remove(admin);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Admin deleted. Id: {Id}, TraceId: {TraceId}", id, traceId);
            return Ok(new ApiResponse<Admin>
            {
                Status = StatusCodes.Status200OK,
                Message = "Admin deleted successfully.",
                TraceId = traceId,
                Data = admin
            });
        }

        // POST: api/Admin/login
        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<Admin>>> Login([FromBody] LoginRequest loginRequest)
        {
            var traceId = GetTraceId();
            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Username == loginRequest.Username && a.PasswordHash == loginRequest.PasswordHash);
            if (admin == null)
            {
                _logger.LogWarning("Login failed for username: {Username}, TraceId: {TraceId}", loginRequest.Username, traceId);
                return Unauthorized(new ApiResponse<Admin>
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Message = "Invalid username or password.",
                    TraceId = traceId,
                    Data = null
                });
            }
            _logger.LogInformation("Login successful for username: {Username}, TraceId: {TraceId}", loginRequest.Username, traceId);
            return Ok(new ApiResponse<Admin>
            {
                Status = StatusCodes.Status200OK,
                Message = "Login successful.",
                TraceId = traceId,
                Data = admin
            });
        }

        public class LoginRequest
        {
            public string Username { get; set; } = string.Empty;
            public string PasswordHash { get; set; } = string.Empty;
        }
    }
}
