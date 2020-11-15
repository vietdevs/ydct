using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static newPSG.PMS.CommonENum;

namespace newPSG.PMS.EntityDB
{
    public class HoSoBase : FullAuditedEntity<long>
    {
        public int? ThuTucId { get; set; }
        public long? PId { get; set; }
        public int? LoaiHoSoId { get; set; }
        [StringLength(2000)]
        [Column(TypeName = "nvarchar")]
        public string TenLoaiHoSo { get; set; } //Tối ưu
        public int? QuiTrinh { get; set; }

        //Thông tin doanh nghiệp lúc đăng ký
        public long? DoanhNghiepId { get; set; }
        public int? TinhId { get; set; }
        public long? HuyenId { get; set; }
        public long? XaId { get; set; }
        [StringLength(2000)]
        [Column(TypeName = "nvarchar")]
        public string DiaChi { get; set; }
        [StringLength(2000)]
        [Column(TypeName = "nvarchar")]
        public string TenDoanhNghiep { get; set; }
        [StringLength(2000)]
        [Column(TypeName = "nvarchar")]
        public string TenNguoiDaiDien { get; set; }
        [StringLength(500)]
        [Column(TypeName = "nvarchar")]
        public string SoDienThoai { get; set; }
        [StringLength(500)]
        [Column(TypeName = "nvarchar")]
        public string Email { get; set; }
        [StringLength(500)]
        [Column(TypeName = "nvarchar")]
        public string Fax { get; set; }
        [StringLength(500)]
        [Column(TypeName = "nvarchar")]
        public string Website { get; set; }
        [StringLength(500)]
        [Column(TypeName = "nvarchar")]
        public string MaSoThue { get; set; }

        //Thông tin hồ sơ
        [StringLength(500)]
        [Column(TypeName = "nvarchar")]
        public string SoDangKy { get; set; }
        [StringLength(500)]
        [Column(TypeName = "nvarchar")]
        public string MaHoSo { get; set; }

        //Ký số hồ sơ
        public bool? IsCA { get; set; }
        public bool? OnIsCA { get; set; }
        [StringLength(2000)]
        [Column(TypeName = "nvarchar")]
        public string DuongDanTepCA { get; set; }
        public DateTime? NgayKySo { get; set; }
        [StringLength(500)]
        [Column(TypeName = "nvarchar")]
        public string PathChuKySo { get; set; }

        //Thanh toán
        public long? ThanhToanId_Active { get; set; }
        public DateTime? NgayThanhToan { get; set; }
        public long? KeToanId { get; set; }
        public DateTime? NgayXacNhanThanhToan { get; set; }

        //Nộp hồ sơ
        public DateTime? NgayTiepNhan { get; set; }
        public DateTime? NgayTraKetQua { get; set; }
        public long? HoSoXuLyId_Active { get; set; }

        //Một cửa chuyển
        public DateTime? NgayNopRaSoat { get; set; }
        public bool? IsChuyenAuto { get; set; }
        public DateTime? NgayChuyenAuto { get; set; }
        public long? MotCuaChuyenId { get; set; }
        public DateTime? NgayMotCuaChuyen { get; set; }
        public int? PhongBanId { get; set; }

        //Trạng thái hồ sơ
        public bool? IsHoSoBS { get; set; }
        public int? TrangThaiHoSo { get; set; }
        public bool? IsHoSoUuTien { get; set; }

        //Nơi tiếp nhận hồ sơ
        public bool? IsChiCuc { get; set; }
        public int? ChiCucId { get; set; }

        //Kết quả hồ sơ
        [StringLength(2000)]
        [Column(TypeName = "nvarchar")]
        public string GiayTiepNhan { get; set; }
        [StringLength(2000)]
        [Column(TypeName = "nvarchar")]
        public string GiayTiepNhanFull { get; set; }
        public string SoGiayTiepNhan { get; set; }
        
        //Luồng xử lý
        public int? LuongXuLy { get; set; }

        //Thư mục chứa
        public int? NamHoSo { get; set; }
        public int? SttHoSo { get; set; }
        public string StrThuMucHoSo { get; set; }

        //Tệp đính kèm
        [Column(TypeName = "ntext")]
        public string TepDinhKemJson { get; set; }
    }
}
