using Application.Exceptions;
using Application.Interfaces;
using Application.Services.UserService.Models;
using AutoMapper;
using Domain.Entities;
using System.Linq.Expressions;

namespace Application.Services.UserService
{
    public class UserService(
        IRepository repository,
        IMapper mapper
        ) : IUserService
    {
        private readonly IRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<User> CreateUserAsync(User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            user.Id = Guid.NewGuid();

            if (await UserExists(user))
            {
                throw new AlreadyExistsException("Пользователь уже существует");
            }

            await _repository.AddUserAsync(user);
            await _repository.Commit();

            return user;
        }



        public async Task<bool> DeleteUser(Guid userId)
        {
            var user = await _repository.GetUserByIdAsync(userId);

            if (user is null)
            {
                return false;
            }

            user.IsDeleted = true;

            await _repository.Commit();
            return true;
        }

        public async Task<bool> ForceDeleteUser(Guid userId)
        {
            var user = await _repository.GetUserByIdAsync(userId);

            if (user is null)
            {
                return false;
            }

            _repository.ForceDeleteUser(user);
            await _repository.Commit();
            return true;
        }

        public async Task<bool> UpdateUserPartial(UpdateUserRequest userModel)
        {
            ArgumentNullException.ThrowIfNull(userModel);

            if (!await _repository.UserExistsByIdAsync(userModel.Id))
            {
                return false;
            }

            var user = _mapper.Map<User>(userModel);

            var props = new List<Expression<Func<User, object>>>();

            if (user.Phone != null)
            {
                props.Add(x => x.Phone);
            }
            if (user.Email != null)
            {
                props.Add(x => x.Email);
            }

            _repository.AttachUser(user, [.. props]);
            await _repository.Commit();

            return true;
        }

        public async Task<User> UpdateUser(User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            if (!await _repository.UserExistsByIdAsync(user.Id))
            {
                throw new NotFoundException("Пользователь не найден");
            }

            _repository.UpdateUser(user);
            await _repository.Commit();

            return user;
        }

        private async Task<bool> UserExists(User user)
        {
            var byId = await _repository.UserExistsByIdAsync(user.Id);
            var byEmail = await _repository.UserExistsByEmailAsync(user.Email);
            var byPhone = await _repository.UserExistsByPhoneAsync(user.Phone);

            return byId || byEmail || byPhone;
        }
    }
}
