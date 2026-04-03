using Azureblob2.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Azureblob2.Controllers
{
    public class BlobController : ControllerBase
    {
        private readonly IBlobStorageService _blobService;
        private readonly JwtTokenService _jwtTokenService;
        private readonly DBContext _context; 

        public BlobController(IBlobStorageService blobService,DBContext context, JwtTokenService jwtTokenService)
        {
            _blobService = blobService;
            _context = context;
            _jwtTokenService = jwtTokenService;
        }
        public class BlobStorageSettings
        {
            public string ConnectionString { get; set; } = string.Empty;
            public string ContainerName { get; set; } = string.Empty;
        }

        // POST /api/blob/upload
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file provided.");

            var url = await _blobService.UploadAsync(file);
            return Ok(new { url });
        }

        // GET /api/blob/download/{blobName}
        [HttpGet("download/{blobName}")]
        public async Task<IActionResult> Download(string blobName)
        {
            var stream = await _blobService.DownloadAsync(blobName);
            return File(stream, "application/octet-stream", blobName);
        }

        // DELETE /api/blob/{blobName}
        [HttpDelete("{blobName}")]
        public async Task<IActionResult> Delete(string blobName)
        {
            var deleted = await _blobService.DeleteAsync(blobName);
            return deleted ? Ok("Deleted successfully.") : NotFound("Blob not found.");
        }

        // GET /api/blob/list
        [HttpGet("list")]
        public async Task<IActionResult> List()
        {
            var blobs = await _blobService.ListBlobsAsync();
            return Ok(blobs);
        }
        public class LoginRequest
        {
            public string UserId { get; set; } = null!;
            public string Password { get; set; } = null!;
            public string Role { get; set; } = null!;
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            // Get the hashed password from db based on user name 
            // If no Record is found Return unauthorized
            //If Record is found, compare the password with the hashed password

            //var user = await _context.User.FirstOrDefault(x => x.UserId == loginRequest.UserId);
            var user = await _context.userMaster.FirstOrDefaultAsync(x => x.UserId == loginRequest.UserId);

            if (user == null)
            {
                return Unauthorized("Invalid username or password");
            }

            if (user.IsBlocked)
            {
                if (user.LockoutEndTime.HasValue && user.LockoutEndTime > DateTime.Now)
                {
                    var remainingTime = user.LockoutEndTime.Value - DateTime.Now;
                    return Forbid($"Account is locked. Try again after {remainingTime.Minutes} minutes.");
                }
                else
                {
                    // Unlock the account after timeout
                    user.IsBlocked = false;
                    user.FailedAttempts = 0;
                    user.LockoutEndTime = null;
                    await _context.SaveChangesAsync();
                }
            }
            // Check if the last failed attempt was more than 60 minutes ago (timeout period)
            if (user.FailedAttempts > 0 && user.LastFailedAttempt.HasValue)
            {
                var timeSinceLastFailure = DateTime.Now - user.LastFailedAttempt.Value;

                if (timeSinceLastFailure.TotalMinutes > 60)
                {
                    // Reset the failed attempts if they are too old
                    user.FailedAttempts = 0;
                    user.IsBlocked = false;
                    user.LockoutEndTime = null;

                    await _context.SaveChangesAsync();
                }
            }


            // Check password
            if (!BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.Password))
            {
                user.FailedAttempts += 1;
                user.LastFailedAttempt = DateTime.Now;

                if (user.FailedAttempts >= 5)
                {
                    user.IsBlocked = true;
                    user.LockoutEndTime = DateTime.Now.AddMinutes(60);
                    await _context.SaveChangesAsync();
                    return Forbid("Account locked for 60 minutes due to multiple failed login attempts.");
                }

                await _context.SaveChangesAsync();
                return Unauthorized($"Invalid password. {5 - user.FailedAttempts} attempt(s) left.");
            }

            // Successful login
            user.FailedAttempts = 0;
            user.IsBlocked = false;
            user.LockoutEndTime = null;
            await _context.SaveChangesAsync();
            var token = _jwtTokenService.GenerateJwtToken(loginRequest.UserId, loginRequest.Role);
            return Ok(new { token });
        }

        /// <summary>
        /// [Authorize(Roles = "Admin")]
        /// </summary>
        /// <param name="userMaster"></param>
        /// <returns></returns>
    
        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] UserMaster userMaster)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var createdUser = await _blobService.CreateUserAsync(userMaster);
               return CreatedAtAction(nameof(GetUserById), new { id = createdUser.UserId }, createdUser);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
       // [Authorize(Roles = "Admin")]
        // [HttpGet("GetById/{id}")]
        [HttpPost("GetById")]
        //public async Task<IActionResult> GetUserById(string id)
        public async Task<IActionResult> GetUserById([FromBody] UserRequestDto request)
        {
            try
            {
                var user = await _blobService.GetUsersAsync(request.UserId);
                if (user == null || (user is List<UserMaster> list && !list.Any()))
                    return NotFound("User not found.");

                return Ok(user);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}

