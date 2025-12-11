using Microsoft.AspNetCore.Mvc;
using Monologer.Data;
using MonoLogger.Entities;
using System.Data.Entity;

namespace MonoLogger.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public UserController(AppDbContext dbContext) {
            _dbContext = dbContext;
        }

        [HttpPost]
        [Route("api/users/create")]
        public async Task<IActionResult> CreateUser([FromBody] string name)
        {
            var random = new Random();
            int accessKey;
            if (await _dbContext.Users.AnyAsync(u => u.Name == name))
                return Conflict(new { error = " User with this name already exists." });
            do
            {
                accessKey = random.Next(100000, 999999);
            } while (await _dbContext.Users.AnyAsync(u => u.AccessKey == accessKey));
            var user = new User
            {
                AccessKey = accessKey
            };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return Ok(new { accessKey = user.AccessKey });
        }
    }
}
