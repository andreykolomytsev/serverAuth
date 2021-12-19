using AutoMapper;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Identity.Contexts;
using Infrastructure.Identity.Helpers;
using Application.DTOs.Auth;
using Application.Interfaces;
using Application.Wrappers;
using Application.Constants;

namespace Infrastructure.Identity.Managers
{
    public class AuthenticateManager : IAuthenticateManager
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ITokenManager _tokenManager;
        private readonly IMapper _mapper;
        private readonly IJwtHelper _jwtHelper;

        public AuthenticateManager(ApplicationDbContext dbContext, ITokenManager tokenManager, IJwtHelper jwtHelper, IMapper mapper)
        {
            _dbContext = dbContext;
            _tokenManager = tokenManager;
            _jwtHelper = jwtHelper;
            _mapper = mapper;
        }

        public async Task<IResult<ResponseAuthentication>> AuthenticateAsync(RequestAuthentication model, string ipAddress, string browser)
        {
            var user = await _dbContext.Users.Include(x => x.RefreshTokens).Include(x => x.AccessTokens).IgnoreQueryFilters().OrderBy(x => x.Id).AsSplitQuery().FirstOrDefaultAsync(x => x.Email == model.Email);

            if (user is null)
                return await Result<ResponseAuthentication>.FailAsync("Неверный логин или пароль");

            if (!user.IsActive)
                return await Result<ResponseAuthentication>.FailAsync("Аккаунт заблокирован. Свяжитесь с Администратором");

            var passwordValid = PasswordHelper.VerifyPassword(user.PasswordHash, model.Password);

            if (!passwordValid)
                return await Result<ResponseAuthentication>.FailAsync("Неверный логин или пароль");

            var roles = await _dbContext.Roles.IgnoreQueryFilters().Where(x => x.Users.Any(a => a.UserId == user.Id)).ToListAsync();
            var services = await _dbContext.MicroServices.IgnoreQueryFilters().Where(x => x.Users.Any(a => a.UserId == user.Id)).ToListAsync();
            var permissions = await _dbContext.Permissions.Include(x => x.Roles).OrderBy(x => x.Id).AsSplitQuery().ToListAsync();

            var permissionsByUser = PermissionHelper.GetPermissionsByUser(user, roles, permissions, services);

            var jwtToken = _jwtHelper.GenerateToken(permissionsByUser, user, browser, ipAddress);
            var refreshToken = ApplicationHelper.GenerateRefreshToken(user, ipAddress, browser);

            user.RefreshTokens.Add(refreshToken);
            user.AccessTokens.Add(jwtToken);

            _dbContext.Update(user);

            await _dbContext.SaveChangesAsync();

            var response = _mapper.Map<ResponseAuthentication>(user);

            response.AccessToken = jwtToken.Token;
            response.RefreshToken = refreshToken.Token;
            response.RefreshTokenExpiry = refreshToken.Expires;
            response.RedirectURL = services.FirstOrDefault(x => x.Id == permissionsByUser.FirstOrDefault(p => p.Type == AllPermissionTypes.Service && p.Value == model.ServiceId)?.Value)?.URL;

            return await Result<ResponseAuthentication>.SuccessAsync(response);
        }

        public async Task<IResult<ResponseAuthentication>> RefreshTokenAsync(string token, string ipAddress, string browser)
        {
            // Проверяем токен обновления
            var (refreshToken, user) = await _tokenManager.GetRefreshTokenAsync(token);

            if (user is null || refreshToken is null)
                return await Result<ResponseAuthentication>.FailAsync("Недействительный токен обновления");

            if (refreshToken.IsExpired)
                return await Result<ResponseAuthentication>.FailAsync("Недействительный токен обновления");

            // Заменяем старый токен обновления новым
            var newRefreshToken = ApplicationHelper.GenerateRefreshToken(user, ipAddress, browser);

            refreshToken.Revoked = DateTime.Now;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.ReplacedByToken = newRefreshToken.Token;

            user.RefreshTokens.Add(newRefreshToken);

            var roles = await _dbContext.Roles.IgnoreQueryFilters().Where(x => x.Users.Any(a => a.UserId == user.Id)).ToListAsync();
            var services = await _dbContext.MicroServices.IgnoreQueryFilters().Where(x => x.Users.Any(a => a.UserId == user.Id)).ToListAsync();
            var permissions = await _dbContext.Permissions.Include(x => x.Roles).OrderBy(x => x.Id).AsSplitQuery().ToListAsync();

            // Генерируем новый токен доступа
            var newAccessToken = _jwtHelper.GenerateToken(PermissionHelper.GetPermissionsByUser(user, roles, permissions, services), user, browser, ipAddress);

            user.AccessTokens.Add(newAccessToken);

            _dbContext.Update(user);

            await _dbContext.SaveChangesAsync();

            var response = _mapper.Map<ResponseAuthentication>(user);
            response.AccessToken = newAccessToken.Token;
            response.RefreshToken = newRefreshToken.Token;
            response.RefreshTokenExpiry = newRefreshToken.Expires;

            return await Result<ResponseAuthentication>.SuccessAsync(response);
        }

        public async Task<IResult<string>> RevokeRefreshTokenAsync(string token, string ipAddress)
        {
            var (refreshToken, user) = await _tokenManager.GetRefreshTokenAsync(token);

            if (user is null || refreshToken is null)
                return await Result<string>.FailAsync("Недействительный токен обновления");

            if (refreshToken.IsExpired)
                return await Result<string>.FailAsync("Недействительный токен обновления");

            // Отзываем токен
            refreshToken.Revoked = DateTime.Now;
            refreshToken.RevokedByIp = ipAddress;

            _dbContext.Update(user);

            await _dbContext.SaveChangesAsync();

            return await Result<string>.SuccessAsync("Токен отозван");
        }

        public async Task<IResult<bool>> CheckAccessTokenPermissionsAsync(string token)
        {
            var (accessToken, user) = await _tokenManager.GetAccessTokenAsync(token);

            if (accessToken is null || user is null)
                return await Result<bool>.FailAsync("Недействительный токен");

            if (accessToken.IsOutDated)
                return await Result<bool>.FailAsync("Права доступа обновились");

            return await Result<bool>.SuccessAsync(true);
        }

        public async Task<bool> CheckActiveTokenAsync(string token)
        {
            var (accessToken, user) = await _tokenManager.GetAccessTokenAsync(token);

            if (accessToken is null || user is null)
                return false;

            if (!accessToken.IsActive)
                return false;

            return true;
        }
    }
}
