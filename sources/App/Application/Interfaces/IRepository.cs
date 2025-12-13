using Application.Services.AuthService.Contracts;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Application.Interfaces;

public interface IRepository
{
    #region Get Methods
    Task<User?> GetUserByIdAsync(Guid id, bool noTracking = false);
    Task<User?> GetUserByEmailAsync(string email, bool noTracking = false);
    Task<User?> GetUserByPhoneAsync(string phone, bool noTracking = false);
    Task<RefreshToken?> GetUserLastRefreshTokenAsync(Guid id);
    Task<VerificationCode?> GetUserLastSmsVerificationCodeAsync(Guid id, bool isPhone);
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
    Task AddUserInfoAsync(UserInfo info);
    #endregion

    #region AttachMethods
    void AttachUser(User user, Expression<Func<User, object>>[] props);
    #endregion

    #region DeleteMethods
    bool ForceDeleteUser(User user);
    #endregion

    #region UpdateMethods
    void UpdateUser(User user);
    #endregion

    #region Save Methods
    Task<int> Commit();
    #endregion
}
