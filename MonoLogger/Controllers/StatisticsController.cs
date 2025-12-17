using Microsoft.AspNetCore.Mvc;
using Monologer.Data;
using Microsoft.EntityFrameworkCore;

namespace MonoLogger.Controllers
{
    [ApiController]
    [Route("api/stats")]
    public class StatisticsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public StatisticsController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("messages/summary")]
        public async Task<IActionResult> GetMessageSummary()
        {
            var data = await _db.WebSocketMessages
                .GroupBy(_ => 1)
                .Select(g => new
                {
                    TotalMessages = g.Count(),
                    ErrorCount = g.Count(m => m.Type == MessageType.Error),
                    MessageCount = g.Count(m => m.Type == MessageType.Info),
                    WarningCount = g.Count(m => m.Type == MessageType.Warning),
                    AvgMagnitude = g.Average(m => (double?)m.Magnitude) ?? 0,
                    MaxMagnitude = g.Max(m => (int?)m.Magnitude) ?? 0,
                    MinMagnitude = g.Min(m => (int?)m.Magnitude) ?? 0,
                    UniqueUsers = g.Select(m => m.UserId).Distinct().Count(),
                    Last7DaysCount = g.Count(m => m.CreatedAt >= DateTime.UtcNow.AddDays(-7))
                })
                .FirstOrDefaultAsync();

            if (data == null)
            {
                return Ok(new
                {
                    TotalMessages = 0,
                    ErrorCount = 0,
                    MessageCount = 0,
                    WarningCount = 0,
                    AvgMagnitude = 0,
                    MaxMagnitude = 0,
                    MinMagnitude = 0,
                    UniqueUsers = 0,
                    Last7DaysCount = 0
                });
            }

            return Ok(data);
        }
        [HttpGet("messages/by-user")]
        public async Task<IActionResult> GetStatsByUser()
        {
            var data = await _db.WebSocketMessages
                .GroupBy(m => new { m.UserId, m.User.Name })
                .Select(g => new
                {
                    UserId = g.Key.UserId,
                    UserName = g.Key.Name,

                    TotalMessages = g.Count(),
                    Errors = g.Count(m => m.Type == MessageType.Error),
                    Messages = g.Count(m => m.Type == MessageType.Info),
                    Warnings = g.Count(m => m.Type == MessageType.Warning),

                    AvgMagnitude = g.Average(m => (double?)m.Magnitude) ?? 0,
                    FirstMessage = g.Min(m => m.CreatedAt),
                    LastMessage = g.Max(m => m.CreatedAt)
                })
                .OrderByDescending(x => x.TotalMessages)
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("messages/by-day")]
        public async Task<IActionResult> GetStatsByDay()
        {
            var data = await _db.WebSocketMessages
                .GroupBy(m => m.CreatedAt.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    TotalMessages = g.Count(),
                    Errors = g.Count(m => m.Type == MessageType.Error),
                    Messages = g.Count(m => m.Type == MessageType.Info),
                    Warnings = g.Count(m => m.Type == MessageType.Warning),
                    AvgMagnitude = g.Average(m => (double?)m.Magnitude) ?? 0
                })
                .OrderByDescending(x => x.Date)
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("messages/top-users")]
        public async Task<IActionResult> GetTopUsers()
        {
            var data = await _db.WebSocketMessages
                .GroupBy(m => new { m.UserId, m.User.Name })
                .Select(g => new
                {
                    UserId = g.Key.UserId,
                    UserName = g.Key.Name,
                    MessageCount = g.Count()
                })
                .OrderByDescending(x => x.MessageCount)
                .ToListAsync();

            return Ok(data);
        }
    }
}
