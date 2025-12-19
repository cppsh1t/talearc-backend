using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using talearc_backend.src.data;
using talearc_backend.src.data.entities;

namespace talearc_backend.src.application.controllers.common
{
    /// <summary>
    /// 需要身份验证的控制器基类，提供自动解析当前用户信息的功能
    /// </summary>
    public abstract class AuthenticatedControllerBase : ControllerBase
    {
        protected readonly AppDbContext Context;
        protected readonly ILogger Logger;

        protected AuthenticatedControllerBase(AppDbContext context, ILogger logger)
        {
            Context = context;
            Logger = logger;
        }

        /// <summary>
        /// 当前登录用户的ID
        /// </summary>
        protected int CurrentUserId
        {
            get
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out var userId))
                {
                    Logger.LogWarning("无效的用户ID: {UserIdClaim}", userIdClaim);
                    return 0;
                }
                return userId;
            }
        }

        /// <summary>
        /// 当前登录用户的用户名
        /// </summary>
        protected string CurrentUserName => User.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;

        /// <summary>
        /// 异步获取当前登录用户的完整实体对象
        /// </summary>
        /// <returns>当前用户实体，如果不存在则返回null</returns>
        protected async Task<User?> GetCurrentUserAsync()
        {
            var userId = CurrentUserId;
            if (userId == 0)
            {
                return null;
            }
            
            return await Context.Users.FindAsync(userId);
        }

        /// <summary>
        /// 验证资源是否属于当前用户
        /// </summary>
        /// <param name="resourceUserId">资源所属的用户ID</param>
        /// <returns>是否属于当前用户</returns>
        protected bool ValidateResourceOwnership(int resourceUserId)
        {
        return CurrentUserId == resourceUserId;
        }
    }
}
