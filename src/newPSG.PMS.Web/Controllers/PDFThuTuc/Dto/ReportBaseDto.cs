using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace newPSG.PMS.Dto
{

    public class PdfDto
    {
        public string Controller { get; set; }
        public string Action { get; set; }
        public long HoSoId { get; set; }
        [AllowHtml]
        public string NoiDungCV { get; set; }
        public bool? HoSoIsDat { get; set; }
        public string NoiDungThamXetJson { get; set; }
        public string YKienChung { get; set; }
        public string YKienBoSung { get; set; }
        public bool? IsXemTruoc { get; set; }
        [AllowHtml]
        public string HeaderCV { get; set; }
        [AllowHtml]
        public string FooterCV { get; set; }
        public int? TinhId { get; set; }
        public string JsonThamDinhCoSo { get; set; }
        public int? TrangThaiCV { get; set; }
        public string TenCoSo { get; set; }
        public string DiaChiCoSo { get; set; }
        [AllowHtml]
        public string LoaiHinhDangKyPhuHop { get; set; }
        public string SoCongVan { get; set; }
        public DateTime? NgayCongVan { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public string TenNguoiDaiDien { get; set; }
    }

    public class ReportBaseDto
    {
        public long HoSoId { get; set; }
        [AllowHtml]
        public string NoiDungCV { get; set; }
        [AllowHtml]
        public string HeaderCV { get; set; }
        [AllowHtml]
        public string FooterCV { get; set; }

        public bool ExportExcel { get; set; } = false;
        public int? TrangThaiCV { get; set; }
        public bool? HoSoIsDat { get; set; }
        public int? LuongXuLy { get; set; }
        public bool? IsXemTruoc { get; set; }
    }

    public class ReportTT37Dto
    {
        public long HoSoId { get; set; }
        [AllowHtml]
        public string NoiDungCV { get; set; }
        public string SoCongVan { get; set; }
        public DateTime? NgayCongVan { get; set; }
        public DateTime? NgayYeuCauBoSung { get; set; }
        public string NoiDungYeuCauGiaiQuyet { get; set; }
        public string LyDo { get; set; }
        public string TenCanBoHoTro { get; set; }
        public string DienThoaiCanBo { get; set; }
        public string TenNguoiDaiDien { get; set; }
        public string DiaChiCoSo { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
    }
    public class CongVan
    {
        public int Id { get; set; }
        public string TenCongVan { get; set; }
    }
}