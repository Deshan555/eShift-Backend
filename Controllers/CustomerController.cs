using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using eShift.Data;
using eShift.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eShift.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly EShiftDbContext _context;
        private readonly ILogger<CustomerController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPasswordHasher<Customer> _passwordHasher;

        public class CustomerLoginRequest
        {
            public string EmailAddress { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        public CustomerController(EShiftDbContext context, ILogger<CustomerController> logger, IHttpContextAccessor httpContextAccessor, IPasswordHasher<Customer> passwordHasher)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _passwordHasher = passwordHasher;
        }

        private string GetTraceId() => _httpContextAccessor.HttpContext?.TraceIdentifier ?? System.Guid.NewGuid().ToString();

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<Customer>>>> GetAll()
        {
            var data = await _context.Customers.ToListAsync();
            var response = new ApiResponse<IEnumerable<Customer>>
            {
                Status = StatusCodes.Status200OK,
                Message = "Customers fetched successfully.",
                TraceId = GetTraceId(),
                Data = data
            };
            _logger.LogInformation("Fetched all customers. TraceId: {TraceId}", response.TraceId);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Customer>>> Get(int id)
        {
            var entity = await _context.Customers.FindAsync(id);
            var traceId = GetTraceId();
            if (entity == null)
            {
                _logger.LogWarning("Customer not found. Id: {Id}, TraceId: {TraceId}", id, traceId);
                return NotFound(new ApiResponse<Customer> { Status = StatusCodes.Status404NotFound, Message = "Customer not found.", TraceId = traceId, Data = null });
            }
            _logger.LogInformation("Fetched customer. Id: {Id}, TraceId: {TraceId}", id, traceId);
            return Ok(new ApiResponse<Customer> { Status = StatusCodes.Status200OK, Message = "Customer fetched successfully.", TraceId = traceId, Data = entity });
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<Customer>>> Post(Customer entity)
        {
            var traceId = GetTraceId();
            // Check if email is already used
            if (await _context.Customers.AnyAsync(c => c.EmailAddress == entity.EmailAddress))
            {
                _logger.LogWarning("Customer registration failed. Email already in use: {Email}, TraceId: {TraceId}", entity.EmailAddress, traceId);
                return Conflict(new ApiResponse<Customer>
                {
                    Status = StatusCodes.Status409Conflict,
                    Message = "Email address is already in use.",
                    TraceId = traceId,
                    Data = null
                });
            }
            // Hash the password before saving
            entity.Password = _passwordHasher.HashPassword(entity, entity.Password);
            _context.Customers.Add(entity);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Customer created. Id: {Id}, TraceId: {TraceId}", entity.CustomerId, traceId);
            var response = new ApiResponse<Customer> { Status = StatusCodes.Status201Created, Message = "Customer created successfully.", TraceId = traceId, Data = entity };
            return CreatedAtAction(nameof(Get), new { id = entity.CustomerId }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Customer entity)
        {
            var traceId = GetTraceId();
            if (id != entity.CustomerId)
            {
                _logger.LogWarning("Customer update failed. Id mismatch. TraceId: {TraceId}", traceId);
                return BadRequest(new ApiResponse<Customer> { Status = StatusCodes.Status400BadRequest, Message = "Customer ID mismatch.", TraceId = traceId, Data = null });
            }
            _context.Entry(entity).State = EntityState.Modified;
            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Customers.Any(e => e.CustomerId == id))
                {
                    _logger.LogWarning("Customer not found for update. Id: {Id}, TraceId: {TraceId}", id, traceId);
                    return NotFound(new ApiResponse<Customer> { Status = StatusCodes.Status404NotFound, Message = "Customer not found.", TraceId = traceId, Data = null });
                }
                else { throw; }
            }
            _logger.LogInformation("Customer updated. Id: {Id}, TraceId: {TraceId}", id, traceId);
            return Ok(new ApiResponse<Customer> { Status = StatusCodes.Status200OK, Message = "Customer updated successfully.", TraceId = traceId, Data = entity });
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<Customer>>> Login([FromBody] CustomerLoginRequest request)
        {
            var traceId = GetTraceId();
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.EmailAddress == request.EmailAddress);
            if (customer == null)
            {
                _logger.LogWarning("Customer login failed. Email not found: {Email}, TraceId: {TraceId}", request.EmailAddress, traceId);
                return Unauthorized(new ApiResponse<Customer>
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Message = "Invalid email or password.",
                    TraceId = traceId,
                    Data = null
                });
            }
            var result = _passwordHasher.VerifyHashedPassword(customer, customer.Password, request.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                _logger.LogWarning("Customer login failed. Invalid password for email: {Email}, TraceId: {TraceId}", request.EmailAddress, traceId);
                return Unauthorized(new ApiResponse<Customer>
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Message = "Invalid email or password.",
                    TraceId = traceId,
                    Data = null
                });
            }
            _logger.LogInformation("Customer login successful. Id: {Id}, TraceId: {TraceId}", customer.CustomerId, traceId);
            return Ok(new ApiResponse<Customer>
            {
                Status = StatusCodes.Status200OK,
                Message = "Login successful.",
                TraceId = traceId,
                Data = customer
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.Customers.FindAsync(id);
            var traceId = GetTraceId();
            if (entity == null)
            {
                _logger.LogWarning("Customer not found for delete. Id: {Id}, TraceId: {TraceId}", id, traceId);
                return NotFound(new ApiResponse<Customer> { Status = StatusCodes.Status404NotFound, Message = "Customer not found.", TraceId = traceId, Data = null });
            }
            _context.Customers.Remove(entity);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Customer deleted. Id: {Id}, TraceId: {TraceId}", id, traceId);
            return Ok(new ApiResponse<Customer> { Status = StatusCodes.Status200OK, Message = "Customer deleted successfully.", TraceId = traceId, Data = entity });
        }
    }
}
