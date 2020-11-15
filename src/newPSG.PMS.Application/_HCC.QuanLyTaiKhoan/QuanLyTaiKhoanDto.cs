using newPSG.PMS.Authorization.Users.Dto;
using System;
using System.ComponentModel.DataAnnotations;

namespace newPSG.PMS.Dto
{
    public class QuanLyTaiKhoanInputPageDto: PagedAndSortedInputDto
    {
        public string Filter { get; set; }
        public CommonENum.LOAI_TAI_KHOAN LoaiTaiKhoan { get; set; }
    }
    public class CreateOrUpdateUser_CustomInput
    {
        [Required]
        public UserEditDto User { get; set; }
        public string[] AssignedRoleNames { get; set; }
        public bool SetRandomPassword { get; set; }
        public CommonENum.LOAI_TAI_KHOAN? LoaiTaiKhoan { get; set; }
    }
    public class GetForEditInput
    {
        public long? Id { get; set; }
        public CommonENum.LOAI_TAI_KHOAN LoaiTaiKhoan { get; set; }
    }


    public class QuanLyTaiKhoanPageListDto
    {
        public long Id { get; set; }// User Id
        public string Name { get; set; }

        public string Surname { get; set; }
        public string FullName { get
            {
                return Surname + " " + Name;
            }
        }
        public string StrPhongBan { get; set; }

        public string UserName { get; set; }

        public string EmailAddress { get; set; }

        public string PhoneNumber { get; set; }


        public DateTime? LastLoginTime { get; set; }

        public bool IsActive { get; set; }

        public bool? IsTrongCuc { get; set; }
        public int? DonViChuyenGiaId { get; set; }
        public string StrDonViChuyenGia { get; set; }
        public int? Role_Level { get; set; }
        public string StrVaiTro
        {
            get
            {
                if (Role_Level.HasValue)
                    return CommonENum.GetEnumDescription((CommonENum.ROLE_LEVEL)this.Role_Level.Value);
                else
                    return "";
            }
        }

    }
}
