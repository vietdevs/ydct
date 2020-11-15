using Abp.Application.Services;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using newPSG.PMS.Authorization.Roles;
using newPSG.PMS.Authorization.Users;
using newPSG.PMS.Editions;
using newPSG.PMS.EntityDB;
using System;
using System.Linq;

namespace newPSG.PMS.Services
{
    public interface ICapSoThuTucAppService : IApplicationService
    {
        string SinhSoChungNhan(long hosoId, int thuTucId = 0, bool IsTemp = false);
        string SinhSoDangKy(long doanhNghiepId, int thuTucId = 0);
        int SinhSoCongVan(long hoSoXuLyId, int phanHeId = 0, bool IsTemp = false);
        int SinhSoQuyetDinh(long hoSoXuLyId, int phanHeId = 0, bool IsTemp = false);
    }
    public class CapSoThuTucAppService : ICapSoThuTucAppService
    {
        private readonly EditionManager _editionManager;
        private readonly IRepository<Tinh> _tinhRepos;
        private readonly IRepository<DoanhNghiep, long> _doanhNghiepRepos;
        private readonly IRepository<User, long> _userRepos;
        private readonly IRepository<Role> _roleRepos;
        private readonly IRepository<UserRole, long> _userRoleRepos;
        private readonly IRepository<CapSoThuTuc> _capSoThuTucRepos;
        private readonly IRepository<ThuTuc> _thuTucRepos;
        private readonly IRepository<CapSoCongVan> _capSoCongVanRepos;
        private readonly IRepository<CapSoQuyetDinh> _capSoQuyetDinhRepos;
        private readonly IAbpSession _session;

        public CapSoThuTucAppService(
            EditionManager editionManager,
            IRepository<Tinh> tinhRepos,
            IRepository<DoanhNghiep, long> doanhNghiepRepos,
            IRepository<User, long> userRepos,
            IRepository<Role> roleRepos,
            IRepository<UserRole, long> userRoleRepos,
            IRepository<CapSoThuTuc> capSoThuTucRepos,
            IRepository<ThuTuc> thuTucRepos,
              IRepository<CapSoCongVan> capSoCongVanRepos,
            IRepository<CapSoQuyetDinh> capSoQuyetDinhRepos,
            IAbpSession session)
        {
            _editionManager = editionManager;
            _tinhRepos = tinhRepos;
            _doanhNghiepRepos = doanhNghiepRepos;
            _userRepos = userRepos;
            _roleRepos = roleRepos;
            _userRoleRepos = userRoleRepos;
            _capSoThuTucRepos = capSoThuTucRepos;
            _thuTucRepos = thuTucRepos;
            _capSoCongVanRepos = capSoCongVanRepos;
            _capSoQuyetDinhRepos = capSoQuyetDinhRepos;
            _session = session;
        }

        public string SinhSoChungNhan(long hoSoId, int thuTucId = 0, bool IsTemp = false)
        {
            try
            {
                CommonENum.THU_TUC_ID thuTuc = CommonENum.THU_TUC_ID.THU_TUC_99;
                if (thuTucId > 0)
                {
                    thuTuc = (CommonENum.THU_TUC_ID)thuTucId;
                }

                int nhomThuTucId = 0;
                //string maNhom = "DONGNAI";
                if (thuTucId > 0)
                {
                    var objThuTuc = _thuTucRepos.FirstOrDefault(x => x.ThuTucIdEnum == thuTucId);
                }

                int namCA = DateTime.Now.Year;

                var objSoTiepNhan = _capSoThuTucRepos.GetAll().Where(x => x.NhomThuTucId == nhomThuTucId && x.HoSoId == hoSoId && x.Nam == namCA && x.TenantId == _session.TenantId).FirstOrDefault();


                string strSoCongBo = "";
                if (objSoTiepNhan != null)
                {
                    //strSoCongBo = string.Format("{0}e/{1}/QLD{2}", objSoTiepNhan.So, namCA, maNhom);
                    strSoCongBo = string.Format("{0}e/ĐKSP", objSoTiepNhan.So);
                }
                else
                {
                    var soTiepNhanMaxOfNam = _capSoThuTucRepos.GetAll().Where(x => x.NhomThuTucId == nhomThuTucId && x.Nam == namCA && x.TenantId == _session.TenantId).OrderByDescending(o => o.So).FirstOrDefault();

                    int soTiepNhanMoi = 1;
                    if (soTiepNhanMaxOfNam != null)
                    {
                        soTiepNhanMoi = soTiepNhanMaxOfNam.So + 1;
                    }
                    if (IsTemp != true)
                    {
                        _capSoThuTucRepos.Insert(new CapSoThuTuc
                        {
                            NhomThuTucId = nhomThuTucId,
                            ThuTucId = thuTucId,
                            HoSoId = hoSoId,
                            Nam = namCA,
                            So = soTiepNhanMoi,
                            TenantId = _session.TenantId.Value
                        });
                    }
                    strSoCongBo = string.Format("{0}e/ĐKSP", soTiepNhanMoi);
                }

                return strSoCongBo;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private int SoDangKyMax(DoanhNghiep doanhNghiep, CommonENum.THU_TUC_ID thuTuc = CommonENum.THU_TUC_ID.THU_TUC_99)
        {
            if (doanhNghiep != null)
            {
                var maThuTuc = (int)thuTuc;
                switch (maThuTuc)
                {
                    case 0: return doanhNghiep.SoThuTuc0CA.HasValue ? doanhNghiep.SoThuTuc0CA.Value : 0;
                    case 1: return doanhNghiep.SoThuTuc1CA.HasValue ? doanhNghiep.SoThuTuc1CA.Value : 0;
                    case 2: return doanhNghiep.SoThuTuc2CA.HasValue ? doanhNghiep.SoThuTuc2CA.Value : 0;
                    case 3: return doanhNghiep.SoThuTuc3CA.HasValue ? doanhNghiep.SoThuTuc3CA.Value : 0;
                    case 4: return doanhNghiep.SoThuTuc4CA.HasValue ? doanhNghiep.SoThuTuc4CA.Value : 0;
                    case 5: return doanhNghiep.SoThuTuc5CA.HasValue ? doanhNghiep.SoThuTuc5CA.Value : 0;
                    case 6: return doanhNghiep.SoThuTuc6CA.HasValue ? doanhNghiep.SoThuTuc6CA.Value : 0;
                    case 7: return doanhNghiep.SoThuTuc7CA.HasValue ? doanhNghiep.SoThuTuc7CA.Value : 0;
                    case 8: return doanhNghiep.SoThuTuc8CA.HasValue ? doanhNghiep.SoThuTuc8CA.Value : 0;
                    case 9: return doanhNghiep.SoThuTuc9CA.HasValue ? doanhNghiep.SoThuTuc9CA.Value : 0;
                    case 10: return doanhNghiep.SoThuTuc10CA.HasValue ? doanhNghiep.SoThuTuc10CA.Value : 0;
                }
            }
            return 0;
        }

        private void UpdateSoDangKyMax(ref DoanhNghiep doanhNghiep, CommonENum.THU_TUC_ID thuTuc = CommonENum.THU_TUC_ID.THU_TUC_99, int soDangKyMax = 1)
        {
            if (doanhNghiep != null)
            {
                var maThuTuc = (int)thuTuc;
                switch (maThuTuc)
                {
                    case 0: doanhNghiep.SoThuTuc0CA = soDangKyMax; break;
                    case 1: doanhNghiep.SoThuTuc1CA = soDangKyMax; break;
                    case 2: doanhNghiep.SoThuTuc2CA = soDangKyMax; break;
                    case 3: doanhNghiep.SoThuTuc3CA = soDangKyMax; break;
                    case 4: doanhNghiep.SoThuTuc4CA = soDangKyMax; break;
                    case 5: doanhNghiep.SoThuTuc5CA = soDangKyMax; break;
                    case 6: doanhNghiep.SoThuTuc6CA = soDangKyMax; break;
                    case 7: doanhNghiep.SoThuTuc7CA = soDangKyMax; break;
                    case 8: doanhNghiep.SoThuTuc8CA = soDangKyMax; break;
                    case 9: doanhNghiep.SoThuTuc9CA = soDangKyMax; break; ;
                    case 10: doanhNghiep.SoThuTuc10CA = soDangKyMax; break;
                }
            }
        }

        public string SinhSoDangKy(long doanhNghiepId, int thuTucId = 0)
        {
            try
            {
                CommonENum.THU_TUC_ID thuTuc = CommonENum.THU_TUC_ID.THU_TUC_99;
                if (thuTucId > 0)
                {
                    thuTuc = (CommonENum.THU_TUC_ID)thuTucId;
                }

                string strThuTuc = "";
                var res = string.Empty;
                if (doanhNghiepId > 0)
                {
                    var doanhNghiepKySoNam = _doanhNghiepRepos.FirstOrDefault(x => x.NamCA == DateTime.Now.Year && x.Id == doanhNghiepId);
                    if (doanhNghiepKySoNam != null)
                    {
                        strThuTuc = CommonENum.GetEnumDescription(thuTuc);
                        var soMoi = SoDangKyMax(doanhNghiepKySoNam, thuTuc) + 1;
                        res = string.Format("{0}/{1}/{2}", soMoi, doanhNghiepKySoNam.MaSoThue, DateTime.Now.Year);
                        UpdateSoDangKyMax(ref doanhNghiepKySoNam, thuTuc, soMoi);
                        _doanhNghiepRepos.Update(doanhNghiepKySoNam);
                    }
                    else
                    {
                        var doanhNghiepNull = _doanhNghiepRepos.FirstOrDefault(x => x.Id == doanhNghiepId);
                        doanhNghiepNull.NamCA = DateTime.Now.Year;

                        #region UpDate Số Thủ tục
                        doanhNghiepNull.SoThuTuc0CA = 0;

                        doanhNghiepNull.SoThuTuc1CA = 0;
                        doanhNghiepNull.SoThuTuc2CA = 0;
                        doanhNghiepNull.SoThuTuc3CA = 0;
                        doanhNghiepNull.SoThuTuc4CA = 0;
                        doanhNghiepNull.SoThuTuc5CA = 0;
                        doanhNghiepNull.SoThuTuc6CA = 0;
                        doanhNghiepNull.SoThuTuc7CA = 0;
                        doanhNghiepNull.SoThuTuc8CA = 0;
                        doanhNghiepNull.SoThuTuc9CA = 0;
                        doanhNghiepNull.SoThuTuc10CA = 0;
                        #endregion

                        strThuTuc = CommonENum.GetEnumDescription(thuTuc);
                        var soMoi = 1;
                        res = string.Format("{0}/{1}/{2}", soMoi, doanhNghiepNull.MaSoThue, DateTime.Now.Year);
                        UpdateSoDangKyMax(ref doanhNghiepNull, thuTuc, 1);
                        _doanhNghiepRepos.Update(doanhNghiepNull);
                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region Sinh số công văn
        public int SinhSoCongVan(long hoSoXuLyId, int phanHeId = 0, bool IsTemp = false)
        {
            try
            {
                int soCongVanResult = 1;
                int namCA = DateTime.Now.Year;

                var objSoCongVan = _capSoCongVanRepos.GetAll().Where(x => x.NhomThuTucId == phanHeId && x.HoSoXuLyId == hoSoXuLyId && x.Nam == namCA).FirstOrDefault();

                if (objSoCongVan != null)
                {
                    soCongVanResult = objSoCongVan.So;
                }
                else
                {
                    var soCongVanMaxOfNam = _capSoCongVanRepos.GetAll().Where(x => x.NhomThuTucId == phanHeId && x.Nam == namCA).OrderByDescending(o => o.So).FirstOrDefault();

                    int soCongVanMoi = 1;
                    if (soCongVanMaxOfNam != null)
                    {
                        soCongVanMoi = soCongVanMaxOfNam.So + 1;
                    }
                    if (IsTemp != true)
                    {
                        _capSoCongVanRepos.Insert(new CapSoCongVan
                        {
                            NhomThuTucId = phanHeId,
                            HoSoXuLyId = hoSoXuLyId,
                            Nam = namCA,
                            So = soCongVanMoi
                        });
                    }
                    soCongVanResult = soCongVanMoi;
                }

                return soCongVanResult;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        #endregion

        #region Sinh số quyết định
        public int SinhSoQuyetDinh(long hoSoXuLyId, int nhomThuTucId = 0, bool IsTemp = false)
        {
            try
            {
                int soQuyetDinhResult = 1;
                int namCA = DateTime.Now.Year;

                var objSoQuyetDinh = _capSoQuyetDinhRepos.GetAll().Where(x => x.NhomThuTucId == nhomThuTucId && x.HoSoXuLyId == hoSoXuLyId && x.Nam == namCA).FirstOrDefault();

                if (objSoQuyetDinh != null)
                {
                    soQuyetDinhResult = objSoQuyetDinh.So;
                }
                else
                {
                    var soQuyetDinhMaxOfNam = _capSoQuyetDinhRepos.GetAll().Where(x => x.NhomThuTucId == nhomThuTucId && x.Nam == namCA).OrderByDescending(o => o.So).FirstOrDefault();

                    int soQuyetDinhMoi = 1;
                    if (soQuyetDinhMaxOfNam != null)
                    {
                        soQuyetDinhMoi = soQuyetDinhMaxOfNam.So + 1;
                    }
                    if (IsTemp != true)
                    {
                        _capSoQuyetDinhRepos.Insert(new CapSoQuyetDinh
                        {
                            NhomThuTucId = nhomThuTucId,
                            HoSoXuLyId = hoSoXuLyId,
                            Nam = namCA,
                            So = soQuyetDinhMoi
                        });
                    }
                    soQuyetDinhResult = soQuyetDinhMoi;
                }

                return soQuyetDinhResult;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        #endregion
    }
}
