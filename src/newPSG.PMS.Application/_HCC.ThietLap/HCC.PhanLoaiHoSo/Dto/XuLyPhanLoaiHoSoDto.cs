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
    public class XuLyPhanLoaiHoSoInputDto
    {
        public int? ThuTucId { get; set; }
        public int? RoleLevel { get; set; }
        public object Filter { get; set; }
    }

    public class XuLyPhanLoaiHoSo_CheckDto
    {
        public int? PhanLoaiHoSoId { get; set; }
        public int? Deplicate { get; set; }
    }

    public class Filter_PhanLoaiHoSoDto
    {
        public int? PhanLoaiHoSo { get; set; }
        //public PhanLoaiThuoc90Dto PhanLoaiThuoc { get; set; }
        //public PhanLoaiHoSo90Dto PhanLoaiHoSo { get; set; }
        //public PhanLoaiHinhThucSanXuat90Dto PhanLoaiHinhThucSanXuat { get; set; }
    }
}