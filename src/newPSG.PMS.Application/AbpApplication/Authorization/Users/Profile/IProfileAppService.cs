using System.Threading.Tasks;
using Abp.Application.Services;
using newPSG.PMS.Authorization.Users.Profile.Dto;

namespace newPSG.PMS.Authorization.Users.Profile
{
    public interface IProfileAppService : IApplicationService
    {
        Task<CurrentUserProfileEditDto> GetCurrentUserProfileForEdit();

        Task<string> UpdateCurrentUserProfile(CurrentUserProfileEditDto input);


        Task ChangePassword(ChangePasswordInput input);

        Task UpdateProfilePicture(UpdateProfilePictureInput input);

        Task<GetPasswordComplexitySettingOutput> GetPasswordComplexitySetting();
    }
}
