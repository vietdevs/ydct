using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static newPSG.PMS.AppQuerySql.WhereQueryFormCaseThuTuc;

namespace newPSG.PMS
{
    public  static class UtilitySqlQuery
    {
        public static string WhereQuery(int? formId,int? formCase)
        {
            string filterKeyWord = @"'%'+@keyword+'%'";
            string where = $@" ( 
                                ISNULL(@keyword,'') = '' 
                                or hs.SoDangKy like {filterKeyWord}
                                or hs.MaHoSo like  {filterKeyWord}
                                or dbo.f_LocDauLowerCaseDB(hs.JsonDonHang)  like {filterKeyWord}
                                or dbo.f_LocDauLowerCaseDB(hs.TenCoSoNhapKhau)  like {filterKeyWord}
                                or dbo.f_LocDauLowerCaseDB(hs.TenDoanhNghiep) like  {filterKeyWord}
                                or hs.MaSoThue like {filterKeyWord}
                                )
                                and(@hosoid = 0 or hs.Id = @hosoid)
                                and(@startdate is null or (hsxl.NgayTiepNhan is not null and hsxl.NgayTiepNhan >= @startdate))
                                and(@enddate is null or (hsxl.NgayTiepNhan is not null and hsxl.NgayTiepNhan <= @enddate))
                                and (ISNULL(@tinh,0) = 0 or TinhId = @tinh)
                                and(ISNULL(@doanhnghiepid,0) = 0 or hs.DoanhNghiepId = @doanhnghiepid) ";
            if (formId == (int)CommonENum.FORM_ID.FORM_DANG_KY_HO_SO)
            {
                where += $" and ({FORM_DANG_KY_HO_SO.TAT_CA})";
                switch (formCase)
                {
                    case (int)CommonENum.FORM_CASE_DANG_KY_HO_SO.HO_SO_MOI:
                        where += $" and ({FORM_DANG_KY_HO_SO.HO_SO_MOI})";
                        break;
                    case (int)CommonENum.FORM_CASE_DANG_KY_HO_SO.HO_SO_DANG_XU_LY:
                        where += $" and ({FORM_DANG_KY_HO_SO.HO_SO_DANG_XU_LY})";
                        break;
                    case (int)CommonENum.FORM_CASE_DANG_KY_HO_SO.HO_SO_BI_TRA_LAI:
                        where += $" and ({FORM_DANG_KY_HO_SO.HO_SO_BI_TRA_LAI})";
                        break;
                    case (int)CommonENum.FORM_CASE_DANG_KY_HO_SO.HO_SO_CAN_BO_SUNG:
                        where += $" and ({FORM_DANG_KY_HO_SO.HO_SO_CAN_BO_SUNG})";
                        break;
                    case (int)CommonENum.FORM_CASE_DANG_KY_HO_SO.HO_SO_HOAN_TAT:
                        where += $" and ({FORM_DANG_KY_HO_SO.HO_SO_HOAN_TAT})";
                        break;

                }
            }
            else if (formId == (int)CommonENum.FORM_ID.FORM_MOT_CUA_PHAN_CONG)
            {
                where += $" and ({FORM_MOT_CUA_PHAN_CONG.TAT_CA})";
                switch (formCase)
                {
                    case (int)CommonENum.FORM_CASE_MOT_CUA_PHAN_CONG.HO_SO_CHUA_PHAN_CONG:
                        where += $" and ({FORM_MOT_CUA_PHAN_CONG.HO_SO_CHUA_PHAN_CONG})";
                        break;
                    case (int)CommonENum.FORM_CASE_MOT_CUA_PHAN_CONG.HO_SO_PHAN_CONG_CHUA_XU_LY:
                        where += $" and ({FORM_MOT_CUA_PHAN_CONG.HO_SO_PHAN_CONG_CHUA_XU_LY})";
                        break;
                    case (int)CommonENum.FORM_CASE_MOT_CUA_PHAN_CONG.HO_SO_BI_TU_CHOI:
                        where += $" and ({FORM_MOT_CUA_PHAN_CONG.HO_SO_BI_TU_CHOI})";
                        break;
                    case (int)CommonENum.FORM_CASE_MOT_CUA_PHAN_CONG.HO_SO_PHAN_CONG_DA_XU_LY:
                        where += $" and ({FORM_MOT_CUA_PHAN_CONG.HO_SO_PHAN_CONG_DA_XU_LY})";
                        break;
                }
            }


            return where;
        }
    }
}
