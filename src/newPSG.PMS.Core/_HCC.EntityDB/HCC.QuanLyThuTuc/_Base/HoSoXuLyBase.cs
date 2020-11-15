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
    public class HoSoXuLyBase : FullAuditedEntity<long>
    {
        public int? ThuTucId { get; set; }
        public long HoSoId { get; set; }
        public int? LoaiHoSoId { get; set; }
        public int? QuiTrinh { get; set; }
        public bool? IsHoSoBS { get; set; }
        public DateTime? NgayTiepNhan { get; set; }
        public DateTime? NgayHenTra { get; set; }
        //Đơn vị xử lý
        public int? DonViGui { get; set; }
        public long? NguoiGuiId { get; set; }
        public DateTime? NgayGui { get; set; }
        public string YKienGui { get; set; }
        public int? DonViXuLy { get; set; }
        public long? NguoiXuLyId { get; set; }
        public int? DonViKeTiep { get; set; }
        //Thành phần xử lý
        public long? LanhDaoBoId { get; set; }
        public bool? LanhDaoBoDaDuyet { get; set; }
        public DateTime? LanhDaoBoNgayDuyet { get; set; }
        public bool? LanhDaoBoIsCA { get; set; }
        public DateTime? LanhDaoBoNgayKy { get; set; }
        //==
        public long? LanhDaoCucId { get; set; }
        public bool? LanhDaoCucDaDuyet { get; set; }
        public DateTime? LanhDaoCucNgayDuyet { get; set; }
        public bool? LanhDaoCucIsCA { get; set; }
        public DateTime? LanhDaoCucNgayKy { get; set; }
        //==
        public long? TruongPhongId { get; set; }
        public bool? TruongPhongDaDuyet { get; set; }
        public DateTime? TruongPhongNgayDuyet { get; set; }
        public bool? TruongPhongIsCA { get; set; }
        public DateTime? TruongPhongNgayKy { get; set; }
        //==
        public long? PhoPhongId { get; set; }
        public bool? PhoPhongDaDuyet { get; set; }
        public DateTime? PhoPhongNgayDuyet { get; set; }
        public bool? PhoPhongIsCA { get; set; }
        public DateTime? PhoPhongNgayKy { get; set; }
        //==
        public long? ChuyenVienThuLyId { get; set; }
        public bool? ChuyenVienThuLyDaDuyet { get; set; }
        public DateTime? ChuyenVienThuLyNgayDuyet { get; set; }
        public long? ChuyenVienPhoiHopId { get; set; }
        public bool? ChuyenVienPhoiHopDaDuyet { get; set; }
        public DateTime? ChuyenVienPhoiHopNgayDuyet { get; set; }
        //public bool? ChuyenVienThuLyDaTongHop { get; set; }
        //==
        public long? ChuyenGiaId { get; set; }
        public bool? ChuyenGiaDaDuyet { get; set; }
        public bool? HoiDongThamDinhDaDuyet { get; set; }
        public DateTime? ChuyenGiaNgayDuyet { get; set; }
        public long? ToTruongChuyenGiaId { get; set; }
        public bool? ToTruongChuyenGiaDaDuyet { get; set; }
        public DateTime? ToTruongChuyenGiaNgayDuyet { get; set; }
        //==
        public long? VanThuId { get; set; }
        public bool? VanThuDaDuyet { get; set; }
        public DateTime? VanThuNgayDuyet { get; set; }
        public bool? VanThuIsCA { get; set; }
        public DateTime? VanThuNgayDongDau { get; set; }
        //==
        public bool? HoSoIsDat { get; set; }
        [StringLength(2000)]
        [Column(TypeName = "nvarchar")]
        public string TieuDeCV { get; set; }
        public string NoiDungCV { get; set; }
        public int? TrangThaiCV { get; set; }

        public long? HoSoXuLyHistoryId_Active { get; set; }

        //Kết quả lần xử lý [Công văn, Thông báo, Giấy tiếp nhận]
        [StringLength(2000)]
        [Column(TypeName = "nvarchar")]
        public string DuongDanTepCA { get; set; }
        public DateTime? NgayTraKetQua { get; set; }

        //Kết quả cuối cùng
        [StringLength(2000)]
        [Column(TypeName = "nvarchar")]
        public string GiayTiepNhanCA { get; set; }
        public bool? IsKetThuc { get; set; }

        //Luồng xử lý
        public int? LuongXuLy { get; set; }
        //Tính quá hạn xử lý (Ngày bắt đầu quy trình so với Today)
        public DateTime? NgayBatDauQuyTrinh { get; set; }
    }
}
