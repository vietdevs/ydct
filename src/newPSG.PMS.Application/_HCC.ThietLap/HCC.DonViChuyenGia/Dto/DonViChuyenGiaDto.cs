using Abp.AutoMapper;
using newPSG.PMS.EntityDB;

namespace newPSG.PMS.Dto
{
    [AutoMap(typeof(DonViChuyenGia))]
    public class DonViChuyenGiaDto : DonViChuyenGia
    {
        public string StrTinh { get; set; }
        public string StrHuyen { get; set; }
        public string StrXa { get; set; }
        public string TenTruongDonVi { get; set; }
        public string StrThuTuc
        {
            get
            {
                if (this.ThuTucId.HasValue)
                    return CommonENum.GetEnumDescription((CommonENum.THU_TUC_ID)this.ThuTucId);
                else return null;
            }
        }
    }

    public class DonViChuyenGiaDtoInput : PagedAndSortedInputDto
    {
        public string Filter { get; set; }
    }
}