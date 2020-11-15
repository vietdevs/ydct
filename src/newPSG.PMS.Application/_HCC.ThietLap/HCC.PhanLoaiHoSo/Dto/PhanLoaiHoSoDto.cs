using Abp.AutoMapper;
using newPSG.PMS.Dto;
using newPSG.PMS.EntityDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace newPSG.PMS.Dto
{
    [AutoMap(typeof(PhanLoaiHoSo))]
    public class PhanLoaiHoSoDto : PhanLoaiHoSo
    {
        public string StrThuTuc
        {
            get
            {
                if (this.ThuTucId.HasValue)
                    return CommonENum.GetEnumDescription((CommonENum.THU_TUC_ID)this.ThuTucId);
                else return null;
            }
        }

        public List<PhanLoaiHoSo_PhanCongDto> ListLoaiHoSo_BienBan { get; set; }

       public List<PhanLoaiHoSo_FilterDto> ListLoaiHoSo_Filter { get; set; }

    }

    public class PhanLoaiHoSoDtoInput : PagedAndSortedInputDto
    {
        public string Filter { get; set; }
        public int? ThuTucId { get; set; }
    }

    [AutoMap(typeof(HoSoXuLy_PhanCongSoLuong))]
    public class Setting_PhanCong_TieuBanDto : HoSoXuLy_PhanCongSoLuong
    {
        public string StrTieuBan
        {
            get
            {
                return CommonENum.GetEnumDescription((CommonENum.TIEU_BAN_CHUYEN_GIA)this.TieuBanEnum);
            }
        }
    }
}