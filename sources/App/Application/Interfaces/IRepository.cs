using Domain.Entities;

namespace Application.Interfaces;

public interface IRepository
{
    #region Get Methods
    Task<User?> GetUserByIdAsync(Guid id, bool noTracking = false);
    Task<User?> GetUserByEmailAsync(string email, bool noTracking = false);
    Task<User?> GetUserByPhoneAsync(string phone, bool noTracking = false);
    Task<RefreshToken?> GetUserLastRefreshTokenAsync(Guid id);
    Task<VerificationCode?> GetUserLastSmsVerificationCodeAsync(Guid id);
    #endregion

    #region Exists Methods
    Task<bool> UserExistsByIdAsync(Guid userId);
    Task<bool> UserExistsByPhoneAsync(string phone);
    Task<bool> UserExistsByEmailAsync(string email);
    #endregion

    #region Add Methods
    Task AddUserAsync(User user);
    Task AddRefreshTokenAsync(RefreshToken refreshToken);
    Task AddVerificationCodeAsync(VerificationCode verificationCode);
    #endregion

    #region Save Methods
    // Save
    Task<int> Commit();
    #endregion
}
