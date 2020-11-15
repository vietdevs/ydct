using Abp.AutoMapper;
using newPSG.PMS.EntityDB;

namespace newPSG.PMS.Dto
{
    [AutoMap(typeof(CauHinhChung))]
    public class CauHinhChungDto : CauHinhChung
    {
    }

    public class CauHinhChungInputDto : PagedAndSortedInputDto
    {
        public string Filter { get; set; }
    }
}