using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Zero.Configuration;
using Microsoft.AspNet.Identity;
using newPSG.PMS.Authorization.Roles;
using newPSG.PMS.Authorization.Users;
using newPSG.PMS.Authorization.Users.Dto;
using newPSG.PMS.Dto;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Abp.Configuration;
using System.Threading.Tasks;

using System.Linq.Dynamic;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using Abp.Extensions;
using newPSG.PMS.EntityDB;
using static newPSG.PMS.CommonENum;
using Abp.Authorization;
using newPSG.PMS.Authorization;

namespace newPSG.PMS.Service
{
    public interface IQuanLyTaiKhoanAppService : IApplicationService
    {
        Task<PagedResultDto<QuanLyTaiKhoanPageListDto>> GetAllServerPaging(QuanLyTaiKhoanInputPageDto input);
        Task<string> CreateOrUpdate(CreateOrUpdateUser_CustomInput input);
        Task<GetUserForEditOutput> GetForEdit(GetForEditInput input);
        Task Delete(EntityDto<long> input);
        Task UnlockUser(EntityDto<long> input);
        List<ItemObj<int>> GetListChucVuByLoaiTK(int loaiTK);
    }
    [AbpAuthorize(AppPermissions.Pages_QuanLyTaiKhoan)]
    public class QuanLyTaiKhoanAppService : PMSAppServiceBase, IQuanLyTaiKhoanAppService
    {
        private readonly IRepository<User, long> _userRepos;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IUserAppService _userService;
        private readonly RoleManager _roleManager;
        private readonly IRepository<UserRole, long> _userRoleRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<PhongBan> _phongBanRepository;
        public QuanLyTaiKhoanAppService(
            IRepository<User, long> userRepos,
            IUnitOfWorkManager unitOfWorkManager,
            IUserAppService userService,
            RoleManager roleManager,
            IRepository<UserRole, long> userRoleRepository,
            IRepository<Role> roleRepository,
             IRepository<PhongBan> phongBanRepository
        )
        {
            _userRepos = userRepos;
            _unitOfWorkManager = unitOfWorkManager;
            _userService = userService;
            _roleManager = roleManager;
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
            _phongBanRepository = phongBanRepository;
        }

        private List<ROLE_LEVEL> GetROLE_LEVELs(LOAI_TAI_KHOAN loaiTK)
        {
            var listRoleLevel = new List<ROLE_LEVEL>();
            switch (loaiTK)
            {
                case LOAI_TAI_KHOAN.TAI_KHOAN_CUC:
                    listRoleLevel.AddRange(new List<ROLE_LEVEL>()
                        {
                            ROLE_LEVEL.BO_PHAN_MOT_CUA,
                            ROLE_LEVEL.KE_TOAN,
                            ROLE_LEVEL.CHUYEN_VIEN,
                            ROLE_LEVEL.PHO_PHONG,
                            ROLE_LEVEL.TRUONG_PHONG,
                            ROLE_LEVEL.LANH_DAO_CUC,
                            ROLE_LEVEL.VAN_THU,
                            ROLE_LEVEL.HOI_DONG_THAM_DINH,
                        });
                    break;
                case LOAI_TAI_KHOAN.TAI_KHOAN_CHUYEN_GIA:
                    listRoleLevel.Add(ROLE_LEVEL.CHUYEN_GIA);
                    break;
                case LOAI_TAI_KHOAN.TAI_KHOAN_SO_Y_TE:
                    listRoleLevel.Add(ROLE_LEVEL.SO_YTE_KIEM_NGHIEM);
                    break;
                default:
                    break;
            }
            return listRoleLevel;
        }

        public List<ItemObj<int>> GetListChucVuByLoaiTK(int loaiTK)
        {
            var ro_level = GetROLE_LEVELs((LOAI_TAI_KHOAN)loaiTK);
            var _list = new List<ItemObj<int>>();
            foreach (var item in ro_level)
            {
                _list.Add(new ItemObj<int>()
                {
                    Id = (int)item,
                    Name = CommonENum.GetEnumDescription(item)
                });
            }
            return _list;
        }

        #region Get Thông tin tài khoản
        public async Task<PagedResultDto<QuanLyTaiKhoanPageListDto>> GetAllServerPaging(QuanLyTaiKhoanInputPageDto input)
        {
            var pageRes = new PagedResultDto<QuanLyTaiKhoanPageListDto>();
            var listRoleLevelEnum = GetROLE_LEVELs(input.LoaiTaiKhoan);
            var listRoleLevelEnumInt = listRoleLevelEnum.Cast<int>().ToList();
            switch (input.LoaiTaiKhoan)
            {
                case LOAI_TAI_KHOAN.TAI_KHOAN_CUC:
                    pageRes = await GetTaiKhoanNoiBo(input, listRoleLevelEnumInt);
                    break;
                case LOAI_TAI_KHOAN.TAI_KHOAN_CHUYEN_GIA:
                    pageRes = await GetTaiKhoanChuyenGia(input, listRoleLevelEnumInt);
                    break;
                case LOAI_TAI_KHOAN.TAI_KHOAN_SO_Y_TE:
                    pageRes = await GetTaiKhoanSoYTe(input, listRoleLevelEnumInt);
                    break;
                default:
                    break;
            }
            return pageRes;
        }

        private async Task<PagedResultDto<QuanLyTaiKhoanPageListDto>> GetTaiKhoanChuyenGia(QuanLyTaiKhoanInputPageDto input, List<int> listRole_level)
        {
            var query = (from user in _userRepos.GetAll().Where(x => listRole_level.Contains(x.RoleLevel.Value))
                         join ur in _userRoleRepository.GetAll() on user.Id equals ur.UserId
                         join role in _roleRepository.GetAll() on ur.RoleId equals role.Id
                         join r_phongBan in _phongBanRepository.GetAll() on user.PhongBanId equals r_phongBan.Id into tb_phongBan
                         from phongBan in tb_phongBan.DefaultIfEmpty()
                         select new QuanLyTaiKhoanPageListDto
                         {
                             Id = user.Id,
                             Name = user.Name,
                             Surname = user.Surname,
                             EmailAddress = user.EmailAddress,
                             PhoneNumber = user.PhoneNumber,
                             StrPhongBan = phongBan.TenPhongBan,
                             UserName = user.UserName,
                             IsActive = user.IsActive,
                             LastLoginTime = user.LastLoginTime
                         } into tabTemp
                         group tabTemp by tabTemp into tbGroup
                         select tbGroup.Key)
                         .WhereIf(!input.Filter.IsNullOrWhiteSpace(),
                            u => u.Name.Contains(input.Filter) || u.Surname.Contains(input.Filter) || u.UserName.Contains(input.Filter)
                            || u.StrPhongBan.Contains(input.Filter) || u.EmailAddress.Contains(input.Filter) | u.StrDonViChuyenGia.Contains(input.Filter));
            var count = await query.CountAsync();
            var dataGrids = await query
                .OrderBy(o => o.Id)
                .PageBy(input)
                .ToListAsync();
            return new PagedResultDto<QuanLyTaiKhoanPageListDto>(count, dataGrids);
        }

        private async Task<PagedResultDto<QuanLyTaiKhoanPageListDto>> GetTaiKhoanSoYTe(QuanLyTaiKhoanInputPageDto input, List<int> listRole_level)
        {
            var query = (from user in _userRepos.GetAll().Where(x => listRole_level.Contains(x.RoleLevel.Value))
                         join ur in _userRoleRepository.GetAll() on user.Id equals ur.UserId
                         join role in _roleRepository.GetAll() on ur.RoleId equals role.Id
                         join r_phongBan in _phongBanRepository.GetAll() on user.PhongBanId equals r_phongBan.Id into tb_phongBan
                         from phongBan in tb_phongBan.DefaultIfEmpty()
                         select new QuanLyTaiKhoanPageListDto
                         {
                             Id = user.Id,
                             Name = user.Name,
                             Surname = user.Surname,
                             EmailAddress = user.EmailAddress,
                             PhoneNumber = user.PhoneNumber,
                             StrPhongBan = phongBan.TenPhongBan,
                             UserName = user.UserName,
                             IsActive = user.IsActive,
                             LastLoginTime = user.LastLoginTime,
                         } into tabTemp
                         group tabTemp by tabTemp into tbGroup
                         select tbGroup.Key)
                         .WhereIf(!input.Filter.IsNullOrWhiteSpace(),
                            u => u.Name.Contains(input.Filter) || u.Surname.Contains(input.Filter) || u.UserName.Contains(input.Filter)
                            || u.StrPhongBan.Contains(input.Filter) || u.EmailAddress.Contains(input.Filter));
            var count = await query.CountAsync();
            var dataGrids = await query
                .OrderBy(o => o.Id)
                .PageBy(input)
                .ToListAsync();

            return new PagedResultDto<QuanLyTaiKhoanPageListDto>(count, dataGrids);
        }

        private async Task<PagedResultDto<QuanLyTaiKhoanPageListDto>> GetTaiKhoanNoiBo(QuanLyTaiKhoanInputPageDto input, List<int> listRole_level)
        {
            var query = (from user in _userRepos.GetAll().Where(x => listRole_level.Contains(x.RoleLevel.Value))
                         join ur in _userRoleRepository.GetAll() on user.Id equals ur.UserId
                         join role in _roleRepository.GetAll() on ur.RoleId equals role.Id
                         join r_phongBan in _phongBanRepository.GetAll() on user.PhongBanId equals r_phongBan.Id into tb_phongBan
                         from phongBan in tb_phongBan.DefaultIfEmpty()
                         select new QuanLyTaiKhoanPageListDto
                         {
                             Id = user.Id,
                             Name = user.Name,
                             Surname = user.Surname,
                             EmailAddress = user.EmailAddress,
                             PhoneNumber = user.PhoneNumber,
                             StrPhongBan = phongBan.TenPhongBan,
                             UserName = user.UserName,
                             IsActive = user.IsActive,
                             LastLoginTime = user.LastLoginTime,
                             Role_Level=user.RoleLevel
                             
                         } into tabTemp
                         group tabTemp by tabTemp into tbGroup
                         select tbGroup.Key)
                         .WhereIf(!input.Filter.IsNullOrWhiteSpace(),
                            u => u.Name.Contains(input.Filter) || u.Surname.Contains(input.Filter) || u.UserName.Contains(input.Filter)
                            || u.StrPhongBan.Contains(input.Filter) || u.EmailAddress.Contains(input.Filter));
            var count = await query.CountAsync();
            var dataGrids = await query
                .OrderBy(o => o.Id)
                .PageBy(input)
                .ToListAsync();

            return new PagedResultDto<QuanLyTaiKhoanPageListDto>(count, dataGrids);
        }
        #endregion;

        public async Task<string> CreateOrUpdate(CreateOrUpdateUser_CustomInput input)
        {
            var resMess = "";
            var user = new CreateOrUpdateUserInput()
            {
                User = input.User,
                AssignedRoleNames=input.AssignedRoleNames,
                SendActivationEmail = false,
                SetRandomPassword = input.SetRandomPassword
            };
            switch (input.LoaiTaiKhoan)
            {
                case LOAI_TAI_KHOAN.TAI_KHOAN_CUC:
                    // Không cần thêm Role_LEVEL vì đã chọn bên ngoài Form
                    break;
                case LOAI_TAI_KHOAN.TAI_KHOAN_CHUYEN_GIA:
                    user.User.RoleLevel =(int) ROLE_LEVEL.CHUYEN_GIA;
                    break;
                case LOAI_TAI_KHOAN.TAI_KHOAN_SO_Y_TE:
                    user.User.RoleLevel = (int)ROLE_LEVEL.SO_YTE_KIEM_NGHIEM;
                    break;
                default:
                    break;
            }
            resMess = await _userService.CreateOrUpdateUser(user);
            return resMess;
        }

        public async Task<GetUserForEditOutput> GetForEdit(GetForEditInput input)
        {
            var listRoleLevelEnum = GetROLE_LEVELs(input.LoaiTaiKhoan);
            var userRoleDtos = (await _roleManager.Roles
                       .Where(x => listRoleLevelEnum.Contains(x.RoleLevel.Value))
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
            if (input.Id.HasValue)
            {
                var user = await UserManager.GetUserByIdAsync(input.Id.Value);
              
                output.User = user.MapTo<UserEditDto>();
                output.ProfilePictureId = user.ProfilePictureId;
                foreach (var userRoleDto in userRoleDtos)
                {
                    userRoleDto.IsAssigned = await UserManager.IsInRoleAsync(input.Id.Value, userRoleDto.RoleName);
                }
            }
            else
            {
                output.User = new UserEditDto
                {
                    IsActive = true,
                    ShouldChangePasswordOnNextLogin = false,
                    IsTwoFactorEnabled = false,
                    IsLockoutEnabled = true
                };
            }
            return output;

        }

        public async Task Delete(EntityDto<long> input)
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
    }

}
