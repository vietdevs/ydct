using Abp.AutoMapper;
using newPSG.PMS.EntityDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace newPSG.PMS.Dto
{
    public class XuLyHoSoChuyenVien37Dto
    {

    }

    public class ThamXet37InputDto
    {
        public long? HoSoId { get; set; }
        public int? TrangThaiXuLy { get; set; }
        public string NoiDungCV { get; set; }
        public string SoCongVan { get; set; }
        public DateTime? NgayYeuCauBoSung { get; set; }
        public string NoiDungYeuCauGiaiQuyet { get; set; }
        public string LyDoYeuCauBoSung { get; set; }
        public string TenCanBoHoTro { get; set; }
        public string DienThoaiCanBo { get; set; }
        public string TenNguoiDaiDien { get; set; }
        public string DiaChiCoSo { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
    }
    public class ThanhvienThamDinhDto
    {
        public long Id { get; set; }
        public string HoTen { get; set; }

    }

    public class HoSoDoanThamDinhInputDto
    {
        public long? HoSoId { get; set; }
        public List<HoSoDoanThamDinh37Dto> ListHoSoDoanThamDinh { get; set; }
    }
    [AutoMap(typeof(TT37_HoSoDoanThamDinh))]
    public class HoSoDoanThamDinh37Dto
    {
        public long? HoSoId { get; set; }
        public long? UserId { get; set; }
        public int? VaiTroEnum { get; set; }
        public string TenVaiTro
        {
            get
            {
                string name = String.Empty;
                if(VaiTroEnum == (int)CommonENum.VAI_TRO_THAM_DINH.TRUONG_DOAN)
                {
                    name =  CommonENum.GetEnumDescription((CommonENum.VAI_TRO_THAM_DINH)CommonENum.VAI_TRO_THAM_DINH.TRUONG_DOAN);
                }
                else if(VaiTroEnum == (int)CommonENum.VAI_TRO_THAM_DINH.THU_KY)
                {
                    name = CommonENum.GetEnumDescription((CommonENum.VAI_TRO_THAM_DINH)CommonENum.VAI_TRO_THAM_DINH.THU_KY);
                }
                else
                {
                    name = CommonENum.GetEnumDescription((CommonENum.VAI_TRO_THAM_DINH)CommonENum.VAI_TRO_THAM_DINH.THANH_VIEN);
                }
                return name;
            }
        }
        public int? TrangThaiXuLy { get; set; }
        public string HoTen { get; set; }
        public string StrTrangThai { get; set; }
        public string NoiDungYKien { get; set; }
    }
    public class HoSoThamDinh37InputDto
    {
        public long? HoSoId { get; set; }
        public int? TrangThaiXuLy { get; set; }
        public string NoiDungYkien { get; set; }
    }
    public class CapNhatKetQuaHoSo37InputDto
    {
        public string BienBanTongHopUrl { get; set; }
        public long? HoSoId { get; set; }
    }

    public class TongHopThamDinhLuu37InputDto
    {
        public long? HoSoId { get; set; }
        public int? TrangThaiXuLy { get; set; }
    }
}
