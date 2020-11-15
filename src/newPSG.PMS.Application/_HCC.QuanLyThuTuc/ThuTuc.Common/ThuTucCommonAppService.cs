using Abp.Application.Services;
using Abp.Authorization;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using newPSG.PMS.ThuTucCommon.Dto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using static newPSG.PMS.AppQuerySql.WhereQueryFormCaseThuTuc;

namespace newPSG.PMS.Services
{
    public interface IThuTucCommonAppService : IApplicationService
    {
        Task<List<GetAllCountThuTucDashBoardDto>> GetAllCountThuTucDashBoard(List<GetAllCountThuTucDashBoardDto> input);
        Task<GetCountThuTucDashBoardDto> GetCountThuTucDashBoard(GetCountThuTucDashBoardDto input);
        TotalLabelFormCase GetCountFormCaseThuTucSql(GetCountFormCaseThuTucSqlInput input);
        TotalLabelFormCase GetCountFormCaseThuTucSqlKeToan(GetCountFormCaseThuTucSqlInput input);
        dynamic GetListFormCase(int _formId);
        dynamic GetListFormFunction();
    }
    public class ThuTucCommonAppService : PMSAppServiceBase, IThuTucCommonAppService
    {
        private readonly IAbpSession _session;
        private readonly IIocResolver _iocResolver;

        public ThuTucCommonAppService(IAbpSession session
            , IIocResolver iocResolver)
        {
            _session = session;
            _iocResolver = iocResolver;
        }
        public async Task<GetCountThuTucDashBoardDto> GetCountThuTucDashBoard(GetCountThuTucDashBoardDto input)
        {
            try
            {
                input.Total = 0;
                string maTT = input.ThuTuc < 10 ? $"0{input.ThuTuc}" : input.ThuTuc.ToString();
                Type girdAppServiceType = Type.GetType($"newPSG.PMS.Services.IXuLyHoSoGridView{maTT}AppService");
                Type XHoSoInputDto = Type.GetType($"newPSG.PMS.Dto.HoSoInput{maTT}Dto");
                if (girdAppServiceType == null || XHoSoInputDto == null) return null;
                dynamic _gridSerVice = _iocResolver.Resolve(girdAppServiceType);

                dynamic xinput = Activator.CreateInstance(XHoSoInputDto);
                xinput.FormId = input.FormId;

                xinput.IsOnlyToTal = true;
                foreach (var formCase in input.ListFormCase)
                {
                    xinput.FormCase = formCase;
                    var getList = await _gridSerVice.GetListHoSoPaging(xinput);

                    input.Total += Convert.ToInt32(getList.TotalCount);
                }
                return input;
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                throw ex;
            }
        }

        public async Task<List<GetAllCountThuTucDashBoardDto>> GetAllCountThuTucDashBoard(List<GetAllCountThuTucDashBoardDto> input)
        {
            foreach (var thutuc in input)
            {
                thutuc.Total = 0;

                Type girdAppServiceType = Type.GetType($"newPSG.PMS.Services.IXuLyHoSoGridView{thutuc.ThuTucEnum}AppService");
                Type XHoSoInputDto = Type.GetType($"newPSG.PMS.Dto.HoSoInput{thutuc.ThuTucEnum}Dto");
                if (girdAppServiceType == null) continue;
                dynamic _gridSerVice = _iocResolver.Resolve(girdAppServiceType);
                foreach (var form in thutuc.ListForm)
                {
                    form.Total = 0;
                    foreach (var formCase in form.ListFormCase)
                    {
                        int total = 0;
                        try
                        {
                            dynamic xinput = Activator.CreateInstance(XHoSoInputDto);
                            xinput.FormId = form.FormId;
                            xinput.FormCase = formCase;
                            xinput.IsOnlyToTal = true;

                            var getList = await _gridSerVice.GetListHoSoPaging(xinput);
                            total = Convert.ToInt32(getList.TotalCount);
                        }
                        catch (Exception ex)
                        {
                            Logger.Fatal(ex.Message);
                            throw ex;
                        }

                        form.Total += total;
                    }
                    thutuc.Total += form.Total;
                }
                _iocResolver.Release(_gridSerVice);
            }
            return input;
        }
        public TotalLabelFormCase GetCountFormCaseThuTucSql(GetCountFormCaseThuTucSqlInput input)
        {
            try
            {
                using (var ctx = new AppDbContext())
                {
                    string maTT = input.ThuTuc < 10 ? $"0{input.ThuTuc}" : input.ThuTuc.ToString();
                    string thuTucQuery = $@" select hs.Id
                    {SelectFormCase(input.FormId, input.ThuTuc)}
                    from dbo.TT{maTT}_HoSo hs
                    left join dbo.TT{maTT}_HoSoXuLy hsxl on hs.HoSoXuLyId_Active = hsxl.Id
                    where hs.IsDeleted = 0
                    and hs.PId is null ";
                    string sqlQuery = $@"select
                             ISNULL(Sum(A.FormCase0),0) As Case0
                            ,ISNULL(Sum(A.FormCase1),0) As Case1
                            ,ISNULL(Sum(A.FormCase2),0) As Case2
                            ,ISNULL(Sum(A.FormCase3),0) As Case3
                            ,ISNULL(Sum(A.FormCase4),0) As Case4
                            ,ISNULL(Sum(A.FormCase5),0) As Case5
                            ,ISNULL(Sum(A.FormCase6),0) As Case6
                            ,ISNULL(Sum(A.FormCase7),0) As Case7
                            ,ISNULL(Sum(A.FormCase8),0) As Case8
                            ,ISNULL(Sum(A.FormCase9),0) As Case9
                            from ( {thuTucQuery} ) as A
                            where A.FormCase0 = 1";
                    SqlParameter[] prm =
                            {
                                new SqlParameter("@doanhnghiepid",SqlDbType.BigInt),
                                new SqlParameter("@phongbanid",SqlDbType.BigInt),
                                new SqlParameter("@motcuachuyenid",SqlDbType.BigInt),
                                new SqlParameter("@truongphongid",SqlDbType.BigInt),
                                new SqlParameter("@chuyenvienid",SqlDbType.BigInt),
                                new SqlParameter("@phophongid",SqlDbType.BigInt),
                                new SqlParameter("@lanhdaocuc",SqlDbType.BigInt),
                                new SqlParameter("@lanhdaobo",SqlDbType.BigInt),
                                new SqlParameter("@vanthuid",SqlDbType.BigInt),
                                new SqlParameter("@tenantid",SqlDbType.Int),
                         };
                    prm[0].Value = (object)SessionCustom.UserCurrent.DoanhNghiepId ?? DBNull.Value;
                    prm[1].Value = (object)SessionCustom.UserCurrent.PhongBanId ?? DBNull.Value;
                    prm[2].Value = (object)SessionCustom.UserCurrent.Id ?? DBNull.Value;
                    prm[3].Value = (object)SessionCustom.UserCurrent.Id ?? DBNull.Value;
                    prm[4].Value = (object)SessionCustom.UserCurrent.Id ?? DBNull.Value;
                    prm[5].Value = (object)SessionCustom.UserCurrent.Id ?? DBNull.Value;
                    prm[6].Value = (object)SessionCustom.UserCurrent.Id ?? DBNull.Value;
                    prm[7].Value = (object)SessionCustom.UserCurrent.Id ?? DBNull.Value;
                    prm[8].Value = (object)SessionCustom.UserCurrent.Id ?? DBNull.Value;
                    prm[9].Value = (object)SessionCustom.UserCurrent.TenantId ?? DBNull.Value;
                    var res = ctx.Database.SqlQuery<TotalLabelFormCase>(sqlQuery, prm).FirstOrDefault();
                    return res;
                }
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                throw ex;
            }
        }
        public TotalLabelFormCase GetCountFormCaseThuTucSqlKeToan(GetCountFormCaseThuTucSqlInput input)
        {
            using (var ctx = new AppDbContext())
            {
                string maTT = input.ThuTuc < 10 ? $"0{input.ThuTuc}" : input.ThuTuc.ToString();
                string thuTucQuery = $@" select hs.Id
                    {SelectFormCase(input.FormId)}
                    from dbo.TT{maTT}_HoSo hs
                    left join dbo.TT{maTT}_HoSoXuLy hsxl on hs.HoSoXuLyId_Active = hsxl.Id
                    LEFT JOIN dbo.ThanhToan tt on hs.ThanhToanId_Active = tt.id
                    where hs.IsDeleted = 0
                    and hs.PId is null ";
                string sqlQuery = $@"select
                             ISNULL(Sum(A.FormCase0),0) As Case0
                            ,ISNULL(Sum(A.FormCase1),0) As Case1
                            ,ISNULL(Sum(A.FormCase2),0) As Case2
                            ,ISNULL(Sum(A.FormCase3),0) As Case3
                            ,ISNULL(Sum(A.FormCase4),0) As Case4
                            ,ISNULL(Sum(A.FormCase5),0) As Case5
                            ,ISNULL(Sum(A.FormCase6),0) As Case6
                            ,ISNULL(Sum(A.FormCase7),0) As Case7
                            ,ISNULL(Sum(A.FormCase8),0) As Case8
                            ,ISNULL(Sum(A.FormCase9),0) As Case9
                            from ( {thuTucQuery} ) as A
                            where A.FormCase0 = 1";
                SqlParameter[] prm =
                        {
                                new SqlParameter("@doanhnghiepid",SqlDbType.BigInt),
                                new SqlParameter("@phongbanid",SqlDbType.BigInt),
                                new SqlParameter("@motcuachuyenid",SqlDbType.BigInt),
                                new SqlParameter("@truongphongid",SqlDbType.BigInt),
                                new SqlParameter("@chuyenvienid",SqlDbType.BigInt),
                                new SqlParameter("@phophongid",SqlDbType.BigInt),
                                new SqlParameter("@lanhdaocuc",SqlDbType.BigInt),
                                new SqlParameter("@lanhdaobo",SqlDbType.BigInt),
                                new SqlParameter("@vanthuid",SqlDbType.BigInt),
                                new SqlParameter("@tenantid",SqlDbType.Int),
                         };
                prm[0].Value = (object)SessionCustom.UserCurrent.DoanhNghiepId ?? DBNull.Value;
                prm[1].Value = (object)SessionCustom.UserCurrent.PhongBanId ?? DBNull.Value;
                prm[2].Value = (object)SessionCustom.UserCurrent.Id ?? DBNull.Value;
                prm[3].Value = (object)SessionCustom.UserCurrent.Id ?? DBNull.Value;
                prm[4].Value = (object)SessionCustom.UserCurrent.Id ?? DBNull.Value;
                prm[5].Value = (object)SessionCustom.UserCurrent.Id ?? DBNull.Value;
                prm[6].Value = (object)SessionCustom.UserCurrent.Id ?? DBNull.Value;
                prm[7].Value = (object)SessionCustom.UserCurrent.Id ?? DBNull.Value;
                prm[8].Value = (object)SessionCustom.UserCurrent.Id ?? DBNull.Value;
                prm[9].Value = (object)SessionCustom.UserCurrent.TenantId ?? DBNull.Value;
                return ctx.Database.SqlQuery<TotalLabelFormCase>(sqlQuery, prm).FirstOrDefault();
            }
        }
        public string SelectFormCase(int formId, long thuTuc = 0)
        {
            string select = "";
            if (formId == (int)CommonENum.FORM_ID.FORM_DANG_KY_HO_SO)
            {
                switch (thuTuc)
                {

                    case (int)CommonENum.THU_TUC_ID.THU_TUC_03:
                        select += $@",CASE   WHEN ( {FORM_DANG_KY_HO_SO.TAT_CA} )   THEN 1    ELSE 0  END AS FormCase0
                            , CASE   WHEN ( {FORM_DANG_KY_HO_SO.HO_SO_MOI} )   THEN 1    ELSE 0  END AS FormCase1
                            , CASE   WHEN ( {FORM_DANG_KY_HO_SO.HO_SO_CHO_TIEP_NHAN} )   THEN 1    ELSE 0  END AS FormCase2
                            , CASE   WHEN ( {FORM_DANG_KY_HO_SO.HO_SO_CHO_THANH_TOAN} )   THEN 1    ELSE 0  END AS FormCase3
                            , CASE   WHEN ( {FORM_DANG_KY_HO_SO.HO_SO_CHO_XAC_NHAN_THANH_TOAN} )   THEN 1    ELSE 0  END AS FormCase4
                            , CASE   WHEN ( {FORM_DANG_KY_HO_SO.HO_SO_BI_TRA_LAI} )   THEN 1    ELSE 0  END AS FormCase5
                            , CASE   WHEN ( {FORM_DANG_KY_HO_SO.HO_SO_CAN_BO_SUNG} )   THEN 1    ELSE 0  END AS FormCase6
                            , CASE   WHEN ( {FORM_DANG_KY_HO_SO.HO_SO_HOAN_TAT} )   THEN 1    ELSE 0  END AS FormCase7
                            , CASE   WHEN ( {FORM_DANG_KY_HO_SO.HO_SO_CHO_NOP_GIAY_KIEM_NGHIEM} )   THEN 1    ELSE 0  END AS FormCase8
                            , 0 AS FormCase9";
                        break;
                    default:
                        select += $@",CASE   WHEN ( {FORM_DANG_KY_HO_SO.TAT_CA} )   THEN 1    ELSE 0  END AS FormCase0
                            , CASE   WHEN ( {FORM_DANG_KY_HO_SO.HO_SO_MOI} )   THEN 1    ELSE 0  END AS FormCase1
                            , CASE   WHEN ( {FORM_DANG_KY_HO_SO.HO_SO_CHO_TIEP_NHAN} )   THEN 1    ELSE 0  END AS FormCase2
                            , CASE   WHEN ( {FORM_DANG_KY_HO_SO.HO_SO_CHO_THANH_TOAN} )   THEN 1    ELSE 0  END AS FormCase3
                            , CASE   WHEN ( {FORM_DANG_KY_HO_SO.HO_SO_CHO_XAC_NHAN_THANH_TOAN} )   THEN 1    ELSE 0  END AS FormCase4
                            , CASE   WHEN ( {FORM_DANG_KY_HO_SO.HO_SO_BI_TRA_LAI} )   THEN 1    ELSE 0  END AS FormCase5
                            , CASE   WHEN ( {FORM_DANG_KY_HO_SO.HO_SO_CAN_BO_SUNG} )   THEN 1    ELSE 0  END AS FormCase6
                            , CASE   WHEN ( {FORM_DANG_KY_HO_SO.HO_SO_HOAN_TAT} )   THEN 1    ELSE 0  END AS FormCase7
                            , 0 AS FormCase8
                            , 0 AS FormCase9";
                        break;
                }

            }
            else if (formId == (int)CommonENum.FORM_ID.FORM_MOT_CUA_RA_SOAT)
            {
                switch (thuTuc)
                {

                    case (int)CommonENum.THU_TUC_ID.THU_TUC_03:
                        select += $@",CASE   WHEN ( {FORM_MOT_CUA_TIEP_NHAN_03.TAT_CA} )   THEN 1    ELSE 0  END AS FormCase0
                            , CASE   WHEN ( {FORM_MOT_CUA_TIEP_NHAN_03.HO_SO_NOP_MOI} )   THEN 1    ELSE 0  END AS FormCase1
                            , 0 AS  FormCase2
                            , 0 AS FormCase3
                            , CASE   WHEN ( {FORM_MOT_CUA_TIEP_NHAN_03.HO_SO_NOP_BO_SUNG})   THEN 1    ELSE 0  END AS FormCase4
                           , 0 AS FormCase5
                            , 0 AS FormCase6
                            , 0 AS FormCase7
                            , 0 AS FormCase8
                            , 0 AS FormCase9";
                        break;
                    default:
                        select += $@",CASE   WHEN ( {FORM_MOT_CUA_TIEP_NHAN.TAT_CA} )   THEN 1    ELSE 0  END AS FormCase0
                            , CASE   WHEN ( {FORM_MOT_CUA_TIEP_NHAN.HO_SO_NOP_MOI} )   THEN 1    ELSE 0  END AS FormCase1
                            , 0 AS FormCase2
                            , 0 AS FormCase3
                            , 0 AS FormCase4
                            , 0 AS FormCase5
                            , 0 AS FormCase6
                            , 0 AS FormCase7
                            , 0 AS FormCase8
                            , 0 AS FormCase9";
                        break;
                }

            }
            else if (formId == (int)CommonENum.FORM_ID.FORM_MOT_CUA_PHAN_CONG)
            {
                select += $@",CASE   WHEN ( {FORM_MOT_CUA_PHAN_CONG.TAT_CA} )   THEN 1    ELSE 0  END AS FormCase0
                            , CASE   WHEN ( {FORM_MOT_CUA_PHAN_CONG.HO_SO_CHO_PHAN_CONG})   THEN 1    ELSE 0  END AS FormCase1
                            , 0 AS FormCase2
                            , 0 AS FormCase3
                            , 0 AS FormCase4
                            , 0 AS FormCase5
                            , 0 AS FormCase6
                            , 0 AS FormCase7
                            , 0 AS FormCase8
                            , 0 AS FormCase9";
            }
            else if (formId == (int)CommonENum.FORM_ID.FORM_PHONG_BAN_PHAN_CONG)
            {
                switch (thuTuc)
                {
                    case (int)CommonENum.THU_TUC_ID.THU_TUC_04:
                        select += $@",CASE   WHEN (  (hs.PhongBanId = @phongbanid) and (hs.IsChuyenAuto = 1 or hs.TrangThaiHoSo = {(int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_DA_TIEP_NHAN} or hs.TrangThaiHoSo = {(int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_BO_SUNG} or hs.TrangThaiHoSo = {(int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_HO_SO_MOI}) )   THEN 1    ELSE 0  END AS FormCase0
                            , CASE   WHEN ( {FORM_PHONG_BAN_PHAN_CONG.CHUA_PHAN_CONG} )   THEN 1    ELSE 0  END AS FormCase1
                            , CASE   WHEN ( {FORM_PHONG_BAN_PHAN_CONG.DA_PHAN_CONG} )   THEN 1    ELSE 0  END AS FormCase2
                            , CASE   WHEN ( {FORM_PHONG_BAN_PHAN_CONG.DA_XU_LY} )   THEN 1    ELSE 0  END AS FormCase3
                            , 0 AS FormCase4
                            , 0 AS FormCase5
                            , 0 AS FormCase6
                            , 0 AS FormCase7
                            , 0 AS FormCase8
                            , 0 AS FormCase9";
                        break;
                    case (int)CommonENum.THU_TUC_ID.THU_TUC_03:
                        select += $@",CASE   WHEN ((hs.PhongBanId = @phongbanid) and (hs.IsChuyenAuto = 1 or hs.TrangThaiHoSo = {(int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_DA_TIEP_NHAN} or hs.TrangThaiHoSo = {(int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_BO_SUNG} or hs.TrangThaiHoSo = {(int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_HO_SO_MOI}) )   THEN 1    ELSE 0  END AS FormCase0
                            , CASE   WHEN ( {FORM_PHONG_BAN_PHAN_CONG.CHUA_PHAN_CONG} )   THEN 1    ELSE 0  END AS FormCase1
                            , CASE   WHEN ( {FORM_PHONG_BAN_PHAN_CONG.DA_PHAN_CONG} )   THEN 1    ELSE 0  END AS FormCase2
                            , CASE   WHEN ( {FORM_PHONG_BAN_PHAN_CONG.DA_XU_LY} )   THEN 1    ELSE 0  END AS FormCase3
                            , 0 AS FormCase4
                            , 0 AS FormCase5
                            , 0 AS FormCase6
                            , 0 AS FormCase7
                            , 0 AS FormCase8
                            , 0 AS FormCase9";
                        break;
                    default:
                        select += $@",CASE   WHEN ( {FORM_PHONG_BAN_PHAN_CONG.TAT_CA} )   THEN 1    ELSE 0  END AS FormCase0
                            , CASE   WHEN ( {FORM_PHONG_BAN_PHAN_CONG.CHUA_PHAN_CONG} )   THEN 1    ELSE 0  END AS FormCase1
                            , CASE   WHEN ( {FORM_PHONG_BAN_PHAN_CONG.DA_PHAN_CONG} )   THEN 1    ELSE 0  END AS FormCase2
                            , CASE   WHEN ( {FORM_PHONG_BAN_PHAN_CONG.DA_XU_LY} )   THEN 1    ELSE 0  END AS FormCase3
                            , 0 AS FormCase4
                            , 0 AS FormCase5
                            , 0 AS FormCase6
                            , 0 AS FormCase7
                            , 0 AS FormCase8
                            , 0 AS FormCase9";
                        break;
                }
            }
            else if (formId == (int)CommonENum.FORM_ID.FORM_THAM_XET_HO_SO)
            {
                switch (thuTuc)
                {
                    //, CASE WHEN( { FORM_THAM_XET_HO_SO.HO_SO_DANG_THEO_DOI} )   THEN 1    ELSE 0  END AS FormCase6
                    case (int)CommonENum.THU_TUC_ID.THU_TUC_03:
                        select += $@",CASE   WHEN ( {FORM_THAM_XET_HO_SO.TAT_CA} )   THEN 1    ELSE 0  END AS FormCase0
                            , CASE   WHEN ( {FORM_THAM_XET_HO_SO.HO_SO_THAM_XET_MOI} )   THEN 1    ELSE 0  END AS FormCase1
                            , CASE   WHEN ( {FORM_THAM_XET_HO_SO.HO_SO_DA_THAM_XET} )   THEN 1    ELSE 0  END AS FormCase2
                            , CASE   WHEN ( {FORM_THAM_XET_HO_SO.HO_SO_THAM_XET_BO_SUNG} )   THEN 1    ELSE 0  END AS FormCase3
                            , CASE   WHEN ( {FORM_THAM_XET_HO_SO.HO_SO_THAM_XET_LAI} )   THEN 1    ELSE 0  END AS FormCase4
                            , CASE   WHEN ( {FORM_THAM_XET_HO_SO.HO_SO_DANG_THEO_DOI} )   THEN 1    ELSE 0  END AS FormCase5
                            , 0 AS FormCase6
                            , 0 AS FormCase7
                            , 0 AS FormCase8
                            , 0 AS FormCase9";
                        break;
                    default:
                        select += $@",CASE   WHEN ( {FORM_THAM_XET_HO_SO.TAT_CA} )   THEN 1    ELSE 0  END AS FormCase0
                            , CASE   WHEN ( {FORM_THAM_XET_HO_SO.HO_SO_THAM_XET_MOI} )   THEN 1    ELSE 0  END AS FormCase1
                            , CASE   WHEN ( {FORM_THAM_XET_HO_SO.HO_SO_DA_THAM_XET} )   THEN 1    ELSE 0  END AS FormCase2
                            , CASE   WHEN ( {FORM_THAM_XET_HO_SO.HO_SO_THAM_XET_BO_SUNG} )   THEN 1    ELSE 0  END AS FormCase3
                            , CASE   WHEN ( {FORM_THAM_XET_HO_SO.HO_SO_THAM_XET_LAI} )   THEN 1    ELSE 0  END AS FormCase4
                            , CASE   WHEN ( {FORM_THAM_XET_HO_SO.HO_SO_DANG_THEO_DOI} )   THEN 1    ELSE 0  END AS FormCase5
                            , 0 AS FormCase6
                            , 0 AS FormCase7
                            , 0 AS FormCase8
                            , 0 AS FormCase9";
                        break;
                }

            }
            else if (formId == (int)CommonENum.FORM_ID.FORM_PHO_PHONG_DUYET)
            {
                select += $@",CASE   WHEN ( {FORM_PHO_PHONG_DUYET.TAT_CA} )   THEN 1    ELSE 0  END AS FormCase0
                            , CASE   WHEN ( {FORM_PHO_PHONG_DUYET.HO_SO_CHUA_DUYET} )   THEN 1    ELSE 0  END AS FormCase1
                            , CASE   WHEN ( {FORM_PHO_PHONG_DUYET.HO_SO_DA_DUỴET} )   THEN 1    ELSE 0  END AS FormCase2
                            , 0 AS FormCase3
                            , 0 AS FormCase4
                            , 0 AS FormCase5
                            , 0 AS FormCase6
                            , 0 AS FormCase7
                            , 0 AS FormCase8
                            , 0 AS FormCase9";
            }
            else if (formId == (int)CommonENum.FORM_ID.FORM_TRUONG_PHONG_DUYET)
            {
                switch (thuTuc)
                {
                    default:
                        select += $@",CASE   WHEN ( {FORM_TRUONG_PHONG_DUYET.TAT_CA} )   THEN 1    ELSE 0  END AS FormCase0
                            , CASE   WHEN ( {FORM_TRUONG_PHONG_DUYET.HO_SO_CHUA_DUYET} )   THEN 1    ELSE 0  END AS FormCase1
                            , CASE   WHEN ( {FORM_TRUONG_PHONG_DUYET.HO_SO_DA_DUỴET} )   THEN 1    ELSE 0  END AS FormCase2
                            , 0 AS FormCase3
                            , 0 AS FormCase4
                            , 0 AS FormCase5
                            , 0 AS FormCase6
                            , 0 AS FormCase7
                            , 0 AS FormCase8
                            , 0 AS FormCase9";
                        break;
                }
            }
            else if (formId == (int)CommonENum.FORM_ID.FORM_LANH_DAO_CUC_DUYET)
            {
                select += $@",CASE   WHEN ( {FORM_LANH_DAO_CUC_DUYET.TAT_CA} )   THEN 1    ELSE 0  END AS FormCase0
                            , CASE   WHEN ( {FORM_LANH_DAO_CUC_DUYET.HO_SO_DAT_CHUA_DUYET} )   THEN 1    ELSE 0  END AS FormCase1
                            , CASE   WHEN ( {FORM_LANH_DAO_CUC_DUYET.HO_SO_DA_DUỴET} )   THEN 1    ELSE 0  END AS FormCase2
                            , 0 AS FormCase3
                            , 0 AS FormCase4
                            , 0 AS FormCase5
                            , 0 AS FormCase6
                            , 0 AS FormCase7
                            , 0 AS FormCase8
                            , 0 AS FormCase9";
            }
            else if (formId == (int)CommonENum.FORM_ID.FORM_LANH_DAO_CUC_DUYET_CONG_VAN)
            {
                select += $@",CASE   WHEN ( {FORM_LANH_DAO_CUC_DUYET_CONG_VAN.TAT_CA} )   THEN 1    ELSE 0  END AS FormCase0
                            , CASE   WHEN ( {FORM_LANH_DAO_CUC_DUYET_CONG_VAN.CONG_VAN_CHUA_DUYET} )   THEN 1    ELSE 0  END AS FormCase1
                            , CASE   WHEN ( {FORM_LANH_DAO_CUC_DUYET_CONG_VAN.CONG_VAN_DA_DUỴET} )   THEN 1    ELSE 0  END AS FormCase2
                            , 0 AS FormCase3
                            , 0 AS FormCase4
                            , 0 AS FormCase5
                            , 0 AS FormCase6
                            , 0 AS FormCase7
                            , 0 AS FormCase8
                            , 0 AS FormCase9";
            }
            else if (formId == (int)CommonENum.FORM_ID.FORM_VAN_THU_DUYET)
            {
                select += $@",CASE   WHEN ( {FORM_VAN_THU_DUYET.TAT_CA} )   THEN 1    ELSE 0  END AS FormCase0
                            , CASE   WHEN ( {FORM_VAN_THU_DUYET.HO_SO_CHUA_DUYET} )   THEN 1    ELSE 0  END AS FormCase1
                            , CASE   WHEN ( {FORM_VAN_THU_DUYET.HO_SO_DA_DUỴET} )   THEN 1    ELSE 0  END AS FormCase2
                            , 0 AS FormCase3
                            , 0 AS FormCase4
                            , 0 AS FormCase5
                            , 0 AS FormCase6
                            , 0 AS FormCase7
                            , 0 AS FormCase8
                            , 0 AS FormCase9";
            }
            else if (formId == (int)CommonENum.FORM_ID.FORM_VAN_THU_DUYET_CONG_VAN)
            {
                select += $@",CASE   WHEN ( {FORM_VAN_THU_DUYET_CONG_VAN.TAT_CA} )   THEN 1    ELSE 0  END AS FormCase0
                            , CASE   WHEN ( {FORM_VAN_THU_DUYET_CONG_VAN.CONG_VAN_CHUA_DUYET} )   THEN 1    ELSE 0  END AS FormCase1
                            , CASE   WHEN ( {FORM_VAN_THU_DUYET_CONG_VAN.CONG_VAN_DA_DUỴET} )   THEN 1    ELSE 0  END AS FormCase2
                            , CASE   WHEN ( {FORM_VAN_THU_DUYET_CONG_VAN.CONG_VAN_TRA_LAI} )   THEN 1    ELSE 0  END AS FormCase3
                            , 0 AS FormCase4
                            , 0 AS FormCase5
                            , 0 AS FormCase6
                            , 0 AS FormCase7
                            , 0 AS FormCase8
                            , 0 AS FormCase9";
            }
            else if (formId == (int)CommonENum.FORM_ID.FORM_KE_TOAN_XAC_NHAN_THANH_TOAN)
            {
                select += $@",CASE   WHEN ( {FORM_KE_TOAN_XAC_NHAN_THANH_TOAN.TAT_CA} )   THEN 1    ELSE 0  END AS FormCase0
                            , CASE   WHEN ( {FORM_KE_TOAN_XAC_NHAN_THANH_TOAN.HO_SO_CHO_XAC_NHAN} )   THEN 1    ELSE 0  END AS FormCase1
                            , CASE   WHEN ( {FORM_KE_TOAN_XAC_NHAN_THANH_TOAN.HO_SO_THANH_TOAN_THANH_CONG} )   THEN 1    ELSE 0  END AS FormCase2
                            , CASE   WHEN ( {FORM_KE_TOAN_XAC_NHAN_THANH_TOAN.HO_SO_THANH_TOAN_THAT_BAI} )   THEN 1    ELSE 0  END AS FormCase3
                            , 0 AS FormCase4
                            , 0 AS FormCase5
                            , 0 AS FormCase6
                            , 0 AS FormCase7
                            , 0 AS FormCase8
                            , 0 AS FormCase9";
            }
            else if (formId == (int)CommonENum.FORM_ID.FORM_TONG_HOP_THAM_XET_HO_SO)
            {
                select += $@",CASE   WHEN ( {FORM_TONG_HOP_THAM_XET_HO_SO.TAT_CA} )   THEN 1    ELSE 0  END AS FormCase0
                            , CASE   WHEN ( {FORM_TONG_HOP_THAM_XET_HO_SO.HO_SO_XU_LY} )   THEN 1    ELSE 0  END AS FormCase1
                            , 0 AS FormCase2
                            , 0 AS FormCase3
                            , 0 AS FormCase4
                            , 0 AS FormCase5
                            , 0 AS FormCase6
                            , 0 AS FormCase7
                            , 0 AS FormCase8
                            , 0 AS FormCase9";
            }
            //else if (formId == 0)
            //{
            //    select += $@",CASE   WHEN ( {FORM_MOT_CUA_TIEP_NHAN_03.TAT_CA} )   THEN 1    ELSE 0  END AS FormCase0
            //                , CASE   WHEN ( {FORM_MOT_CUA_TIEP_NHAN_03.HO_SO_NOP_MOI} )   THEN 1    ELSE 0  END AS FormCase1
            //                , 0 AS  FormCase2
            //                , 0 AS FormCase3
            //                , CASE   WHEN ( {FORM_MOT_CUA_TIEP_NHAN_03.HO_SO_CHO_TRA_GIAY_TIEP_NHAN})   THEN 1    ELSE 0  END AS FormCase4
            //               , 0 AS FormCase5
            //                , 0 AS FormCase6
            //                , 0 AS FormCase7
            //                , 0 AS FormCase8
            //                , 0 AS FormCase9";
            //}
            return select;
        }

        [AbpAllowAnonymous]
        public dynamic GetListFormCase(int _formId)
        {
            try
            {
                var _form = (CommonENum.FORM_ID)_formId;
                return new
                {
                    FormCase = CommonEnumExtensions.GetListFormCase(_form),
                    FormCase2 = CommonEnumExtensions.GetListFormCase2(_form)
                };
            }
            catch
            {
                return null;
            }
        }

        [AbpAllowAnonymous]
        public dynamic GetListFormFunction()
        {
            try
            {
                return CommonEnumExtensions.getListFormFunction();
            }
            catch
            {
                return null;
            }
        }
    }
}
