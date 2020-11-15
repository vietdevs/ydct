using System;
using System.Collections.Generic;

namespace newPSG.PMS.Dto
{
    public class VanThuRaSoatHoSo37Input
    {
        public long HoSoXuLyId { get; set; }
        public long HoSoId { get; set; }
        public string LyDoTuChoi { get; set; }
        public int? TrangThaiXuLy { get; set; }
        public decimal? PhiDaNop { get; set; }
    }

    public class VanThuXuLy37InputDto
    {
        public long HoSoXuLyId { get; set; }
        public long HoSoId { get; set; }
        public string NoiDungYKien { get; set; }
        public int? TrangThaiXuLy { get; set; }

        //File PDF Ký
        public string TenTepCA { get; set; }
        public string DuongDanTepCA { get; set; }
        public string GiayTiepNhanCA { get; set; } //Giấy tiếp nhận full
    }

    public class PhieuTiepNhanInputDto
    {
        public long HoSoId { get; set; }
        public DateTime? NgayHenCap { get; set; }
        public decimal? PhiDaNop { get; set; }
        public List<PhieuTiepNhanDto> HinhThucCapChungChi { get; set; }
        public List<PhieuTiepNhanDto> ListTaiLieuDaNhan { get; set; }
        public string GiayTiepNhanCA { get; set; }
        public int? SoLanTiepNhanBoSung { get; set; }
    }
    public class PhieuTiepNhanDto
    {
        public int STT { get; set; }
        public string Name { get; set; }
        public bool? Value { get; set; }
        public string StrCheck
        {
            get
            {
                if (Value.HasValue && Value.Value == true)
                    return "x";
                return "";
            }
        }
    }

    public class TiepNhanBoSungReport37Dto
    {
        public int? SoLanBoSung { get; set; }
        public string NgayTraGiayTiepNhanStr { get; set; }
    }

}