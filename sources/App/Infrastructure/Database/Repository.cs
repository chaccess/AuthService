using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Database
{
    public class Repository(AuthDbContext context) : IRepository
    {
        private readonly AuthDbContext _context = context;

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

        public async Task<int> Commit()
        {
            return await _context.SaveChangesAsync();
        }

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

        public async Task<VerificationCode?> GetUserLastSmsVerificationCodeAsync(Guid id)
        {
            return await _context.VerificationCodes
                .WhereNotDeleted(c => c.UserId == id && c.Destination == VerificationCodeDestination.Phone && !c.Used)
                .OrderByDescending(c => c.CreateDate)
                .FirstOrDefaultAsync();
        }

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
