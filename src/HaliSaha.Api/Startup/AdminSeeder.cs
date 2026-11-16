using HaliSaha.Core.Entities;
using HaliSaha.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HaliSaha.Api.Startup
{
    public static class AdminSeeder
    {
        public static async Task SeedAdminAsync(this WebApplication app)
        {
            var email = app.Configuration["Admin:Email"];
            var password = app.Configuration["Admin:Password"];

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                app.Logger.LogWarning("Admin bilgileri ayarlarda bulunamadı, admin oluşturulmadı.");
                return;
            }

            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

            bool adminExists = await context.Users.AnyAsync(u => u.Role == UserRole.Admin);
            if (adminExists)
            {
                return;
            }

            var normalizedEmail = email.Trim().ToLowerInvariant();
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail);

            if (user == null)
            {
                user = new User
                {
                    FullName = "Sistem Yöneticisi",
                    Email = normalizedEmail,
                    CreatedAt = DateTime.UtcNow
                };
                user.PasswordHash = passwordHasher.HashPassword(user, password);
                context.Users.Add(user);
            }

            user.Role = UserRole.Admin;
            await context.SaveChangesAsync();

            app.Logger.LogInformation("Admin hesabı hazırlandı: {Email}", normalizedEmail);
        }
    }
}