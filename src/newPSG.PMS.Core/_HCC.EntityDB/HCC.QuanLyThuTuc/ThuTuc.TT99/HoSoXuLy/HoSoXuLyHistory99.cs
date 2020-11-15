using System.ComponentModel.DataAnnotations.Schema;

namespace newPSG.PMS.EntityDB
{
    [Table("TT99_HoSoXuLyHistory")]
    public class HoSoXuLyHistory99 : HoSoXuLyHistoryBase
    {
        public long? BienBanThamDinhId_Active { get; set; }
    }
}