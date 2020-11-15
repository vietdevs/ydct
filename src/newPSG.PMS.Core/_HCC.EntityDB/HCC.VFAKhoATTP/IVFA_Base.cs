using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace newPSG.PMS.EntityDB
{
    public interface IVFA_Base
    {
        long? HoSoId { get; set; }
        long? LienThongId { get; set; }

        [Column(TypeName = "ntext")]
        string TepDinhKemKQJson { get; set; }
        [Column(TypeName = "ntext")]
        string TepDinhKemJson { get; set; } //Array
        [Column(TypeName = "ntext")]
        string KetQuaXuLyJson { get; set; } //Object
        int? TrangThaiLienThong { get; set; }//
        DateTime? NgayLienThong { get; set; }
        DateTime? NgayLienThongThanhCong { get; set; }
        [StringLength(500)]
        [Column(TypeName = "nvarchar")]
        string Guid { get; set; }
    }
}
