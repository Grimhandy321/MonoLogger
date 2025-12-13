using Microsoft.AspNetCore.Mvc;
using Monologer.Data;
using MonoLogger.Entities;
using Microsoft.EntityFrameworkCore;

namespace MonoLogger.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private static Random random = new Random();

        public UserController(AppDbContext dbContext) {
            _dbContext = dbContext;
        }

        [HttpPost]
        [Route("api/users/create")]
        public async Task<IActionResult> CreateUser([FromForm] User user)
        {
            var random = new Random();
            string accessKey;
            if (await _dbContext.Users.AnyAsync(u => u.Name == user.Name))
                return Conflict(new { error = " User with this name already exists." });
            do
            {
                accessKey = RandomString(17);
            } while (await _dbContext.Users.AnyAsync(u => u.AccessKey == accessKey));
            user = new User
            {
                AccessKey = accessKey
            };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return Ok(new { accessKey = user.AccessKey });
        }

   

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
