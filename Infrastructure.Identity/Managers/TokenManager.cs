using AutoMapper;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Application.DTOs.Tokens;
using Application.Interfaces;
using Application.Wrappers;
using Application.Parameters;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Identity.Contexts;
using Infrastructure.Identity.Models;
using Infrastructure.Identity.Helpers;

namespace Infrastructure.Identity.Managers
{
    public interface ITokenManager
    {
        Task<PaginatedResult<ResponseAccessToken>> GetAllAccessTokenAsync(PaginationFilter pagination, TokenFilter filter);
        Task<PaginatedResult<ResponseRefreshToken>> GetAllRefreshTokenAsync(PaginationFilter pagination, TokenFilter filter);


        Task<(ModelAccessToken, ModelUser)> GetAccessTokenAsync(string token);
        Task<(ModelRefreshToken, ModelUser)> GetRefreshTokenAsync(string token);

        Task<IResult<string>> ChangeStatusTokenAsync(string token, string ipAddress, bool active);
    }

    public class TokenManager : ITokenManager
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ICurrentUser _currentUser;
        private readonly IMapper _mapper;

        public TokenManager(ApplicationDbContext dbContext, ICurrentUser currentUser, IMapper mapper)
        {
            _dbContext = dbContext;
            _currentUser = currentUser;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<ResponseAccessToken>> GetAllAccessTokenAsync(PaginationFilter pagination, TokenFilter filter)
        {
            var validFilter = new PaginationFilter(pagination.PageNumber, pagination.PageSize);

            var totalCount = await _dbContext.AccessTokens.FilterBySuperAdmin(_currentUser).FilterForTokens(filter).CountAsync();

            var pagedTokens = await _dbContext.AccessTokens.FilterBySuperAdmin(_currentUser).FilterForTokens(filter).Include(x => x.User).OrderByDescending(x => x.Expires).Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).ToListAsync();

            return PaginatedResult<ResponseAccessToken>.Success(_mapper.Map<List<ResponseAccessToken>>(pagedTokens), totalCount, pagination.PageNumber, pagination.PageSize);
        }

        public async Task<PaginatedResult<ResponseRefreshToken>> GetAllRefreshTokenAsync(PaginationFilter pagination, TokenFilter filter)
        {
            var validFilter = new PaginationFilter(pagination.PageNumber, pagination.PageSize);

            var totalCount = await _dbContext.RefreshTokens.FilterBySuperAdmin(_currentUser).FilterForTokens(filter).CountAsync();

            var pagedTokens = await _dbContext.RefreshTokens.FilterBySuperAdmin(_currentUser).FilterForTokens(filter).Include(x => x.User).OrderByDescending(x => x.Expires).Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).ToListAsync();

            return PaginatedResult<ResponseRefreshToken>.Success(_mapper.Map<List<ResponseRefreshToken>>(pagedTokens), totalCount, pagination.PageNumber, pagination.PageSize);
        }

        public async Task<IResult<string>> ChangeStatusTokenAsync(string token, string ipAddress, bool active)
        {
            var (accessToken, user) = await GetAccessTokenAsync(token);

            if (accessToken is null || user is null)
                return await Result<string>.FailAsync("Недействительный токен");

            string resultMessage;

            if (accessToken.IsActive && !active)
            {
                accessToken.IsActive = false;

                resultMessage = "Токен неактивен";
            }
            else if (accessToken.IsActive && active)
            {
                return await Result<string>.FailAsync("Токен уже активен");
            }
            else if (!accessToken.IsActive && !active)
            {
                return await Result<string>.FailAsync("Токен уже неактивен");
            }
            else
            {
                accessToken.IsActive = true;

                resultMessage = "Токен активен";
            }

            _dbContext.Update(user);

            await _dbContext.SaveChangesAsync();

            return await Result<string>.SuccessAsync(resultMessage);
        }


        /// <summary>
        /// Получаем токен обновления
        /// </summary>
        /// <param name="token">Токен</param>
        /// <returns></returns>
        public async Task<(ModelRefreshToken, ModelUser)> GetRefreshTokenAsync(string token)
        {
            var user = await _dbContext.Users.Include(x => x.RefreshTokens).Include(x => x.AccessTokens).IgnoreQueryFilters().AsSplitQuery().SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user is null) return (null, null);

            var refreshToken = user.RefreshTokens.FirstOrDefault(x => x.Token == token);

            return (refreshToken, user);
        }

        /// <summary>
        /// Получаем токен доступа
        /// </summary>
        /// <param name="token">Токен</param>
        /// <returns></returns>
        public async Task<(ModelAccessToken, ModelUser)> GetAccessTokenAsync(string token)
        {
            var user = await _dbContext.Users.Include(x => x.RefreshTokens).Include(x => x.AccessTokens).IgnoreQueryFilters().AsSplitQuery().SingleOrDefaultAsync(u => u.AccessTokens.Any(t => t.Token == token));

            if (user is null) return (null, null);

            var accessToken = user.AccessTokens.FirstOrDefault(x => x.Token == token);

            return (accessToken, user);
        }
    }
}
