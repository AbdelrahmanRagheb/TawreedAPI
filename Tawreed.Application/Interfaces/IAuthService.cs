using Tawreed.Application.Common.Models;

namespace Tawreed.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse> RegisterBuyerAsync(RegisterBuyerRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse> RegisterSupplierAsync(RegisterSupplierRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse> RegisterDeliveryPersonAsync(RegisterDeliveryPersonRequest request, CancellationToken cancellationToken = default);
    Task LogoutAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<AuthResponse> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default);
    Task<UserInfo> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken = default);
#if DEBUG
    Task<AuthResponse> DebugLoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
#endif
}
