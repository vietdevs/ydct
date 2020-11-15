using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;

namespace newPSG.PMS.EntityDB
{
    [Table("HoSoXuLy_PhanCongSoLuong")]
    public class HoSoXuLy_PhanCongSoLuong : AuditedEntity<long>
    {
        public int ThuTucId { get; set; }
        public long HoSoId { get; set; }
        public long HoSoXuLyId { get; set; }
        public int TieuBanEnum { get; set; }
        public int? SoLuong { get; set; }

        #region Trường hợp có đơn vị ngoài cục
        public long? HoSoXuLy_DonViId { get; set; }
        #endregion
    }
}