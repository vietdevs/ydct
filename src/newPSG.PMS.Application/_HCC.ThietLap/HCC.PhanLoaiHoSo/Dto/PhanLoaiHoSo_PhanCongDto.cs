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
    [AutoMap(typeof(PhanLoaiHoSo_PhanCong))]
    public class PhanLoaiHoSo_PhanCongDto : PhanLoaiHoSo_PhanCong
    {
        public string StrLoaiBienBanThamDinh { get; set; }
    }

    public class PhanLoaiHoSo_PhanCong_CreateInputDto
    {
        public int PhanLoaiHoSoId { get; set; }
        public List<PhanLoaiHoSo_PhanCongDto> ListPhanLoaiHoSo_PhanCong { get; set; }
    }
}