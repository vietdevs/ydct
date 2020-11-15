using Abp.AutoMapper;
using newPSG.PMS.Dto;
using newPSG.PMS.EntityDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace newPSG.PMS.Dto
{

    [AutoMap(typeof(LoaiHoSo))]
    public class LoaiHoSoDto : LoaiHoSo
    {
        public string PhongBan { set; get; }
        public int? ThuTucIdEnum { get; set; }
        public string TenThuTuc { get; set; }
        public string JsonHanXuLy { get; set; }
        //public bool? IsCoChiTieu { get; set; }
    }
    public class LoaiHoSoHanXuLyDto : LoaiHoSo_HanXuLy
    {
        //public bool? IsCoChiTieu { get; set; }
    }
    public class DanhSachCauHinhDto
    {
        public int? ThuTucId { get; set; }
        public string LoaiHoSoId { get; set; }
        public string MoTa { get; set; }
        public bool? IsActive { get; set; }

        //Cấu hình xử lý
        public int? DonViGui { get; set; }
        public int? DonViNhan { get; set; }
        public int? SoNgayXuLy { get; set; }
        public bool? IsHoSoBS { get; set; }
        public int? LuongXuLy { get; set; }
    }


    public class LoaiHoSoInputDto : PagedAndSortedInputDto
    {
        public int ID { get; set; }
        public string Filter { get; set; }
    }
}
