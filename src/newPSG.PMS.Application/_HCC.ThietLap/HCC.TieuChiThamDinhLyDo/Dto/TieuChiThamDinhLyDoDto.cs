using Abp.AutoMapper;
using newPSG.PMS.EntityDB;

namespace newPSG.PMS.Dto
{
    [AutoMap(typeof(TieuChiThamDinh_LyDo))]
    public class TieuChiThamDinh_LyDoDto : TieuChiThamDinh_LyDo
    {
     
    }

    public class TieuChiThamDinh_LyDoDtoInput : PagedAndSortedInputDto
    {
        public string Filter { get; set; }
    }
}