using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace newPSG.PMS.EntityDB
{
    [Table("LoaiHoSo_HanXuLy")]
    public class LoaiHoSo_HanXuLy : CreationAuditedEntity, IPassivable
    {
        public int? ThuTucId { get; set; }
        public string LoaiHoSoId { get; set; }
        public string MoTa { get; set; }
        public bool IsActive { get; set; }

        //Cấu hình xử lý
        public int? DonViGui { get; set; }
        public int? DonViNhan { get; set; }
        public int? SoNgayXuLy { get; set; }
        public bool? IsHoSoBS { get; set; }
        public int? LuongXuLy { get; set; }

        public string JsonHanXuLy { get; set; }
    }
}
