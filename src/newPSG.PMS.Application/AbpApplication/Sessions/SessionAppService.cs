using Abp.Auditing;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Notifications;
using newPSG.PMS.Authorization.Roles;
using newPSG.PMS.Authorization.Users;
using newPSG.PMS.Authorization.Users.Exporting;
using newPSG.PMS.EntityDB;
using newPSG.PMS.Notifications;
using newPSG.PMS.Sessions.Dto;
using Newtonsoft.Json;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace newPSG.PMS.Sessions
{
    [AbpAuthorize]
    public class SessionAppService : PMSAppServiceBase, ISessionAppService
    {
        private readonly IRepository<UserRole, long> _userRoleRepository;
        private readonly UserManager _userManager;
        private readonly IRepository<Role> _roleRepos;
        private readonly IRepository<ChucVu> _chucVuRepos;
        public SessionAppService(IRepository<UserRole, long> userRoleRepository
            , UserManager userManager
            , IRepository<ChucVu> chucVuRepos
            , IRepository<Role> roleRepos)
        {
            _userManager = userManager;
            _userRoleRepository = userRoleRepository;
            _roleRepos = roleRepos;
            _chucVuRepos = chucVuRepos;
        }
        [DisableAuditing]
        public async Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations()
        {
            var output = new GetCurrentLoginInformationsOutput
            {
                User = (await GetCurrentUserAsync()).MapTo<UserLoginInfoDto>()
            };

            if (AbpSession.TenantId.HasValue)
            {
                output.Tenant = (await GetCurrentTenantAsync()).MapTo<TenantLoginInfoDto>();
            }

            if(output.User.RoleLevel.HasValue)
            {
                var roleLevel = (CommonENum.ROLE_LEVEL)output.User.RoleLevel;
                output.User.RoleLevelName = CommonENum.GetEnumDescription(roleLevel);
            }
            if(SessionCustom.UserCurrent != null)
            {
                output.User.ThuTucEnum = SessionCustom.UserCurrent.ThuTucEnum;
            }
            return output;
        }

        //public object GetRoleCurrentUser()
        //{
        //    var userId = _userManager.AbpSession.UserId;
        //    var urole = _userRoleRepository.FirstOrDefault(x => x.UserId == userId);
        //    var role = _roleRepos.FirstOrDefault(x => x.Id == urole.RoleId);

        //    return new
        //    {
        //        DisplayName = role.DisplayName,
        //        RoleLevel = role.RoleLevel
        //    };
        //}

        public object GetAllChucVu()
        {
            var querry = from cv in _chucVuRepos.GetAll()
                         orderby cv.Id
                         select new
                         {
                             Ten = cv.TenChucVu,
                             Id = cv.Id
                         };
            return querry.ToList();
        }
    }
}


namespace newPSG.PMS
{
    public class CustomSessionAppSession : ITransientDependency
    {
        public User UserSession
        {
            get
            {
                var ret = new User();
                var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
                if (claimsPrincipal == null)
                {
                    return null;
                }

                var userClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == AppConsts.KeyClaimSession);
                if (userClaim == null || string.IsNullOrEmpty(userClaim.Value))
                {
                    return null;
                }
                ret = JsonConvert.DeserializeObject<User>(userClaim.Value);

                return ret;
            }
        }
    }
    public static class SessionCustom
    {
        public static UserLoginInfoDto UserCurrent
        {
            get
            {
                var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
                if (claimsPrincipal == null)
                {
                    return null;
                }

                var userClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == AppConsts.KeyClaimSession);
                if (userClaim == null || string.IsNullOrEmpty(userClaim.Value))
                {
                    return null;
                }
                return JsonConvert.DeserializeObject<UserLoginInfoDto>(userClaim.Value);
            }
        }
    }
}