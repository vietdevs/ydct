using Abp.AutoMapper;
using newPSG.PMS.Dto;
using newPSG.PMS.EntityDB;

namespace newPSG.PMS.Dto
{
    [AutoMap(typeof(CuaKhau))]
    public class CuaKhauDto : CuaKhau
    {     
    }
    public class CuaKhauInputDto : PagedAndSortedInputDto
    {
        public string Filter { get; set; }
    }
}
