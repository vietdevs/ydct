using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace newPSG.PMS.EntityDB
{
    [Table("TT37_HoSoXuLy")]
    public class HoSoXuLy37 : HoSoXuLyBase
    {
        //Thông tin thẩm xét
        public long? BienBanThamDinhId_ChuyenVienThuLy { get; set; }
        public long? BienBanThamDinhId_ChuyenVienPhoiHop { get; set; }
        [StringLength(2000)]
        [Column(TypeName = "nvarchar")]
        public string YKienChung { get; set; }
        public DateTime? NgayTraGiayTiepNhan { get; set; }
        public string SoGiayTiepNhan { get; set; }
        public DateTime? NgayHenCap { get; set; }
        [Column(TypeName = "ntext")]
        public string HinhThucCapCTJson { get; set; }
        [Column(TypeName = "ntext")]
        public string TaiLieuDaNhanJson { get; set; }
        public int? TrangThaiXuLy { get; set; }
        public string LyDoTraLai { get; set; }

        public string SoCongVan { get; set; }
        public DateTime? NgayYeuCauBoSung { get; set; }
        public string NoiDungYeuCauGiaiQuyet { get; set; }
        public string LyDoYeuCauBoSung { get; set; }
        public string TenCanBoHoTro { get; set; }
        public string DienThoaiCanBo { get; set; }

        [StringLength(2000)]
        [Column(TypeName = "nvarchar")]
        public string BienBanTongHopUrl { get; set; }

        public DateTime? NgayLapDoanThamDinh { get; set; }
        public long? NguoiLapDoanThamDinhId { get; set; }
    }
}