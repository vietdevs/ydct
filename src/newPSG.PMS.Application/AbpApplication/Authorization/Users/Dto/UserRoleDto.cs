using Abp.Application.Services.Dto;
using static newPSG.PMS.CommonENum;

namespace newPSG.PMS.Authorization.Users.Dto
{
    public class UserRoleDto
    {
        public int RoleId { get; set; }

        public string RoleName { get; set; }

        public string RoleDisplayName { get; set; }

        public bool IsAssigned { get; set; }
        public ROLE_LEVEL? RoleLevel { get; set; }
    }
}