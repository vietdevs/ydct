using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace newPSG.PMS.EntityDB
{
    //Thong tin ho so
    [Table("TT37_HoSo")]
    public class HoSo37 : HoSoBase
    {
        public DateTime? NgayDeNghi { get; set; }
        public string HoTenNguoiDeNghi { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string DiaChiCuTru { get; set; }
        public string SoNhanBiet { get; set; }
        public DateTime? NgayCap { get; set; }
        public string NoiCap { get; set; }
        public string DienThoaiNguoiDeNghi { get; set; }
        public string EmailNguoiDeNghi { get; set; }
        public int? VanBangChuyenMon { get; set; }


    }
}