using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using newPSG.PMS.Authorization.Users.Dto;
using newPSG.PMS.Dto;

namespace newPSG.PMS.Authorization.Users
{
    public interface IUserAppService : IApplicationService
    {
        Task<PagedResultDto<UserListDto>> GetUsers(GetUsersInput input);

        Task<FileDto> GetUsersToExcel();

        Task<GetUserForEditOutput> GetUserForEdit(NullableIdDto<long> input);

        Task<GetUserPermissionsForEditOutput> GetUserPermissionsForEdit(EntityDto<long> input);

        Task ResetUserSpecificPermissions(EntityDto<long> input);

        Task UpdateUserPermissions(UpdateUserPermissionsInput input);

        Task<string> CreateOrUpdateUser(CreateOrUpdateUserInput input);
        Task UpdateChuKyImageUrlAsync(string fileName, string maChuKy, long UserId);
        Task DeleteUser(EntityDto<long> input);

        Task UnlockUser(EntityDto<long> input);
        Task<long> CreateUserFrontEndAsync(CreateOrUpdateUserInput input);

        //Custom
        Task<PagedResultDto<AdminListDto>> GetAdmins(GetAdminsInput input);
        Task ChangePasswordAdmin(ChangePasswordAdminInput input);
        Task UnlockAdmin(EntityDto<long> input);
    }
}