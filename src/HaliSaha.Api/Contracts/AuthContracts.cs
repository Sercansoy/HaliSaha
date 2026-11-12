using System.ComponentModel.DataAnnotations;

namespace HaliSaha.Api.Contracts
{  
        public record RegisterRequest(
            [Required, MaxLength(100)] string FullName,
            [Required, EmailAddress] string Email,
            [Required, MinLength(8)] string Password);

        public record UserResponse (int Id, string Fullname, string Email, string Role);
}

