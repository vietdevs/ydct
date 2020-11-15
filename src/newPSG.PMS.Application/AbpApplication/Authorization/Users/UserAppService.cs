using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Notifications;
using Abp.Runtime.Session;
using Abp.UI;
using Abp.Zero.Configuration;
using Microsoft.AspNet.Identity;
using newPSG.PMS.Authorization.Permissions;
using newPSG.PMS.Authorization.Permissions.Dto;
using newPSG.PMS.Authorization.Roles;
using newPSG.PMS.Authorization.Users.Dto;
using newPSG.PMS.Authorization.Users.Exporting;
using newPSG.PMS.Dto;
using newPSG.PMS.Features;
using newPSG.PMS.Notifications;
using Abp.Domain.Uow;
using System.Configuration;
using newPSG.PMS.MultiTenancy;
using System;
using newPSG.PMS.Services;
using System.IO;
using System.Drawing;

namespace newPSG.PMS.Authorization.Users
{
    [AbpAuthorize(AppPermissions.Pages_Administration_Users)]
    public class UserAppService : PMSAppServiceBase, IUserAppService
    {
        private readonly RoleManager _roleManager;
        private readonly IUserEmailer _userEmailer;
        private readonly IUserListExcelExporter _userListExcelExporter;
        private readonly INotificationSubscriptionManager _notificationSubscriptionManager;
        private readonly IAppNotifier _appNotifier;
        private readonly IRepository<RolePermissionSetting, long> _rolePermissionRepository;
        private readonly IRepository<UserPermissionSetting, long> _userPermissionRepository;
        private readonly IRepository<UserRole, long> _userRoleRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IUserPolicy _userPolicy;
        private readonly TenantManager _tenantManager;
        private readonly IAbpSession _session;

        private readonly ICustomTennantAppService _customTennantAppService;
        private readonly IAppFolders _appFolders;

        public UserAppService(IAppFolders appFolders,
            RoleManager roleManager,
            IUserEmailer userEmailer,
            IUserListExcelExporter userListExcelExporter,
            INotificationSubscriptionManager notificationSubscriptionManager,
            IAppNotifier appNotifier,
            IRepository<RolePermissionSetting, long> rolePermissionRepository,
            IRepository<UserPermissionSetting, long> userPermissionRepository,
            IRepository<UserRole, long> userRoleRepository,
            IRepository<Role> roleRepository,
            IUserPolicy userPolicy,
            TenantManager tenantManager,
            IAbpSession session,

        ICustomTennantAppService customTennantAppService)
        {
            _roleManager = roleManager;
            _userEmailer = userEmailer;
            _userListExcelExporter = userListExcelExporter;
            _notificationSubscriptionManager = notificationSubscriptionManager;
            _appNotifier = appNotifier;
            _rolePermissionRepository = rolePermissionRepository;
            _userPermissionRepository = userPermissionRepository;
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
            _userPolicy = userPolicy;
            _tenantManager = tenantManager;
            _session = session;

            _customTennantAppService = customTennantAppService;
            _appFolders = appFolders;
        }

        public async Task<PagedResultDto<UserListDto>> GetUsers(GetUsersInput input)
        {
            var query = UserManager.Users
                .Include(u => u.Roles)
                .WhereIf(input.Role.HasValue, u => u.Roles.Any(r => r.RoleId == input.Role.Value))
                .WhereIf(
                    !input.Filter.IsNullOrWhiteSpace(),
                    u =>
                        u.Name.Contains(input.Filter) ||
                        u.Surname.Contains(input.Filter) ||
                        u.UserName.Contains(input.Filter) ||
                        u.EmailAddress.Contains(input.Filter)
                );

            if (!input.Permission.IsNullOrWhiteSpace())
            {
                query = (from user in query
                         join ur in _userRoleRepository.GetAll() on user.Id equals ur.UserId into urJoined
                         from ur in urJoined.DefaultIfEmpty()
                         join up in _userPermissionRepository.GetAll() on new { UserId = user.Id, Name = input.Permission } equals new { up.UserId, up.Name } into upJoined
                         from up in upJoined.DefaultIfEmpty()
                         join rp in _rolePermissionRepository.GetAll() on new { RoleId = ur.RoleId, Name = input.Permission } equals new { rp.RoleId, rp.Name } into rpJoined
                         from rp in rpJoined.DefaultIfEmpty()
                         where (up != null && up.IsGranted) || (up == null && rp != null)
                         group user by user into userGrouped
                         select userGrouped.Key);
            }

            var userCount = await query.CountAsync();
            var users = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();

            var userListDtos = users.MapTo<List<UserListDto>>();
            await FillRoleNames(userListDtos);

            return new PagedResultDto<UserListDto>(
                userCount,
                userListDtos
                );
        }

        public async Task<FileDto> GetUsersToExcel()
        {
            var users = await UserManager.Users.Include(u => u.Roles).ToListAsync();
            var userListDtos = users.MapTo<List<UserListDto>>();
            await FillRoleNames(userListDtos);

            return _userListExcelExporter.ExportToFile(userListDtos);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Users_Create, AppPermissions.Pages_Administration_Users_Edit)]
        public async Task<GetUserForEditOutput> GetUserForEdit(NullableIdDto<long> input)
        {
            //Getting all available roles
            var userRoleDtos = (await _roleManager.Roles
                .OrderBy(r => r.DisplayName)
                .Select(r => new UserRoleDto
                {
                    RoleId = r.Id,
                    RoleName = r.Name,
                    RoleDisplayName = r.DisplayName,
                    RoleLevel = r.RoleLevel
                })
                .ToArrayAsync());

            var output = new GetUserForEditOutput
            {
                Roles = userRoleDtos
            };

            if (!input.Id.HasValue)
            {
                //Creating a new user
                output.User = new UserEditDto
                {
                    IsActive = true,
                    ShouldChangePasswordOnNextLogin = true,
                    IsTwoFactorEnabled = await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.TwoFactorLogin.IsEnabled),
                    IsLockoutEnabled = await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.UserLockOut.IsEnabled)
                };

                foreach (var defaultRole in await _roleManager.Roles.Where(r => r.IsDefault).ToListAsync())
                {
                    var defaultUserRole = userRoleDtos.FirstOrDefault(ur => ur.RoleName == defaultRole.Name);
                    if (defaultUserRole != null)
                    {
                        defaultUserRole.IsAssigned = true;
                    }
                }
            }
            else
            {
                //Editing an existing user
                var user = await UserManager.GetUserByIdAsync(input.Id.Value);

                output.User = user.MapTo<UserEditDto>();
                output.ProfilePictureId = user.ProfilePictureId;

                foreach (var userRoleDto in userRoleDtos)
                {
                    userRoleDto.IsAssigned = await UserManager.IsInRoleAsync(input.Id.Value, userRoleDto.RoleName);
                }
            }

            if (!string.IsNullOrWhiteSpace(output.User.UrlImageChuKyNhay))
            {
                string HCC_FILE_PDF = GetUrlFileDefaut();
                var folderPath = Path.Combine(HCC_FILE_PDF, output.User.UrlImageChuKyNhay);

                if (File.Exists(folderPath))
                {
                    using (var fsTempProfilePicture = new FileStream(folderPath, FileMode.Open))
                    {
                        using (var bmpImage = new Bitmap(fsTempProfilePicture))
                        {
                            using (var stream = new MemoryStream())
                            {
                                bmpImage.Save(stream, bmpImage.RawFormat);
                                stream.Close();
                                output.User.DataImage = stream.ToArray();

                            }
                        }
                    }
                }
                 
            }
            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Users_ChangePermissions)]
        public async Task<GetUserPermissionsForEditOutput> GetUserPermissionsForEdit(EntityDto<long> input)
        {
            var user = await UserManager.GetUserByIdAsync(input.Id);
            var permissions = PermissionManager.GetAllPermissions();
            var grantedPermissions = await UserManager.GetGrantedPermissionsAsync(user);

            return new GetUserPermissionsForEditOutput
            {
                Permissions = permissions.MapTo<List<FlatPermissionDto>>().OrderBy(p => p.DisplayName).ToList(),
                GrantedPermissionNames = grantedPermissions.Select(p => p.Name).ToList()
            };
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Users_ChangePermissions)]
        public async Task ResetUserSpecificPermissions(EntityDto<long> input)
        {
            var user = await UserManager.GetUserByIdAsync(input.Id);
            await UserManager.ResetAllPermissionsAsync(user);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Users_ChangePermissions)]
        public async Task UpdateUserPermissions(UpdateUserPermissionsInput input)
        {
            var user = await UserManager.GetUserByIdAsync(input.Id);
            var grantedPermissions = PermissionManager.GetPermissionsFromNamesByValidating(input.GrantedPermissionNames);
            await UserManager.SetGrantedPermissionsAsync(user, grantedPermissions);
        }

        public async Task<string> CreateOrUpdateUser(CreateOrUpdateUserInput input)
        {
            var exMessage = string.Empty;
            input.User.IsChanChuKyNhay = string.IsNullOrWhiteSpace(input.User.UrlImageChuKyNhay) ? true : false;

            if (AbpSession.TenantId.HasValue)
            {
                exMessage = CheckFormatUserNameByTenant(AbpSession.TenantId, input.User.UserName.Trim());
                if (string.IsNullOrEmpty(exMessage))
                {
                    if (input.User.Id.HasValue)
                    {
                        await UpdateUserAsync(input);
                    }
                    else
                    {
                        await CreateUserAsync(input);
                    }
                }
                else
                {
                    return exMessage;
                }
            }
            else
            {
                if (input.User.Id.HasValue)
                {
                    await UpdateUserAsync(input);
                }
                else
                {
                  input.User.Id=  await CreateUserAsync(input);
                }
            }

            if(input.User.IsNew == true) await UpdateChuKyImageUrlAsync(input.User.UrlImageChuKyNhay, "KN", input.User.Id.Value);
            return exMessage;
        }

        public async Task UpdateChuKyImageUrlAsync(string fileName, string maChuKy, long UserId)
        {
            var tempProfilePicturePath = Path.Combine(_appFolders.TempFileDownloadFolder, fileName);

            byte[] byteArray;

            using (var fsTempProfilePicture = new FileStream(tempProfilePicturePath, FileMode.Open))
            {
                using (var bmpImage = new Bitmap(fsTempProfilePicture))
                {
                    using (var stream = new MemoryStream())
                    {
                        bmpImage.Save(stream, bmpImage.RawFormat);
                        stream.Close();
                        byteArray = stream.ToArray();

                        //var updateData = await _chuKyRepos.GetAsync(input);
                        string HCC_FILE_PDF = GetUrlFileDefaut();

                        //Tạo thư mục ngoài
                        string parentFolder = "_can-bo";
                        string baseFolder = @"ChuKy\" + parentFolder + @"\" + UserId;
                        var folderPath = Path.Combine(HCC_FILE_PDF, baseFolder);
                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }
                        var fileInfo = new FileInfo(tempProfilePicturePath);
                        var saveFileName = UserId + "_" + maChuKy + fileInfo.Extension;
                        var saveFileUrl = Path.Combine(baseFolder, saveFileName);
                        var saveFilePath = Path.Combine(folderPath, saveFileName);
                        bmpImage.Save(saveFilePath);

                        var user = await UserManager.FindByIdAsync(UserId);
                        user.UrlImageChuKyNhay = saveFileUrl;
                        // await _chuKyRepos.UpdateAsync(updateData);
                        await UserManager.UpdateAsync(user);
                    }
                }
            }

            Abp.IO.FileHelper.DeleteIfExists(tempProfilePicturePath);
        }

        private string CheckFormatUserNameByTenant(int? _tenantId, string _username)
        {
            var ex = string.Empty;
            if (_tenantId.HasValue)
            {
                if (_tenantId == _customTennantAppService.GetTenantIdCucHCC())
                {
                    if (_username.Count(x => x == '.') > 2)
                    {
                        ex = "Tài khoản chỉ được đặt dấu chấm ở '@hcc.gov.vn'";
                    }
                    else if (_username.Count(x => x == '.') == 2)
                    {
                        ex = _username.EndsWith("@hcc.gov.vn") ? null : "Tài khoản đăng nhập cần kết thúc bằng '@hcc.gov.vn'";
                    }
                }
                else if(_tenantId == _customTennantAppService.GetTenantIdDoanhNghiep())
                {
                    ex = _username.EndsWith("@hcc.gov.vn") ? "Tài khoản đăng nhập không được kết thúc bằng '@hcc.gov.vn'" : null;
                }
                else
                {
                    var tenant = _tenantManager.Tenants.FirstOrDefault(x => x.Id == _tenantId.Value && x.IsActive == true);
                    //if (tenant != null)
                    //{
                    //    if (_username.Count(x => x == '.') > 3)
                    //    {
                    //        ex = string.Format("Tài khoản chỉ được đặt dấu chấm ở '.{0}@vihema.gov.vn'", tenant.TenancyName);
                    //    }
                    //    else
                    //    {
                    //        ex = _username.EndsWith("." + tenant.TenancyName + "@vihema.gov.vn") ? null : string.Format("Tài khoản đăng nhập cần kết thúc bằng .{0}@vihema.gov.vn", tenant.TenancyName);
                    //    }
                    //}
                }
            }
            return ex;
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Users_Delete)]
        public async Task DeleteUser(EntityDto<long> input)
        {
            if (input.Id == AbpSession.GetUserId())
            {
                throw new UserFriendlyException(L("YouCanNotDeleteOwnAccount"));
            }

            var user = await UserManager.GetUserByIdAsync(input.Id);
            CheckErrors(await UserManager.DeleteAsync(user));
        }

        public async Task UnlockUser(EntityDto<long> input)
        {
            var user = await UserManager.GetUserByIdAsync(input.Id);
            user.Unlock();
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Users_Edit)]
        protected virtual async Task UpdateUserAsync(CreateOrUpdateUserInput input)
        {
            Debug.Assert(input.User.Id != null, "input.User.Id should be set.");

            var user = await UserManager.FindByIdAsync(input.User.Id.Value);

            //Update user properties
            input.User.MapTo(user); //Passwords is not mapped (see mapping configuration)

            if (input.SetRandomPassword)
            {
                input.User.Password = User.CreateRandomPassword();
            }

            if (!input.User.Password.IsNullOrEmpty())
            {
                CheckErrors(await UserManager.ChangePasswordAsync(user, input.User.Password));
            }
            user.PhongBanId = input.User.PhongBanId;
            user.RoleLevel = input.User.RoleLevel;
            CheckErrors(await UserManager.UpdateAsync(user));

            //Update roles
            CheckErrors(await UserManager.SetRoles(user, input.AssignedRoleNames));

            if (input.SendActivationEmail)
            {
                user.SetNewEmailConfirmationCode();
                await _userEmailer.SendEmailActivationLinkAsync(user, input.User.Password);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Users_Create)]
        protected virtual async Task<long> CreateUserAsync(CreateOrUpdateUserInput input)
        {
            input.User.UserName = input.User.UserName.Replace(" ", "");
            if (AbpSession.TenantId.HasValue)
            {
                await _userPolicy.CheckMaxUserCountAsync(AbpSession.GetTenantId());
            }

            var user = input.User.MapTo<User>(); //Passwords is not mapped (see mapping configuration)
            user.TenantId = AbpSession.TenantId;

            //Set password
            if (!input.User.Password.IsNullOrEmpty())
            {
                CheckErrors(await UserManager.PasswordValidator.ValidateAsync(input.User.Password));
            }
            else
            {
                input.User.Password = User.CreateRandomPassword();
            }

            user.Password = new PasswordHasher().HashPassword(input.User.Password);
            user.ShouldChangePasswordOnNextLogin = input.User.ShouldChangePasswordOnNextLogin;

            //Assign roles
            user.Roles = new Collection<UserRole>();
            foreach (var roleName in input.AssignedRoleNames)
            {
                var role = await _roleManager.GetRoleByNameAsync(roleName);
                user.Roles.Add(new UserRole(AbpSession.TenantId, user.Id, role.Id));
            }

            user.PhongBanId = input.User.PhongBanId;

            CheckErrors(await UserManager.CreateAsync(user));
            await CurrentUnitOfWork.SaveChangesAsync(); //To get new user's Id.

            //Notifications
            await _notificationSubscriptionManager.SubscribeToAllAvailableNotificationsAsync(user.ToUserIdentifier());
            await _appNotifier.WelcomeToTheApplicationAsync(user);

            //Send activation email
            if (input.SendActivationEmail)
            {
                user.SetNewEmailConfirmationCode();
                await _userEmailer.SendEmailActivationLinkAsync(user, input.User.Password);
            }

            return user.Id;
        }

        [AbpAllowAnonymous]
        public async Task<long> CreateUserFrontEndAsync(CreateOrUpdateUserInput input)
        {
            input.User.UserName = input.User.UserName.Replace(" ", "");

            int tenancyDoanhNghiepId = 1;

            if (!ConfigurationManager.AppSettings["TENANTCY_ID_DOANH_NGHIEP"].IsNullOrEmpty())
            {
                tenancyDoanhNghiepId = Convert.ToInt32(ConfigurationManager.AppSettings["TENANTCY_ID_DOANH_NGHIEP"]);
            }
            await _userPolicy.CheckMaxUserCountAsync(tenancyDoanhNghiepId);

            var user = input.User.MapTo<User>(); //Passwords is not mapped (see mapping configuration)
            user.TenantId = tenancyDoanhNghiepId;

            //Set password
            if (!input.User.Password.IsNullOrEmpty())
            {
                CheckErrors(await UserManager.PasswordValidator.ValidateAsync(input.User.Password));
            }
            else
            {
                input.User.Password = User.CreateRandomPassword();
            }

            user.Password = new PasswordHasher().HashPassword(input.User.Password);
            user.ShouldChangePasswordOnNextLogin = input.User.ShouldChangePasswordOnNextLogin;

            //Assign roles
            user.Roles = new Collection<UserRole>();
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                foreach (var roleName in input.AssignedRoleNames)
                {
                    //var role = _roleManager.Roles.FirstOrDefault(x => x.Name == roleName);
                    var role = await _roleManager.GetRoleByNameAsync(roleName);
                    user.Roles.Add(new UserRole(tenancyDoanhNghiepId, user.Id, role.Id));
                }
            }

            user.PhongBanId = input.User.PhongBanId;

            await UserManager.CreateAsync(user);
            await CurrentUnitOfWork.SaveChangesAsync(); //To get new user's Id.

            //Notifications
            await _notificationSubscriptionManager.SubscribeToAllAvailableNotificationsAsync(user.ToUserIdentifier());
            await _appNotifier.WelcomeToTheApplicationAsync(user);

            //Send activation email
            if (input.SendActivationEmail)
            {
                await _userEmailer.SendEmailConfirmRegisterAsync(user);
            }
            return user.Id;
        }

        private async Task FillRoleNames(List<UserListDto> userListDtos)
        {
            /* This method is optimized to fill role names to given list. */

            var distinctRoleIds = (
                from userListDto in userListDtos
                from userListRoleDto in userListDto.Roles
                select userListRoleDto.RoleId
                ).Distinct();

            var roleNames = new Dictionary<int, string>();
            foreach (var roleId in distinctRoleIds)
            {
                roleNames[roleId] = (await _roleManager.GetRoleByIdAsync(roleId)).DisplayName;
            }

            foreach (var userListDto in userListDtos)
            {
                foreach (var userListRoleDto in userListDto.Roles)
                {
                    userListRoleDto.RoleName = roleNames[userListRoleDto.RoleId];
                }

                userListDto.Roles = userListDto.Roles.OrderBy(r => r.RoleName).ToList();
            }
        }

        //Custom User
        [AbpAuthorize(AppPermissions.Pages_Tenant_QuanLyAdmin)]
        public async Task<PagedResultDto<AdminListDto>> GetAdmins(GetAdminsInput input)
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                var cucId = _customTennantAppService.GetTenantIdCucHCC();
                if(_session.TenantId != cucId && _session.TenantId != null)
                {
                    return null;
                }

                var query = (from user in UserManager.Users
                             join ur in _userRoleRepository.GetAll() on user.Id equals ur.UserId into urJoined
                             from ur in urJoined.DefaultIfEmpty()
                             join r in _roleRepository.GetAll() on ur.RoleId equals r.Id into rJoined
                             from r_tb in rJoined.DefaultIfEmpty()
                             join t in _tenantManager.Tenants on user.TenantId equals t.Id into tJoined
                             from t_tb in tJoined.DefaultIfEmpty()
                             where (r_tb.Name == "Admin" && user.IsActive == true && user.TenantId.HasValue && t_tb.IsActive == true)
                             select new AdminListDto
                             {
                                 Id = user.Id,
                                 UserName = user.UserName,
                                 Name = user.Name,
                                 Surname = user.Surname,
                                 EmailAddress = user.EmailAddress,
                                 TenantId = user.TenantId,
                                 TenantName = user.TenantId.HasValue ? t_tb.Name : string.Empty,
                                 TenancyName = user.TenantId.HasValue ? t_tb.TenancyName : "",
                                 CreationTime = user.CreationTime,
                                 LastLoginTime = user.LastLoginTime,
                                 IsActive = user.IsActive
                             })
                     .WhereIf(!string.IsNullOrEmpty(input.Filter), u =>
                                                                        u.Name.Contains(input.Filter) ||
                                                                        u.Surname.Contains(input.Filter) ||
                                                                        u.EmailAddress.Contains(input.Filter) ||
                                                                        u.UserName.Contains(input.Filter)
                                                                        )
                    .WhereIf(input.TenantId.HasValue, u => u.TenantId == input.TenantId);


                var adminCount = await query.CountAsync();
                var admins = await query
                    .OrderBy(u => u.UserName)
                    .PageBy(input)
                    .ToListAsync();

                return new PagedResultDto<AdminListDto>(
                    adminCount,
                    admins
                    );
            }
        }

        public async Task ChangePasswordAdmin(ChangePasswordAdminInput input)
        {
            try
            {
                using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
                {
                    var admin = await UserManager.GetUserByIdAsync(input.Id);
                    if (true)
                    {
                        CheckErrors(await UserManager.ChangePasswordAsync(admin, input.NewPassword));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task UnlockAdmin(EntityDto<long> input)
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                var user = await UserManager.GetUserByIdAsync(input.Id);
                user.Unlock();
            }
        }
    }
}
