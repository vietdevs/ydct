
#region Class Riêng Cho Từng Thủ tục
using XHoSo = newPSG.PMS.EntityDB.HoSo98;
#endregion

namespace newPSG.PMS.Dto
{
    public class TraCuuHoSo98Dto : XHoSo
    {
        public string LoaiQuangCao { get; set; }
        public string StrTinh { get; set; }
    }
}
