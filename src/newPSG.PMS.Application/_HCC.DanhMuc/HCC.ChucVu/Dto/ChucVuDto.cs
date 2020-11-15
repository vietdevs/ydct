using Abp.AutoMapper;
using newPSG.PMS.Dto;
using newPSG.PMS.EntityDB;

namespace newPSG.PMS.Dto
{
    [AutoMap(typeof(ChucVu))]
    public class ChucVuDto: ChucVu
    {     
    }
    public class ChucVuInputDto : PagedAndSortedInputDto
    {
        public string Filter { get; set; }
    }
}
