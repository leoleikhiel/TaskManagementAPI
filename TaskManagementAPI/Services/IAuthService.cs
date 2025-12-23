using TaskManagementAPI.Models;

namespace TaskManagementAPI.Services
{
    public interface IAuthService
    {
        Task<UserResponseDto> RegisterAsync(CreateUserDto userDto);
        Task<object?> LoginAsync(LoginDto loginDto);
        Task<bool> EmailExistsAsync(string email);
        Task<object?> GoogleLoginAsync(string idToken);
    }
}
