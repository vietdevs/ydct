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
    [AutoMap(typeof(PhanLoaiHoSo_Filter))]
    public class PhanLoaiHoSo_FilterDto : PhanLoaiHoSo_Filter
    {
        public Filter_PhanLoaiHoSoDto Filter { get; set; }
    }

    public class PhanLoaiHoSo_Filter_DtoInput
    {
        public int PhanLoaiHoSoId { get; set; }
    }
}