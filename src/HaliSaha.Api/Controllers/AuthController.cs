using Azure.Core;
using HaliSaha.Api.Contracts;
using HaliSaha.Core.Entities;
using HaliSaha.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HaliSaha.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AuthController(AppDbContext context, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }
        [HttpPost("register")]
        public async Task<ActionResult<UserResponse>> Register([FromBody] RegisterRequest request)
        {
            var normalizedEmail = request.Email.Trim().ToLowerInvariant();

            bool emailExists = await _context.Users.AnyAsync(u => u.Email == normalizedEmail);

            if (emailExists)
            {
                return Problem(
                    title: "E-posta kullanımda",
                    detail: "Bu e-posta adresiyle kayıtlı bir hesap zaten var.",
                    statusCode: StatusCodes.Status409Conflict);
            }

            var user = new User
            {
                FullName = request.FullName.Trim(),
                Email = normalizedEmail,
                Role = UserRole.Player,
                CreatedAt = DateTime.UtcNow
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var response = new UserResponse(user.Id, user.FullName, user.Email, user.Role.ToString());

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserResponse>> Login([FromBody] LoginRequest request)
        {
            var normalizedEmail = request.Email.Trim().ToLowerInvariant();

            var user = await _context.Users.FirstOrDefaultAsync(u=> u.Email == normalizedEmail);

            if(user == null || 
                _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
            {
                return Problem(
                    title: "Giriş başarısız",
                    detail: "E-posta veya şifre hatalı",
                    statusCode: StatusCodes.Status401Unauthorized
                    );
            }

            var userResponse = new UserResponse(user.Id, user.FullName, user.Email, user.Role.ToString());
           
            return Ok(userResponse);

        }
    }

}
