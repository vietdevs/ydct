using Abp.AutoMapper;
using newPSG.PMS.Dto;
using newPSG.PMS.EntityDB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace newPSG.PMS.Dto
{
    public class DangKyCongBoLTDto : VFADangKyCongBo
    {
        public string StrTinh { get; set; }
        public string StrLoaiHoSo { get; set; }
    }

    public class PagingDangKyCongBoLTInput : PagedAndSortedInputDto
    {
        public int? FormCase { get; set; }

        public string Keyword { get; set; }
        public DateTime? NgayTraKetQuaTu { get; set; }
        public DateTime? NgayTraKetQuaToi { get; set; }

        public long? LoaiHoSoId { get; set; }
        public int? TinhId { get; set; }
        public int? DoanhNghiepId { get; set; }
        public bool? IsChiCuc { get; set; }
        public int? ChiCucId { get; set; }
        public int? ThuTucId { get; set; }
    }

    [AutoMap(typeof(DangKyCongBoLTDto))]
    public class CallApiDangKyCongBoInput : DangKyCongBoLTDto
    {

    }


    public class DangKyCongBoRequest
    {
        [JsonProperty(PropertyName = "thong_tin_doanh_nghiep")]
        public DoanhNghiepModel DoanhNghiep { get; set; }
        [JsonProperty(PropertyName = "dang_ky_cong_bo")]
        public DangKyCongBoModel DangKyCongBo { get; set; }

        public DangKyCongBoRequest()
        {
            DoanhNghiep = new DoanhNghiepModel();
            DangKyCongBo = new DangKyCongBoModel();
        }
    }

    [AutoMap(typeof(CallApiDangKyCongBoInput))]
    public class DangKyCongBoModel
    {
        [JsonProperty(PropertyName = "guid_id")]
        public string Guid { get; set; }
        [JsonProperty(PropertyName = "ten_san_pham_dang_ky")]
        public string TenSanPhamDangKy { get; set; }
        [JsonProperty(PropertyName = "thanh_phan_san_pham_dang_ky")]
        public string ThanhPhanSanPhamDangKy { get; set; }

        [JsonProperty(PropertyName = "chi_tieu_chat_luong_chu_yeu")]
        public List<ChiTieuChatLuong> ChiTieus { get; set; }

        [JsonProperty(PropertyName = "thoi_han_su_dung")]
        public string ThoiHanSuDung { get; set; }
        [JsonProperty(PropertyName = "chat_lieu_bao_bi_va_quy_cach_dong_goi")]
        public string ChatLieuBaoBiVaQuyCachDongGoi { get; set; }
        [JsonProperty(PropertyName = "ten_va_dia_chi_co_so_san_xuat")]
        public string TenVaDiaChiCoSoSanXuat { get; set; } //Gop chung
        [JsonProperty(PropertyName = "nhom_san_pham_dkcb_id")]
        public int? NhomSanPhamDKCBId { get; set; } 
        [JsonProperty(PropertyName = "quy_chuan_ky_thuat_quoc_gia")]
        public string QuyChuanKyThuatQuocGia { get; set; }
        [JsonProperty(PropertyName = "tieu_chuan_quoc_gia")]
        public string TieuChuanQuocGia { get; set; }
        [JsonProperty(PropertyName = "thong_tu")]
        public string ThongTu { get; set; }
        [JsonProperty(PropertyName = "tieu_chuan_quoc_te")]
        public string TieuChuanQuocTe { get; set; }
        [JsonProperty(PropertyName = "quy_chuan_dia_phuong")]
        public string QuyChuanDiaPhuong { get; set; }
        [JsonProperty(PropertyName = "tieu_chuan_nha_san_xuat")]
        public string TieuChuanNhaSanXuat { get; set; }
        [JsonProperty(PropertyName = "is_nhap_khau")]
        public bool? IsNhapKhau { get; set; }
        [JsonProperty(PropertyName = "quoc_gia_nhap_khau_id")]
        public int? QuocGiaNhapKhauId { get; set; }
        [JsonProperty(PropertyName = "ngay_cap_chung_nhan")]
        public string NgayCapChungNhan { get; set; }
        
        [JsonProperty(PropertyName = "giay_xac_nhan_to_base64_string")]
        public string GiayXacNhanToBase64String { get; set; }

        public DangKyCongBoModel()
        {
            ChiTieus = new List<ChiTieuChatLuong>();
        }
    }

    public class ChiTieuChatLuong
    {
        [JsonProperty(PropertyName = "ten_chi_tieu")]
        public string TenChiTieu { get; set; }
        [JsonProperty(PropertyName = "don_vi_tinh")]
        public string DonViTinh { get; set; }
        [JsonProperty(PropertyName = "muc_cong_bo")]
        public string MucCongBo { get; set; }
    }
}
