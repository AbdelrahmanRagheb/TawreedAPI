using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using static System.Security.Cryptography.CryptographicOperations;
using Tawreed.Application.Common.Models;
using Tawreed.Application.Interfaces;
using Tawreed.Domain.Entities;
using Tawreed.Domain.Interfaces;

namespace Tawreed.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IBuyerRepository _buyerRepository;
    private readonly ISupplierRepository _supplierRepository;
    private readonly ISupplierCategoryRepository _supplierCategoryRepository;
    private readonly IDeliveryPersonProfileRepository _deliveryPersonProfileRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtService _jwtService;
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(
        IUserRepository userRepository,
        IBuyerRepository buyerRepository,
        ISupplierRepository supplierRepository,
        ISupplierCategoryRepository supplierCategoryRepository,
        IDeliveryPersonProfileRepository deliveryPersonProfileRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IJwtService jwtService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _buyerRepository = buyerRepository;
        _supplierRepository = supplierRepository;
        _supplierCategoryRepository = supplierCategoryRepository;
        _deliveryPersonProfileRepository = deliveryPersonProfileRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtService = jwtService;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken)
            ?? throw new UnauthorizedAccessException("Invalid email or password.");

        if (user.Status == "Suspended")
            throw new UnauthorizedAccessException("Account is suspended.");

        if (!VerifyPassword(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");

        user.LastLoginAt = DateTimeOffset.UtcNow;
        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await GenerateAuthResponse(user, cancellationToken);
    }

    #if DEBUG
    public async Task<AuthResponse> DebugLoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken)
            ?? throw new UnauthorizedAccessException("Invalid email or password.");

        if (user.Status == "Suspended")
            throw new UnauthorizedAccessException("Account is suspended.");

        user.LastLoginAt = DateTimeOffset.UtcNow;
        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await GenerateAuthResponse(user, cancellationToken);
    }
    #endif

    public async Task<AuthResponse> RegisterBuyerAsync(RegisterBuyerRequest request, CancellationToken cancellationToken = default)
    {
        if (await _userRepository.IsEmailUniqueAsync(request.Email, cancellationToken) == false)
            throw new InvalidOperationException("Email already registered.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = HashPassword(request.Password),
            Phone = request.Phone,
            FullName = request.FullName,
            Role = "Buyer",
            Status = "Active",
            PreferredLang = "en",
            EmailVerified = false,
            PhoneVerified = false
        };
        _userRepository.Add(user);

        var buyer = new Buyer
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            BusinessName = request.BusinessName,
            BusinessType = request.BusinessType,
            RegionId = request.RegionId,
            Address = request.Address,
            Latitude = null,
            Longitude = null,
            RatingAvg = 0
        };
        _buyerRepository.Add(buyer);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await GenerateAuthResponse(user, cancellationToken);
    }

    public async Task<AuthResponse> RegisterSupplierAsync(RegisterSupplierRequest request, CancellationToken cancellationToken = default)
    {
        if (await _userRepository.IsEmailUniqueAsync(request.Email, cancellationToken) == false)
            throw new InvalidOperationException("Email already registered.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = HashPassword(request.Password),
            Phone = request.Phone,
            FullName = request.FullName,
            Role = "Supplier",
            Status = "PendingApproval",
            PreferredLang = "en",
            EmailVerified = false,
            PhoneVerified = false
        };
        _userRepository.Add(user);

        var supplier = new Supplier
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            CompanyName = request.CompanyName,
            TaxId = request.TaxId,
            RegionId = request.RegionId,
            IsApproved = false,
            RatingAvg = 0
        };
        _supplierRepository.Add(supplier);

        foreach (var categoryId in request.CategoryIds)
        {
            _supplierCategoryRepository.Add(new SupplierCategory
            {
                SupplierId = supplier.Id,
                CategoryId = categoryId
            });
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await GenerateAuthResponse(user, cancellationToken);
    }

    public async Task<AuthResponse> RegisterDeliveryPersonAsync(RegisterDeliveryPersonRequest request, CancellationToken cancellationToken = default)
    {
        if (await _userRepository.IsEmailUniqueAsync(request.Email, cancellationToken) == false)
            throw new InvalidOperationException("Email already registered.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = HashPassword(request.Password),
            Phone = request.Phone,
            FullName = request.FullName,
            Role = "DeliveryPerson",
            Status = "Active",
            PreferredLang = "en",
            EmailVerified = false,
            PhoneVerified = false
        };
        _userRepository.Add(user);

        var profile = new DeliveryPersonProfile
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            VehicleType = request.VehicleType,
            LicenseInfo = request.LicenseInfo,
            BaseDeliveryFee = request.BaseDeliveryFee,
            Rating = 0,
            TotalDeliveries = 0,
            IsActive = true,
            CoverageRegionId = request.CoverageRegionId
        };
        _deliveryPersonProfileRepository.Add(profile);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await GenerateAuthResponse(user, cancellationToken);
    }

    public async Task LogoutAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        await _refreshTokenRepository.RevokeByUserAsync(userId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<AuthResponse> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken, cancellationToken)
            ?? throw new UnauthorizedAccessException("Invalid refresh token.");

        if (!storedToken.IsActive)
            throw new UnauthorizedAccessException("Refresh token expired or revoked.");

        storedToken.RevokedAt = DateTimeOffset.UtcNow;
        _refreshTokenRepository.Update(storedToken);

        var user = await _userRepository.GetByIdAsync(storedToken.UserId, cancellationToken)
            ?? throw new UnauthorizedAccessException("User not found.");

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await GenerateAuthResponse(user, cancellationToken);
    }

    public async Task ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("User not found.");

        if (user.PasswordHash != HashPassword(currentPassword))
            throw new UnauthorizedAccessException("Current password is incorrect.");

        user.PasswordHash = HashPassword(newPassword);
        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<UserInfo> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("User not found.");

        return new UserInfo(
            user.Id,
            user.FullName,
            user.Email,
            user.Role,
            user.AvatarUrl,
            user.PreferredLang
        );
    }

    private async Task<AuthResponse> GenerateAuthResponse(User user, CancellationToken cancellationToken)
    {
        var (token, refreshToken) = _jwtService.GenerateTokens(user);

        _refreshTokenRepository.Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(7)
        });
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthResponse(
            token,
            refreshToken,
            new UserInfo(user.Id, user.FullName, user.Email, user.Role, user.AvatarUrl, user.PreferredLang)
        );
    }

    public static string HashPassword(string password)
    {
        var salt = new byte[16];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(salt);

        var hash = KeyDerivation.Pbkdf2(
            password,
            salt,
            KeyDerivationPrf.HMACSHA256,
            100000,
            32
        );

        return Convert.ToBase64String(salt) + "." + Convert.ToBase64String(hash);
    }

    private static bool VerifyPassword(string password, string storedHash)
    {
        var parts = storedHash.Split('.');
        if (parts.Length != 2) return false;

        var salt = Convert.FromBase64String(parts[0]);
        var storedHashBytes = Convert.FromBase64String(parts[1]);

        var hash = KeyDerivation.Pbkdf2(
            password,
            salt,
            KeyDerivationPrf.HMACSHA256,
            100000,
            32
        );

        return FixedTimeEquals(storedHashBytes, hash);
    }
}
