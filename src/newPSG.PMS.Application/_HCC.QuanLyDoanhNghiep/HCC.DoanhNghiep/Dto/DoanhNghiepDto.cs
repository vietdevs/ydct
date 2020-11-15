using Abp.AutoMapper;
using newPSG.PMS.Dto;
using newPSG.PMS.EntityDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace newPSG.PMS.DoanhNghiepInput
{
    [AutoMap(typeof(DoanhNghiep))]
    public class DoanhNghiepDto : DoanhNghiep
    {
        public bool? UserActive { get; set; }
        public long? UserId { get; set; }
        public string TenChucVuNguoiDaiDien { get; set; }
        public List<ThongTinPhapLy> ThongTinPhapLy { get; set; }

        public ChuKyDto chuKy { get; set; }
        public DoanhNghiepDto()
        {
            ThongTinPhapLy = new List<ThongTinPhapLy>();
            chuKy = new ChuKyDto();
        }
    }

    [AutoMap(typeof(DoanhNghiep))]
    public class DoanhNghiepInfoInput : DoanhNghiep
    {
        public string Name { get; set; }
        public string SurName { get; set; }
        public List<NguoiLienHe> NguoiLienHes { get; set; }
        public List<ThongTinPhapLy> GiayPhepPhapLys { get; set; }

        public DoanhNghiepInfoInput()
        {
            NguoiLienHes = new List<NguoiLienHe>();
            GiayPhepPhapLys = new List<ThongTinPhapLy>();
        }
    }

    public class NguoiLienHe
    {
        public string HoTen { get; set; }
        public int? ChucVuId { get; set; }
        public string DienThoai { get; set; }
        public string Email { get; set; }
    }
    public class TepHoSo: ThongTinPhapLy
    {
        public int? Index { get; set; }
    }
    public class CreateOrUpdateDoanhNghiepInfoInput
    {
        public DoanhNghiepInfoInput DoanhNghiep;
    }
    public class GetDoanhNghiepInput : PagedAndSortedInputDto
    {
        public string Filter { get; set; }
        public string MaSoThue { get; set; }
        public int? TinhId { get; set; }
        public int? LoaiHinhDoanhNghiepId { get; set; }
        public bool? IsActive { get; set; }
        public int? FormCase { get; set; }
    }
    public class CheckEmail
    {
        public string Email { get; set; }
    }
    public class ChangeDoanhNghiepPasswordInput
    {
        public string MaSoThue { get; set; }
        public string Password { get; set; }
    }

    public class DuyetDoanhNghiepInput
    {
        public long Id { get; set; }
        public string LyDoKhongDuyet { get; set; }
    }
}
