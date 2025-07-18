using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using eShift.Models;
using eShift.Data;

namespace eShift.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly EShiftDbContext _context;

        public DashboardController(EShiftDbContext context)
        {
            _context = context;
        }

        // 1. Summary endpoint: jobs, clients, admins, drivers, lorries, containers
        [HttpGet("summary")]
        public async Task<ActionResult<object>> GetSummary()
        {
            var jobsCount = await _context.Jobs.CountAsync();
            var clientsCount = await _context.Customers.CountAsync();
            var adminsCount = await _context.Admins.CountAsync();
            var driversCount = await _context.Drivers.CountAsync();
            var lorriesCount = await _context.Lorries.CountAsync();
            var containersCount = await _context.Containers.CountAsync();

            return Ok(new
            {
                jobsCount,
                clientsCount,
                adminsCount,
                driversCount,
                lorriesCount,
                containersCount
            });
        }

        // 2. Jobs by status (for pie/doughnut chart)
        [HttpGet("jobs-by-status")]
        public async Task<ActionResult<object>> GetJobsByStatus()
        {
            var data = await _context.Jobs
                .GroupBy(j => j.Status)
                .Select(g => new { status = g.Key, count = g.Count() })
                .ToListAsync();
            return Ok(data);
        }

        // 3. Jobs per branch (for bar chart)
        [HttpGet("jobs-by-branch")]
        public async Task<ActionResult<object>> GetJobsByBranch()
        {
            var data = await _context.Jobs
                .GroupBy(j => j.BranchId)
                .Select(g => new { branchId = g.Key, count = g.Count() })
                .ToListAsync();
            return Ok(data);
        }

        // 4. Jobs per day (for line chart)
        [HttpGet("jobs-per-day")]
        public async Task<ActionResult<object>> GetJobsPerDay()
        {
            var data = await _context.Jobs
                .GroupBy(j => j.Date.Date)
                .Select(g => new { date = g.Key, count = g.Count() })
                .OrderBy(x => x.date)
                .ToListAsync();
            return Ok(data);
        }

        // 5. Resource utilization (for gauge or bar chart)
        [HttpGet("resource-utilization")]
        public async Task<ActionResult<object>> GetResourceUtilization()
        {
            var totalDrivers = await _context.Drivers.CountAsync();
            var activeDrivers = await _context.Drivers.CountAsync(d => d.WorkingStatus == "Active");
            var totalLorries = await _context.Lorries.CountAsync();
            var availableLorries = await _context.Lorries.CountAsync(l => l.Status == "available");
            var totalContainers = await _context.Containers.CountAsync();

            return Ok(new
            {
                totalDrivers,
                activeDrivers,
                totalLorries,
                availableLorries,
                totalContainers
            });
        }
    }
}
