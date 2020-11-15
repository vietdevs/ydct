using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static newPSG.PMS.CommonENum;

namespace newPSG.PMS
{
    public static class CommonEnumExtensions
    {
        public static List<ItemObj<int>> GetListFormCase(FORM_ID _form)
        {
            var _list = new List<ItemObj<int>>();
            Type _typeOf = null;
            switch (_form)
            {
                case FORM_ID.FORM_DANG_KY_HO_SO:
                    _typeOf = typeof(FORM_CASE_DANG_KY_HO_SO);
                    break;
                case FORM_ID.FORM_KE_TOAN_XAC_NHAN_THANH_TOAN:
                    _typeOf = typeof(FORM_CASE_XAC_NHAN_THANH_TOAN);
                    break;
                case FORM_ID.FORM_MOT_CUA_RA_SOAT:
                    _typeOf = typeof(FORM_CASE_MOT_CUA_RA_SOAT);
                    break;
                case FORM_ID.FORM_MOT_CUA_PHAN_CONG:
                    _typeOf = typeof(FORM_CASE_MOT_CUA_PHAN_CONG);
                    break;
                case FORM_ID.FORM_PHONG_BAN_PHAN_CONG:
                    _typeOf = typeof(FORM_CASE_PHONG_BAN_PHAN_CONG);
                    break;
                case FORM_ID.FORM_THAM_XET_HO_SO:
                    _typeOf = typeof(FORM_CASE_THAM_XET_HO_SO);
                    break;
                case FORM_ID.FORM_CHUYEN_GIA_THAM_DINH:
                    _typeOf = typeof(FORM_CASE_CHUYEN_GIA_THAM_DINH);
                    break;
                case FORM_ID.FORM_HOI_DONG_THAM_DINH:
                    _typeOf = typeof(FORM_CASE_HOI_DONG_THAM_DINH);
                    break;
                case FORM_ID.FORM_PHO_PHONG_DUYET:
                    _typeOf = typeof(FORM_CASE_PHO_PHONG_DUYET);
                    break;
                case FORM_ID.FORM_TRUONG_PHONG_DUYET:
                    _typeOf = typeof(FORM_CASE_TRUONG_PHONG_DUYET);
                    break;
                case FORM_ID.FORM_LANH_DAO_CUC_DUYET:
                    _typeOf = typeof(FORM_CASE_LANH_DAO_CUC_DUYET);
                    break;
                case FORM_ID.FORM_LANH_DAO_CUC_DUYET_CONG_VAN:
                    _typeOf = typeof(FORM_CASE_LANH_DAO_CUC_DUYET);
                    break;
                case FORM_ID.FORM_VAN_THU_DUYET:
                    _typeOf = typeof(FORM_CASE_VAN_THU_DUYET);
                    break;
                case FORM_ID.FORM_VAN_THU_DUYET_CONG_VAN:
                    _typeOf = typeof(FORM_CASE_VAN_THU_DUYET_CONG_VAN);
                    break;
                case FORM_ID.FORM_TRA_CUU_HO_SO:
                    _typeOf = typeof(FORM_CASE_TRA_CUU_HO_SO);
                    break;
                case FORM_ID.FORM_TONG_HOP_THAM_XET_HO_SO:
                    _typeOf = typeof(FORM_CASE_CHUYEN_VIEN_TONG_HOP);
                    break;
                case FORM_ID.FORM_THAM_DINH_HO_SO_TT37:
                    _typeOf = typeof(FORM_CASE_THAM_DINH_HO_SO_TT37);
                    break;
                case FORM_ID.FORM_TONG_HOP_THAM_DINH_TT37:
                    _typeOf = typeof(FORM_CASE_TONG_HOP_THAM_DINH_TT37);
                    break;
                case FORM_ID.FORM_TRUONG_PHONG_DUYET_THAM_DINH_TT37:
                    _typeOf = typeof(FORM_CASE_TRUONG_PHONG_DUYET_THAM_DINH_TT37);
                    break;
                case FORM_ID.FORM_LANH_DAO_CUC_DUYET_THAM_DINH_TT37:
                    _typeOf = typeof(FORM_CASE_LANH_DAO_DUYET_THAM_DINH_TT37);
                    break;
                case FORM_ID.FORM_VAN_THU_DUYET_THAM_DINH_TT37:
                    _typeOf = typeof(FORM_CASE_VAN_THU_DUYET_THAM_DINH_TT37);
                    break;
            }

            foreach (object iEnumItem in Enum.GetValues(_typeOf))
            {
                int iEnum = Convert.ToInt32(iEnumItem);

                var instanceOfAttribute = GetThucEnumAttriblue((Enum)Enum.ToObject(_typeOf, iEnum));

                _list.Add(new ItemObj<int>
                {
                    Id = (int)iEnumItem,
                    Name = instanceOfAttribute.DisplayName,
                    IsForXuLy = instanceOfAttribute.IsForXuLy,
                });
            }
            return _list;
        }
        public static List<ItemObj<int>> GetListFormCaseTT03(FORM_ID _form,FORM_CASE_DEFAULT _formDefault)
        {
            var _list = new List<ItemObj<int>>();
            Type _typeOf = null;
            switch (_form)
            {
                case FORM_ID.FORM_DANG_KY_HO_SO:
                    if (_formDefault == FORM_CASE_DEFAULT.THAM_DINH_KHAO_NGHIEM)
                    {
                        _typeOf = typeof(FORM_CASE_KHAO_NGHIEM); 
                    }
                    if (_formDefault == FORM_CASE_DEFAULT.THAM_DINH_CAP_SO_DANG_KY)
                    {
                        _typeOf = typeof(FORM_CASE_CAP_SO_DANG_KY);
                    }
                    //_typeOf = typeof(FORM_CASE_DANG_KY_HO_SO);
                    break;
                case FORM_ID.FORM_KE_TOAN_XAC_NHAN_THANH_TOAN:
                    _typeOf = typeof(FORM_CASE_XAC_NHAN_THANH_TOAN);
                    break;
                case FORM_ID.FORM_MOT_CUA_RA_SOAT:
                    _typeOf = typeof(FORM_CASE_MOT_CUA_RA_SOAT_TT03);
                    break;
                case FORM_ID.FORM_MOT_CUA_PHAN_CONG:
                    _typeOf = typeof(FORM_CASE_MOT_CUA_PHAN_CONG);
                    break;
                case FORM_ID.FORM_PHONG_BAN_PHAN_CONG:
                    _typeOf = typeof(FORM_CASE_PHONG_BAN_PHAN_CONG);
                    break;
                case FORM_ID.FORM_THAM_XET_HO_SO:
                    if (_formDefault == FORM_CASE_DEFAULT.THAM_DINH_KHAO_NGHIEM)
                    {
                        _typeOf = typeof(FORM_CASE_THAM_XET_HO_SO);
                    }
                    if (_formDefault == FORM_CASE_DEFAULT.THAM_DINH_CAP_SO_DANG_KY)
                    {
                        _typeOf = typeof(FORM_CASE_THAM_XET_HO_SO_CAP_SO_DANG_KY);
                    }
                    break;
                case FORM_ID.FORM_CHUYEN_GIA_THAM_DINH:
                    _typeOf = typeof(FORM_CASE_CHUYEN_GIA_THAM_DINH);
                    break;
                case FORM_ID.FORM_HOI_DONG_THAM_DINH:
                    _typeOf = typeof(FORM_CASE_HOI_DONG_THAM_DINH);
                    break;
                case FORM_ID.FORM_PHO_PHONG_DUYET:
                    _typeOf = typeof(FORM_CASE_PHO_PHONG_DUYET);
                    break;
                case FORM_ID.FORM_TRUONG_PHONG_DUYET:
                    _typeOf = typeof(FORM_CASE_TRUONG_PHONG_DUYET);
                    break;
                case FORM_ID.FORM_LANH_DAO_CUC_DUYET:
                    _typeOf = typeof(FORM_CASE_LANH_DAO_CUC_DUYET);
                    break;
                case FORM_ID.FORM_LANH_DAO_CUC_DUYET_CONG_VAN:
                    _typeOf = typeof(FORM_CASE_LANH_DAO_CUC_DUYET);
                    break;
                case FORM_ID.FORM_VAN_THU_DUYET:
                    _typeOf = typeof(FORM_CASE_VAN_THU_DUYET);
                    break;
                case FORM_ID.FORM_VAN_THU_DUYET_CONG_VAN:
                    _typeOf = typeof(FORM_CASE_VAN_THU_DUYET_CONG_VAN);
                    break;
                case FORM_ID.FORM_TRA_CUU_HO_SO:
                    _typeOf = typeof(FORM_CASE_TRA_CUU_HO_SO);
                    break;
                case FORM_ID.FORM_TONG_HOP_THAM_XET_HO_SO:
                    _typeOf = typeof(FORM_CASE_CHUYEN_VIEN_TONG_HOP);
                    break;
            }

            foreach (object iEnumItem in Enum.GetValues(_typeOf))
            {
                int iEnum = Convert.ToInt32(iEnumItem);
                _list.Add(new ItemObj<int>
                {
                    Id = (int)iEnumItem,
                    Name = GetEnumDescription((Enum)Enum.ToObject(_typeOf, iEnum))
                });
            }
            return _list;
        }
        public static List<ItemObj<int>> GetListFormCase2(FORM_ID _form)
        {
            var _list = new List<ItemObj<int>>();
            if (_form == FORM_ID.FORM_THAM_XET_HO_SO)
            {
                foreach (object iEnumItem in Enum.GetValues(typeof(FORM_CASE2_THAM_XET_HO_SO)))
                {
                    _list.Add(new ItemObj<int>
                    {
                        Id = (int)iEnumItem,
                        Name = GetEnumDescription((FORM_CASE2_THAM_XET_HO_SO)(int)iEnumItem)
                    });
                }
            }
            return _list;
        }
        public static List<ItemObj<int>> GetListFormCaseDeFault(FORM_ID _form)
        {
            var _list = new List<ItemObj<int>>();
            if (_form == FORM_ID.FORM_PHONG_BAN_PHAN_CONG|| _form == FORM_ID.FORM_MOT_CUA_RA_SOAT|| _form == FORM_ID.FORM_TRUONG_PHONG_DUYET|| _form == FORM_ID.FORM_THAM_XET_HO_SO)
            {
                foreach (object iEnumItem in Enum.GetValues(typeof(FORM_CASE_DEFAULT)))
                {
                    _list.Add(new ItemObj<int>
                    {
                        Id = (int)iEnumItem,
                        Name = GetEnumDescription((FORM_CASE_DEFAULT)(int)iEnumItem)
                    });
                }
            }
            return _list;
        }

        public static List<ItemObj<int>> getListTrangThaiThanhToanHoSo()
        {
            var _list = new List<ItemObj<int>>();
            foreach (object iEnumItem in Enum.GetValues(typeof(TRANG_THAI_THANH_TOAN_HO_SO)))
            {
                _list.Add(new ItemObj<int>
                {
                    Id = (int)iEnumItem,
                    Name = GetEnumDescription((TRANG_THAI_THANH_TOAN_HO_SO)(int)iEnumItem)
                });
            }
            return _list;
        }
        public static List<ItemObj<int>> getFormCaseXacNhanThanhToan()
        {
            var _list = new List<ItemObj<int>>();
            foreach (object iEnumItem in Enum.GetValues(typeof(FORM_CASE_XAC_NHAN_THANH_TOAN)))
            {
                _list.Add(new ItemObj<int>
                {
                    Id = (int)iEnumItem,
                    Name = GetEnumDescription((FORM_CASE_XAC_NHAN_THANH_TOAN)(int)iEnumItem)
                });
            }
            return _list;
        }
        
        public static List<ItemObj<int>> getListFormFunction()
        {
            var _list = new List<ItemObj<int>>();
            foreach (object iEnumItem in Enum.GetValues(typeof(FORM_FUNCTION)))
            {
                _list.Add(new ItemObj<int>
                {
                    Id = (int)iEnumItem,
                    Name = GetEnumDescription((FORM_FUNCTION)(int)iEnumItem)
                });
            }
            return _list;
        }
    }    
}
