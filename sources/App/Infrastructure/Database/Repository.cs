using Application.Interfaces;
using Application.Services.AuthService.Contracts;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Infrastructure.Database
{
    public class Repository(AuthDbContext context) : IRepository
    {
        private readonly AuthDbContext _context = context;

        #region AddMethods
        public async Task AddRefreshTokenAsync(RefreshToken refreshToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken);
        }

        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task AddVerificationCodeAsync(VerificationCode verificationCode)
        {
            await _context.AddAsync(verificationCode);
        }

        public async Task AddUserInfoAsync(UserInfo info)
        {
            await _context.UserInfos.AddAsync(info);
        }
        #endregion

        #region DeleteMethods
        public bool ForceDeleteUser(User user)
        {
            _context.Users.Remove(user);

            return true;
        }
        #endregion

        #region UpdateMethods
        public void UpdateUser(User user)
        {
            _context.Users.Update(user);
        }
        #endregion

        #region AttachMethods
        public void AttachUser(User user, Expression<Func<User, object>>[] props)
        {
            _context.Users.Attach(user);

            var entry = _context.Users.Entry(user);

            foreach (var p in props)
            {
                entry.Property(p).IsModified = true;
            }
        }
        #endregion

        #region SaveMethods
        public async Task<int> Commit()
        {
            return await _context.SaveChangesAsync();
        }
        #endregion

        #region GetMethods
        public async Task<User?> GetUserByEmailAsync(string email, bool noTracking = false)
        {
            var query = _context.Users.WhereNotDeleted(u => u.Email == email);

            if (noTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<User?> GetUserByIdAsync(Guid id, bool noTracking = false)
        {
            var query = _context.Users.WhereNotDeleted(u => u.Id == id);

            if (noTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<User?> GetUserByPhoneAsync(string phone, bool noTracking = false)
        {
            var query = _context.Users.WhereNotDeleted(u => u.Phone == phone);

            if (noTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<RefreshToken?> GetUserLastRefreshTokenAsync(Guid id)
        {
            return await _context.RefreshTokens
                .WhereNotDeleted()
                .OrderByDescending(t => t.CreateDate)
                .FirstOrDefaultAsync(t => t.UserId == id);
        }

        public async Task<VerificationCode?> GetUserLastSmsVerificationCodeAsync(Guid id, bool isPhone)
        {
            return await _context.VerificationCodes
                .WhereNotDeleted(c => c.UserId == id && c.Destination == (isPhone ? VerificationCodeDestination.Phone : VerificationCodeDestination.Email) && !c.Used)
                .OrderByDescending(c => c.CreateDate)
                .FirstOrDefaultAsync();
        }
        #endregion

        #region ExistsMethods
        public async Task<bool> UserExistsByEmailAsync(string email)
        {
            return await _context.Users.WhereNotDeleted(u => u.Email == email).AnyAsync();
        }

        public async Task<bool> UserExistsByIdAsync(Guid userId)
        {
            return await _context.Users.WhereNotDeleted(u => u.Id == userId).AnyAsync();
        }

        public async Task<bool> UserExistsByPhoneAsync(string phone)
        {
            return await _context.Users.WhereNotDeleted(u => u.Phone == phone).AnyAsync();
        }
        #endregion
    }

    public static class DbSetExtentions
    {
        public static IQueryable<T> WhereNotDeleted<T>(this DbSet<T> dbSet, Expression<Func<T, bool>>? predicate = null) where T : BaseEntity
        {
            if (predicate == null)
            {
                return dbSet.Where(x => !x.IsDeleted);
            }

            var param = predicate.Parameters[0];
            var notDeleted = Expression.Not(Expression.Property(param, nameof(BaseEntity.IsDeleted)));
            var combined = Expression.Lambda<Func<T, bool>>(Expression.AndAlso(predicate.Body, notDeleted), param);

            return dbSet.Where(combined);
        }
    }
}
