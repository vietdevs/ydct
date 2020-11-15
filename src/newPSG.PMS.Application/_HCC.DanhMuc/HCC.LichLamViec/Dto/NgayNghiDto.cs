using Abp.AutoMapper;
using newPSG.PMS.Dto;
using newPSG.PMS.EntityDB;
using System;

namespace newPSG.PMS.Dto
{
    [AutoMap(typeof(NgayNghi))]
    public class NgayNghiDto: NgayNghi
    {
    }
    public class NgayNghiInputDto : PagedAndSortedInputDto
    {
        public string Filter { get; set; }
    }
}
