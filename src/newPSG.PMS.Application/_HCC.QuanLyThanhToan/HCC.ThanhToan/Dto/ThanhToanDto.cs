using Abp.AutoMapper;
using newPSG.PMS.Dto;
using newPSG.PMS.EntityDB;
using newPSG.PMS.KeypayServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace newPSG.PMS.Dto
{
    [AutoMap(typeof(ThanhToan))]
    public class ThanhToanDto : ThanhToan
    {
        public int? ThuTucId { get; set; }
        public int? TotalRows { get; set; }
        public string StrKenhThanhToan { get; set; }

        //Thông tin hồ sơ
        public string MaHoSo { get; set; }
        public string SoDangKy { get; set; }
        public string TenDoanhNghiep { get; set; }
        public string TenSanPhamDangKy { get; set; }
        public int? LoaiHoSoId { get; set; }
        public string StrLoaiHoSo { get; set; }
        public bool? IsChiCuc { get; set; }
        public int? ChiCucId { get; set; }
        public int? TrangThaiHoSo { get; set; }
        public int? TinhId { get; set; }
        public string StrTinh { get; set; }
        public int? PhongBanId { get; set; }
        public string DiaChi { get; set; }

        //thông tin thanh toán
        public long? ThanhToanId_Active { get; set; }
        public string StrNgayGiaoDich { get; set; }
        public string StrTrangThaiNganHang { get; set; }
        public string StrTrangThaiKeToan { get; set; }

        public string TenSanPham { get; set; }
        public string JsonSanPham { get; set; }
        public string TenCoSo { get; set; }
        public string DiaChiCoSo { get; set; }
        public string HoTenChuCoSo { get; set; }
    }

    public class KeypayDto : Keypay
    {
        public int? TenantId { get; set; }
        public long? ThanhToanId { get; set; }
    }
}
