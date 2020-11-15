using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using newPSG.PMS.Authorization.Users;
using System.Collections.Generic;

namespace newPSG.PMS.Sessions.Dto
{
    [AutoMapFrom(typeof(User))]
    public class UserLoginInfoDto : EntityDto<long>
    {
        public string Name { get; set; }

        public string Surname { get; set; }

        public string UserName { get; set; }

        public string EmailAddress { get; set; }

        public string ProfilePictureId { get; set; }

        public long? DoanhNghiepId { get; set; }
        public int? PhongBanId { get; set; }
        public int? RoleLevel { get; set; }
        public string RoleLevelName { get; set; }
        public int? Stt { get; set; }
        public bool? IsDonViTrucThuoc { get; set; }
        public long? ParentId { get; set; }
        public int? TinhId { get; set; }
        public long? HuyenId { get; set; }
        public long? XaId { get; set; }
        public int? TenantId { get; set; }
        // huongcv--
        public List<int> ThuTucEnum { get; set; }

    }
}
