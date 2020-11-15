using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Extensions;
using Abp.Linq.Extensions;
using newPSG.PMS.Common.Dto;
using newPSG.PMS.Editions;
using System.Collections.Generic;
using System;
using static newPSG.PMS.CommonENum;
using Abp.Application.Services;

namespace newPSG.PMS.Common
{
    public interface ICommonLookupAppService : IApplicationService
    {
        #region Zero ASP
        Task<ListResultDto<ComboboxItemDto>> GetEditionsForCombobox();

        Task<PagedResultDto<NameValueDto>> FindUsers(FindUsersInput input);

        string GetDefaultEditionName();
        #endregion

        //Customs
        #region QuanLyDoanhNghiep

        List<DropDownListOutput> GetLoaiChuKy(int roleLevel);
        #endregion

        #region thủ tục
        #region Thủ tục 05
        List<DropDownListOutput> GetLoaiTaiLieuDinhKemTT05();
        #endregion
        #endregion

        #region Thanh toan
        List<DropDownListOutput> GetKenhThanhToan();
        List<ItemObj<int>> GetListTrangThaiThanhToanHoSo();
        List<ItemObj<int>> GetFormCaseXacNhanThanhToan();
        #endregion

        #region NguoiDung
        List<ItemObj<int>> GetRoleLevel();
        List<ItemObj<int>> GetListThuTucEnum();

        List<ItemObj<int>> GetListDonViTrucThuoc();

        List<ItemObj<int>> GetListTieuBanChuyenGia();
        #endregion

        #region TrangThai-ThuTuc
        List<DropDownListOutput> GetTrangThaiNganHang();
        List<DropDownListOutput> GetTrangThaiKeToan();
        List<DropDownListOutput> GetTrangThaiQuaHan();
        List<DropDownListOutput> GetTrangThaiHoSo();
        List<DropDownListOutput> GetThuTuc();
        List<DropDownListOutput> GetQuyTrinhThamDinh();
        List<DropDownListOutput> GetLoaiFile();
        #endregion
    }

    [AbpAuthorize]
    public class CommonLookupAppService : PMSAppServiceBase, ICommonLookupAppService
    {
        #region Zero ASP

        private readonly EditionManager _editionManager;

        public CommonLookupAppService(EditionManager editionManager)
        {
            _editionManager = editionManager;
        }

        public async Task<ListResultDto<ComboboxItemDto>> GetEditionsForCombobox()
        {
            var editions = await _editionManager.Editions.ToListAsync();
            return new ListResultDto<ComboboxItemDto>(
                editions.Select(e => new ComboboxItemDto(e.Id.ToString(), e.DisplayName)).ToList()
                );
        }

        public async Task<PagedResultDto<NameValueDto>> FindUsers(FindUsersInput input)
        {
            if (AbpSession.TenantId != null)
            {
                //Prevent tenants to get other tenant's users.
                input.TenantId = AbpSession.TenantId;
            }

            using (CurrentUnitOfWork.SetTenantId(input.TenantId))
            {
                var query = UserManager.Users
                    .WhereIf(
                        !input.Filter.IsNullOrWhiteSpace(),
                        u =>
                            u.Name.Contains(input.Filter) ||
                            u.Surname.Contains(input.Filter) ||
                            u.UserName.Contains(input.Filter) ||
                            u.EmailAddress.Contains(input.Filter)
                    );

                var userCount = await query.CountAsync();
                var users = await query
                    .OrderBy(u => u.Name)
                    .ThenBy(u => u.Surname)
                    .PageBy(input)
                    .ToListAsync();

                return new PagedResultDto<NameValueDto>(
                    userCount,
                    users.Select(u =>
                        new NameValueDto(
                            u.FullName + " (" + u.EmailAddress + ")",
                            u.Id.ToString()
                            )
                        ).ToList()
                    );
            }
        }

        public string GetDefaultEditionName()
        {
            return EditionManager.DefaultEditionName;
        }
        #endregion

        //Customs
        #region QuanLyDoanhNghiep

        public List<DropDownListOutput> GetLoaiChuKy(int roleLevel)
        {
            List<DropDownListOutput> objTemList = new List<DropDownListOutput>();
            try
            {
                foreach (object iEnumItem in Enum.GetValues(typeof(LOAI_CHU_KY)))
                {
                    if ((roleLevel == (int)CommonENum.ROLE_LEVEL.BO_PHAN_MOT_CUA && (int)iEnumItem == (int)LOAI_CHU_KY.DAU_CUA_CUC) 
                        || (int)iEnumItem != (int)LOAI_CHU_KY.DAU_CUA_CUC || roleLevel == -1)
                    {
                        DropDownListOutput objTem = new DropDownListOutput();
                        objTem.Name = GetEnumDescription((LOAI_CHU_KY)(int)iEnumItem);
                        objTem.Id = ((int)iEnumItem);
                        objTemList.Add(objTem);
                    }
                }
            }
            catch { }
            return objTemList;
        }
        #endregion

        #region Thủ Tục
        #region Thủ tục 05
        public List<DropDownListOutput> GetLoaiTaiLieuDinhKemTT05()
        {
            List<DropDownListOutput> objTemList = new List<DropDownListOutput>();
            try
            {
                foreach (object iEnumItem in Enum.GetValues(typeof(CommonENum.LOAI_TAI_LIEU_TT05)))
                {
                    DropDownListOutput objTem = new DropDownListOutput();
                    objTem.Name = GetEnumDescription((CommonENum.LOAI_TAI_LIEU_TT05)(int)iEnumItem);
                    objTem.Id = ((int)iEnumItem);
                    objTemList.Add(objTem);
                }
            }
            catch (Exception)
            {
                objTemList = new List<DropDownListOutput>();
            }
            return objTemList;
        }
        #endregion
        #endregion

        #region Thanh toan
        public List<DropDownListOutput> GetKenhThanhToan()
        {
            List<DropDownListOutput> objTemList = new List<DropDownListOutput>();
            try
            {
                foreach (object iEnumItem in Enum.GetValues(typeof(KENH_THANH_TOAN)))
                {
                    DropDownListOutput objTem = new DropDownListOutput();
                    objTem.Name = GetEnumDescription((KENH_THANH_TOAN)(int)iEnumItem);
                    objTem.Id = ((int)iEnumItem);
                    objTemList.Add(objTem);
                }
            }
            catch { }
            return objTemList;
        }
        public List<ItemObj<int>> GetListTrangThaiThanhToanHoSo()
        {
            return CommonEnumExtensions.getListTrangThaiThanhToanHoSo();
        }
        public List<ItemObj<int>> GetFormCaseXacNhanThanhToan()
        {
            return CommonEnumExtensions.getFormCaseXacNhanThanhToan();
        }
        #endregion

        #region NguoiDung
        public List<ItemObj<int>> GetRoleLevel()
        {
            List<ItemObj<int>> objTemList = new List<ItemObj<int>>();
            try
            {
                foreach (object iEnumItem in Enum.GetValues(typeof(ROLE_LEVEL)))
                {
                    ItemObj<int> objTem = new ItemObj<int>();
                    objTem.Id = (int)iEnumItem;
                    objTem.Name = GetEnumDescription((ROLE_LEVEL)(int)iEnumItem);
                    objTemList.Add(objTem);
                }
            }
            catch { }
            return objTemList;
        }
        public List<ItemObj<int>> GetListThuTucEnum()
        {
            var _list = new List<ItemObj<int>>();
            foreach (object iEnumItem in Enum.GetValues(typeof(THU_TUC_ID)))
            {
                _list.Add(new ItemObj<int>
                {
                    Id = (int)iEnumItem,
                    Name = GetEnumDescription((THU_TUC_ID)(int)iEnumItem)
                });
            }
            return _list;
        }
        public List<ItemObj<int>> GetListDonViTrucThuoc()
        {
            return getListDonViTrucThuoc();
        }

        public List<ItemObj<int>> GetListTieuBanChuyenGia()
        {
            return getListTieuBanChuyenGia();
        }

        #endregion        

        #region TrangThai-ThuTuc
        public List<DropDownListOutput> GetTrangThaiNganHang()
        {
            List<DropDownListOutput> objTemList = new List<DropDownListOutput>();
            try
            {
                foreach (object iEnumItem in Enum.GetValues(typeof(TRANG_THAI_GIAO_DICH)))
                {
                    DropDownListOutput objTem = new DropDownListOutput();
                    objTem.Name = GetEnumDescription((TRANG_THAI_GIAO_DICH)(int)iEnumItem);
                    objTem.Id = ((int)iEnumItem);
                    objTemList.Add(objTem);
                }
            }
            catch { }
            return objTemList;
        }
        public List<DropDownListOutput> GetTrangThaiKeToan()
        {
            List<DropDownListOutput> objTemList = new List<DropDownListOutput>();
            try
            {
                foreach (object iEnumItem in Enum.GetValues(typeof(TRANG_THAI_KE_TOAN)))
                {
                    DropDownListOutput objTem = new DropDownListOutput();
                    objTem.Name = GetEnumDescription((TRANG_THAI_KE_TOAN)(int)iEnumItem);
                    objTem.Id = ((int)iEnumItem);
                    objTemList.Add(objTem);
                }
            }
            catch { }
            return objTemList;
        }
        public List<DropDownListOutput> GetTrangThaiQuaHan()
        {
            List<DropDownListOutput> objTemList = new List<DropDownListOutput>();
            try
            {
                foreach (object iEnumItem in Enum.GetValues(typeof(CommonENum.TRANG_THAI_QUA_HAN)))
                {
                    DropDownListOutput objTem = new DropDownListOutput();
                    objTem.Name = GetEnumDescription((CommonENum.TRANG_THAI_QUA_HAN)(int)iEnumItem);
                    objTem.Id = ((int)iEnumItem);
                    objTemList.Add(objTem);
                }
            }
            catch
            {

            }
            return objTemList;
        }
        public List<DropDownListOutput> GetTrangThaiHoSo()
        {
            List<DropDownListOutput> objTemList = new List<DropDownListOutput>();
            try
            {
                foreach (object iEnumItem in Enum.GetValues(typeof(DON_VI_XU_LY)))
                {
                    DropDownListOutput objTem = new DropDownListOutput();
                    objTem.Name = GetEnumDescription((DON_VI_XU_LY)(int)iEnumItem);
                    objTem.Id = ((int)iEnumItem);
                    objTemList.Add(objTem);
                }
            }
            catch { }
            return objTemList;
        }
        public List<DropDownListOutput> GetThuTuc()
        {
            List<DropDownListOutput> objTemList = new List<DropDownListOutput>();
            try
            {
                foreach (object iEnumItem in Enum.GetValues(typeof(THU_TUC_ID)))
                {
                    DropDownListOutput objTem = new DropDownListOutput();
                    objTem.Name = GetEnumDescription((THU_TUC_ID)(int)iEnumItem);
                    objTem.Id = ((int)iEnumItem);
                    objTemList.Add(objTem);
                }
            }
            catch { }
            return objTemList;
        }
        
        public List<DropDownListOutput> GetQuyTrinhThamDinh()
        {
            List<DropDownListOutput> objTemList = new List<DropDownListOutput>();
            try
            {
                foreach (object iEnumItem in Enum.GetValues(typeof(QUI_TRINH_THAM_DINH)))
                {
                    DropDownListOutput objTem = new DropDownListOutput();
                    objTem.Name = GetEnumDescription((QUI_TRINH_THAM_DINH)(int)iEnumItem);
                    objTem.Id = ((int)iEnumItem);
                    objTemList.Add(objTem);
                }
            }
            catch { }
            return objTemList;
        }
        public List<DropDownListOutput> GetLoaiFile()
        {
            List<DropDownListOutput> objTemList = new List<DropDownListOutput>();
            try
            {
                foreach (object iEnumItem in Enum.GetValues(typeof(LOAI_FILE)))
                {
                    DropDownListOutput objTem = new DropDownListOutput();
                    objTem.Name = GetEnumDescription((LOAI_FILE)(int)iEnumItem);
                    objTem.Id = ((int)iEnumItem);
                    objTemList.Add(objTem);
                }
            }
            catch { }
            return objTemList;
        }
        #endregion
    }
}
