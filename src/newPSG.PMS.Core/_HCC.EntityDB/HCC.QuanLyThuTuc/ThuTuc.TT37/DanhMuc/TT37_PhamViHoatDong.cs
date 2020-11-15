using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace newPSG.PMS.EntityDB
{
    [Table("TT37_PhamViHoatDong")]
    public class TT37_PhamViHoatDong: CreationAuditedEntity
    {
        public string Ten { get; set; }
        public string MoTa { get; set; }
        public bool? IsActive { get; set; }
        public CommonOnject Chung { get; set; }
    }
    public class CommonOnject
    {
        public string Name { get; set; }
        public DateTime? BirthDay { get; set; }
        public string Province { get; set; }
        public int? Age { get; set; }
    }
}
