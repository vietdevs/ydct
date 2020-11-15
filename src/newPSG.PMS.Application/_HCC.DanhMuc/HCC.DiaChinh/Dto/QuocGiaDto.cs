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
    [AutoMap(typeof(QuocGia))]
    public class QuocGiaDto: QuocGia
    {
    }
    public class QuocGiaInputDto : PagedAndSortedInputDto
    {
        public string Filter { get; set; }
    }
}
