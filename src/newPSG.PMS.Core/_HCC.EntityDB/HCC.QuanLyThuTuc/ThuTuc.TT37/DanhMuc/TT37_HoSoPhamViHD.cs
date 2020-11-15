using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace newPSG.PMS.EntityDB
{
    [Table("TT37_HoSoPhamViHD")]
    public class TT37_HoSoPhamViHD : CreationAuditedEntity<long>
    {
        public long? HoSoId { get; set; }
        public int? PhamViHoatDongId { get; set; }
    }
}
