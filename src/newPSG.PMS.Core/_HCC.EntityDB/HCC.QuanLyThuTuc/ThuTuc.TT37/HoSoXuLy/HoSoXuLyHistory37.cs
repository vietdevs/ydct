using System.ComponentModel.DataAnnotations.Schema;

namespace newPSG.PMS.EntityDB
{
    [Table("TT37_HoSoXuLyHistory")]
    public class HoSoXuLyHistory37 : HoSoXuLyHistoryBase
    {
        public long? BienBanThamDinhId_Active { get; set; }
        public int? TrangThaiXuLy { get; set; }
    }
}