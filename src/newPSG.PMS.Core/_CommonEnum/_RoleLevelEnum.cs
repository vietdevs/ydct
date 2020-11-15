using System;
using System.Collections.Generic;

namespace newPSG.PMS
{
    public static partial class CommonENum
    {
        #region ROLE_LEVEL - Chức vụ người dùng
        public enum ROLE_LEVEL
        {
            [EnumDisplayString("SA")]
            SA = 0,
            [EnumDisplayString("Doanh nghiệp")]
            DOANH_NGHIEP = 1,
            [EnumDisplayString("Sở y tế/ Viện kiểm nghiệm")]
            SO_YTE_KIEM_NGHIEM = 111,

            [EnumDisplayString("Kế toán")]
            KE_TOAN = 2,
            [EnumDisplayString("Bộ phận tiếp nhận/trả hồ sơ")]
            BO_PHAN_MOT_CUA = 3,
            [EnumDisplayString("Văn thư")]
            VAN_THU = 4,
            [EnumDisplayString("Lãnh đạo Ban")]
            LANH_DAO_CUC = 6,
            [EnumDisplayString("Trưởng phòng")]
            TRUONG_PHONG = 7,
            [EnumDisplayString("Phó phòng")]
            PHO_PHONG = 8,
            [EnumDisplayString("Chuyên viên")]
            CHUYEN_VIEN = 9,
            [EnumDisplayString("Chuyên gia")]
            CHUYEN_GIA = 10,
            [EnumDisplayString("Hội đồng thẩm định")]
            HOI_DONG_THAM_DINH = 201,
        }
        public static List<ItemObj<int>> GetListRole_Level()
        {
            var _list = new List<ItemObj<int>>();
            foreach (object iEnumItem in Enum.GetValues(typeof(ROLE_LEVEL)))
            {
                _list.Add(new ItemObj<int>
                {
                    Id = (int)iEnumItem,
                    Name = GetEnumDescription((ROLE_LEVEL)(int)iEnumItem)
                });
            }
            return _list;
        }
        #endregion

        #region DON_VI_TRUC_THUOC
        public enum DON_VI_TRUC_THUOC
        {
            [EnumDisplayString("Bộ Y Tế")]
            BO_Y_TE = 0,
            [EnumDisplayString("Bộ Nông Nghiệp")]
            BO_NONG_NGHIEP = 1,
            [EnumDisplayString("Bộ Công Thương")]
            BO_CONG_THUONG = 2
        }
        public static List<ItemObj<int>> getListDonViTrucThuoc()
        {
            var _list = new List<ItemObj<int>>();
            foreach (object iEnumItem in Enum.GetValues(typeof(DON_VI_TRUC_THUOC)))
            {
                _list.Add(new ItemObj<int>
                {
                    Id = (int)iEnumItem,
                    Name = GetEnumDescription((DON_VI_TRUC_THUOC)(int)iEnumItem)
                });
            }
            return _list;
        }
        #endregion

        #region TIEU_BAN_CHUYEN_GIA

        public enum TIEU_BAN_CHUYEN_GIA
        {
            [EnumDisplayString("Tiểu ban diệt côn trùng")]
            DIET_CON_TRUNG = 1,
            [EnumDisplayString("Tiểu ban diệt khuẩn")]
            DIET_KHUAN = 2,
        }
        public static List<ItemObj<int>> getListTieuBanChuyenGia()
        {
            var _list = new List<ItemObj<int>>();
            foreach (object iEnumItem in Enum.GetValues(typeof(TIEU_BAN_CHUYEN_GIA)))
            {
                _list.Add(new ItemObj<int>
                {
                    Id = (int)iEnumItem,
                    Name = GetEnumDescription((TIEU_BAN_CHUYEN_GIA)(int)iEnumItem)
                });
            }
            return _list;
        }
        #endregion
    }
}