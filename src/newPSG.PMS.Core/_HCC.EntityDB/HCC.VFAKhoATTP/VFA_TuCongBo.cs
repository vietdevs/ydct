using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace newPSG.PMS.EntityDB
{
    [Table("VFA_TuCongBo")]
    public class VFA_TuCongBo : HoSoBase, IVFA_Base
    {
        public long? HoSoId { get; set; }
        public long? LienThongId { get; set; }
        [Column(TypeName = "ntext")]
        public string TepDinhKemKQJson { get; set; }
        [Column(TypeName = "ntext")]
        public string KetQuaXuLyJson { get; set; } //Object
        public int? TrangThaiLienThong { get; set; }
        public DateTime? NgayLienThong { get; set; }
        public DateTime? NgayLienThongThanhCong { get; set; }
        [StringLength(500)]
        [Column(TypeName = "nvarchar")]
        public string Guid { get; set; }

        #region HoSo01
        //Thong tin so giay chung nhan
        [StringLength(2000)]
        public string SoGiayChungNhanATTP { get; set; }
        public DateTime? NgayCapGiayChungNhanATTP { get; set; }
        [StringLength(2000)]
        public string NoiCapGiayChungNhanATTP { get; set; }

        //Thong tin san pham
        [StringLength(2000)]
        public string TenSanPhamDangKy { get; set; }
        [StringLength(2000)]
        public string ThanhPhanSanPhamDangKy { get; set; }

        [StringLength(2000)]
        public string ThoiHanSuDung { get; set; }

        [StringLength(2000)]
        public string ChatLieuBaoBi { get; set; }

        [StringLength(2000)]
        public string QuyCachDongGoi { get; set; }

        [StringLength(2000)]
        public string TenCoSoSanXuat { get; set; }

        [StringLength(2000)]
        public string DiaChiCoSoSanXuat { get; set; }
        public int? NhomSanPhamId { get; set; }

        //Yeu cau ve an toan thuc pham
        public int? LoaiTieuChuanQuyChuan { get; set; }
        [StringLength(2000)]
        public string QuyChuanKyThuatQuocGia { get; set; }
        [StringLength(2000)]
        public string TieuChuanQuocGia { get; set; }
        [StringLength(2000)]
        public string ThongTu { get; set; }
        [StringLength(2000)]
        public string TieuChuanQuocTe { get; set; }
        [StringLength(2000)]
        public string QuyChuanDiaPhuong { get; set; }
        [StringLength(2000)]
        public string TieuChuanNhaSanXuat { get; set; }
        public bool? IsNhapKhau { get; set; }
        public int? QuocGiaNhapKhauId { get; set; }

        //Thong tin xu ly
        public DateTime? NgayTuCongBo { get; set; }
        #endregion
    }
}
