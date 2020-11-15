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

    [AutoMap(typeof(LoaiFile))]
    public class LoaiFileDto : LoaiFile
    {

    }
    
}
