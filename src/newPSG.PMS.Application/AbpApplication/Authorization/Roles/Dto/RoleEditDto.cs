using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using static newPSG.PMS.CommonENum;

namespace newPSG.PMS.Authorization.Roles.Dto
{
    [AutoMap(typeof(Role))]
    public class RoleEditDto
    {
        public int? Id { get; set; }

        [Required]
        public string DisplayName { get; set; }
        
        public bool IsDefault { get; set; }
        public ROLE_LEVEL? RoleLevel { get; set; }
    }
}