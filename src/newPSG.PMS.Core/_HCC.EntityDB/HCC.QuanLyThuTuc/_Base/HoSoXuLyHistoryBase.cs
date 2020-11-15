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
    public class HoSoXuLyHistoryBase : FullAuditedEntity<long>
    {
        public int? ThuTucId { get; set; }
        public int? LoaiHoSoId { get; set; }
        public long? HoSoXuLyId { get; set; }
        public long? HoSoId { get; set; }
        public bool? IsHoSoBS { get; set; }
        //Người xử lý, cho ý kiến
        public int? DonViXuLy { get; set; } //=DonViGui
        public long? NguoiXuLyId { get; set; }
        public DateTime? NgayXuLy { get; set; } //=NgayGui
        public string NoiDungYKien { get; set; }
        public int? DonViKeTiep { get; set; }//=DonViNhan
        //Lịch sử kết quả
        public bool? HoSoIsDat_Pre { get; set; }
        public bool? HoSoIsDat { get; set; }
        public int? TrangThaiCV_Pre { get; set; }
        public int? TrangThaiCV { get; set; }
        public bool? IsSuaCV { get; set; }
        [StringLength(2000)]
        [Column(TypeName = "nvarchar")]
        public string TieuDeCV { get; set; }
        [Column(TypeName = "ntext")]
        public string NoiDungCV { get; set; }
        //Chức năng chuyển nhanh
        public bool? IsChuyenNhanh { get; set; }
        [StringLength(2000)]
        [Column(TypeName = "nvarchar")]
        public string LyDoChuyenNhanh { get; set; }
        //Loại hành động xử lý
        public int? ActionEnum { get; set; }
        //Cán bộ xử lý
        public long? LanhDaoBoId { get; set; }
        public long? LanhDaoCucId { get; set; }
        public long? TruongPhongId { get; set; }
        public long? PhoPhongId { get; set; }
        public long? ChuyenVienThuLyId { get; set; }
        public long? ChuyenVienPhoiHopId { get; set; }
        public long? ChuyenGiaId { get; set; }
        public long? ToTruongChuyenGiaId { get; set; }
        public long? VanThuId { get; set; }
        public long? KeToanId { get; set; }
        public long? DoanhNghiepId { get; set; }
        [Column(TypeName = "ntext")]
        public string FullJson { get; set; }

        public bool? IsKetThuc { get; set; }

        //Luồng xử lý
        public int? LuongXuLy { get; set; }

        //Tính quá hạn
        public DateTime? NgayBatDauQuyTrinh { get; set; }
        public int? LoaiHoSo_HanXuLyId { get; set; }
    }
}
