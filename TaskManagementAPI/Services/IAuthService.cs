using TaskManagementAPI.Models;

namespace TaskManagementAPI.Services
{
    public interface IAuthService
    {
        Task<UserResponseDto> RegisterAsync(CreateUserDto userDto);
        Task<UserResponseDto?> LoginAsync(LoginDto loginDto);
        Task<bool> EmailExistsAsync(string email);
    }
}
