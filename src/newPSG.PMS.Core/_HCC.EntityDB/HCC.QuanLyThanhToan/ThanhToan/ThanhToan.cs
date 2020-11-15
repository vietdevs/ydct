using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace newPSG.PMS.EntityDB
{
    [Table("ThanhToan")]
    public class ThanhToan : FullAuditedEntity<long>
    {
        public int? TenantId { get; set; }
        public int? PhanHeId { get; set; }
        public long? HoSoId { get; set; }
        [StringLength(2000)]
        [Column(TypeName = "nvarchar")]
        public string ArrHoSoId { get; set; }
        public int? KenhThanhToan { get; set; }

        //Thông tin thanh toán
        public long? DoanhNghiepId { get; set; }
        public decimal? PhiDaNop { get; set; }
        public decimal? PhiXacNhan { get; set; }
        [StringLength(500)]
        [Column(TypeName = "nvarchar")]
        public string SoTaiKhoanNop { get; set; }
        [StringLength(500)]
        [Column(TypeName = "nvarchar")]
        public string SoTaiKhoanHuong { get; set; }
        [StringLength(500)]
        [Column(TypeName = "nvarchar")]
        public string MaGiaoDich { get; set; }
        public DateTime? NgayGiaoDich { get; set; }
        [StringLength(500)]
        [Column(TypeName = "nvarchar")]
        public string MaDonHang { get; set; }
        [StringLength(2000)]
        [Column(TypeName = "nvarchar")]
        public string DuongDanHoaDonThanhToan { get; set; }
        [StringLength(500)]
        [Column(TypeName = "nvarchar")]
        public string GhiChu { get; set; }

        //Kết quả giao dịch
        public int? TrangThaiNganHang { get; set; } //lay trong enum Trang thai thanh toan
        [StringLength(2000)]
        [Column(TypeName = "nvarchar")]
        public string KetQuaGiaoDichJson { get; set; }
        [Column(TypeName = "ntext")]
        public string RequestJson { get; set; }
        [Column(TypeName = "ntext")]
        public string ResponseJson { get; set; }

        //Kế toán xử lý
        public int? TrangThaiKeToan { get; set; }  //lay enum trang thai ke toan
        [StringLength(2000)]
        [Column(TypeName = "nvarchar")]
        public string LyDoHuyThanhToan { get; set; }
        [StringLength(2000)]
        [Column(TypeName = "nvarchar")]
        public string YKienXacNhanThanhToan { get; set; }
        public DateTime? NgayXacNhanThanhToan { get; set; }

        //Luồng xử lý
        public int? LuongXuLy { get; set; }
    }
}
