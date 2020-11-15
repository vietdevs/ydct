using Abp.Application.Services;
using Abp.Authorization;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using newPSG.PMS.Authorization.Users;
using newPSG.PMS.Dto;
using newPSG.PMS.Editions;
using newPSG.PMS.EntityDB;
using newPSG.PMS.KeypayServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using static newPSG.PMS.CommonENum;

#region Class Riêng Cho Từng Thủ tục

using XHoSo = newPSG.PMS.EntityDB.HoSo37;
using XHoSoTepDinhKem = newPSG.PMS.EntityDB.HoSoTepDinhKem37;
using XHoSoXuLy = newPSG.PMS.EntityDB.HoSoXuLy37;
using XHoSoXuLyHistory = newPSG.PMS.EntityDB.HoSoXuLyHistory37;
using XHoSoDto = newPSG.PMS.Dto.HoSo37Dto;
using System.IO;
using System.Configuration;

#endregion Class Riêng Cho Từng Thủ tục

namespace newPSG.PMS.Services
{
    #region INTERFACE

    public interface IXuLyHoSoDoanhNghiep37AppService : IApplicationService
    {
        #region Public Common

        int GetPhongBanIdXuLyHoSo(XHoSo hoSo);

        #endregion Public Common

        #region Thêm & Sửa & Hủy Hồ Sơ

        Task<dynamic> GetHoSoById(long hoSoId);

        Task<long> CreateOrUpdateHoSo(CreateOrUpdateHoSo37Input input);

        Task OpenHuyHoSo(long hoSoId);

        Task TaoBanSaoHoSo(long hoSoId);

        #endregion Thêm & Sửa & Hủy Hồ Sơ


        #region Nộp hồ sơ để rà soát

        Task NopHoSoDeRaSoat(HoSoNopRaSoat37InputDto input);

        Task NopHoSoTraLai(long hoSoId);

        #endregion Nộp hồ sơ để rà soát


        #region Nộp hồ sơ bổ sung

        Task NopHoSoBoSung(long hoSoId);

        #endregion Nộp hồ sơ bổ sung

        #region File đính kèm

        List<HoSoTepDinhKem37Dto> GetHoSoTepDinhKem(long hoSoId);

        bool CopyFileTaiLieuDaTaiVaoHoSo(CopyFileTaiLieuDaTaiVaoHoSo37Input input);

        #endregion File đính kèm

        Task<List<TT37_PhamViHoatDong>> GetListPhamViToDDL();
        List<ItemObj<int>> GetListVanBangToDDL();
    }

    #endregion INTERFACE

    #region MAIN

    [AbpAuthorize]
    public class XuLyHoSoDoanhNghiep37AppService : PMSAppServiceBase, IXuLyHoSoDoanhNghiep37AppService
    {
        private readonly EditionManager _editionManager;
        private readonly IRepository<DoanhNghiep, long> _doanhNghiepRepos;
        private readonly IRepository<XHoSo, long> _hoSoRepos;
        private readonly IRepository<XHoSoTepDinhKem, long> _hoSoTepDinhKemRepos;
        private readonly IRepository<XHoSoXuLy, long> _hoSoXuLyRepos;
        private readonly IRepository<XHoSoXuLyHistory, long> _hoSoXuLyHistoryRepos;
        private readonly IRepository<LoaiHoSo> _loaiHoSoRepos;
        private readonly IRepository<TT37_PhamViHoatDong> _phamViHoatDongRepos;
        private readonly IRepository<TT37_HoSoPhamViHD,long> _hosoPhamViHDRepos;
        private readonly IRepository<PhongBan> _phongBanRepos;
        private readonly IRepository<PhongBanLoaiHoSo> _phongBanLoaiHoSoRepos;
        private readonly IRepository<User, long> _userRepos;
        private readonly IRepository<QuocGia> _quocGiaRepos;
        private readonly IAbpSession _session;
        private readonly IRepository<ThanhToan, long> _thanhToanRepos;
        private readonly IRepository<LogUploadFile, long> _uploadFileRepos;

        private readonly ILichLamViecAppService _lichLamViecAppService;
        private readonly ICustomTennantAppService _customTennantAppService;
        private readonly CustomSessionAppSession _mySession;
        private readonly CapSoThuTucAppService _capSoThuTucAppService;
        private readonly ThanhToanKeyPayAppService _thanhToanKeyPayAppService;

        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public XuLyHoSoDoanhNghiep37AppService(
            EditionManager editionManager,
            IRepository<DoanhNghiep, long> doanhNghiepRepos,
            IRepository<XHoSo, long> hoSoRepos,
            IRepository<XHoSoTepDinhKem, long> hoSoTepDinhKemRepos,
            IRepository<XHoSoXuLy, long> hoSoXuLyRepos,
            IRepository<XHoSoXuLyHistory, long> hoSoXuLyHistoryRepos,
            IRepository<LoaiHoSo> loaiHoSoRepos,
            IRepository<TT37_PhamViHoatDong> phamViHoatDongRepos,
            IRepository<TT37_HoSoPhamViHD, long> hosoPhamViHDRepos,
            IRepository<PhongBan> phongBanRepos,
            IRepository<PhongBanLoaiHoSo> phongBanLoaiHoSoRepos,
            IRepository<User, long> userRepos,
            IRepository<QuocGia> quocGiaRepos,
            IAbpSession session,
            IRepository<ThanhToan, long> thanhToanRepos,
            IRepository<LogUploadFile, long> uploadFileRepos,

            ILichLamViecAppService lichLamViecAppService,
            ICustomTennantAppService customTennantAppService,
            CustomSessionAppSession mySession,
            CapSoThuTucAppService capSoThuTucAppService,
            ThanhToanKeyPayAppService thanhToanKeyPayAppService,

            IUnitOfWorkManager unitOfWorkManager)
        {
            _editionManager = editionManager;
            _doanhNghiepRepos = doanhNghiepRepos;
            _hoSoRepos = hoSoRepos;
            _hoSoXuLyRepos = hoSoXuLyRepos;
            _hoSoTepDinhKemRepos = hoSoTepDinhKemRepos;
            _hoSoXuLyHistoryRepos = hoSoXuLyHistoryRepos;
            _phamViHoatDongRepos = phamViHoatDongRepos;
            _hosoPhamViHDRepos = hosoPhamViHDRepos;
            _loaiHoSoRepos = loaiHoSoRepos;
            _phongBanRepos = phongBanRepos;
            _phongBanLoaiHoSoRepos = phongBanLoaiHoSoRepos;
            _userRepos = userRepos;
            _quocGiaRepos = quocGiaRepos;
            _session = session;
            _thanhToanRepos = thanhToanRepos;
            _uploadFileRepos = uploadFileRepos;

            _mySession = mySession;

            _lichLamViecAppService = lichLamViecAppService;
            _customTennantAppService = customTennantAppService;
            _capSoThuTucAppService = capSoThuTucAppService;
            _thanhToanKeyPayAppService = thanhToanKeyPayAppService;

            _unitOfWorkManager = unitOfWorkManager;
        }

        #region Function Common

        private int GetTenantIdFromHoSo(XHoSo hoSo)
        {
            var tenantId = _customTennantAppService.GetTenantIdCucHCC();
            if (hoSo != null && hoSo.ChiCucId.HasValue)
            {
                tenantId = hoSo.ChiCucId.Value;
            }
            return tenantId;
        }

        public int GetPhongBanIdXuLyHoSo(XHoSo hoSo)
        {
            //Loai ho so
            int tenantId = GetTenantIdFromHoSo(hoSo);
            int phongBanId = -1;

            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant))
            {
                var _listPhongBanXuLy = (from pn in _phongBanLoaiHoSoRepos.GetAll()
                                         where pn.LoaiHoSoId == hoSo.LoaiHoSoId && pn.TenantId == tenantId
                                         select pn.PhongBanId).ToList();

                if (_listPhongBanXuLy != null && _listPhongBanXuLy.Count == 1)
                {
                    phongBanId = _listPhongBanXuLy[0];
                }
            }

            return phongBanId;
        }

        #endregion Function Common

        #region Thêm & Sửa & Hủy Hồ Sơ

        public async Task<dynamic> GetHoSoById(long hoSoId)
        {
            try
            {
                if (hoSoId > 0)
                {
                    var hoSo = await _hoSoRepos.GetAsync(hoSoId);
                    if (hoSo != null)
                    {
                        var _queryTep = from tep in _hoSoTepDinhKemRepos.GetAll()
                                        where (tep.HoSoId == hoSo.Id && tep.IsActive == true)
                                        orderby tep.Id
                                        select tep;
                        var _phamViHoatDongIdArr = await _hosoPhamViHDRepos.GetAll().Where(x => x.HoSoId == hoSoId).Select(x=>x.PhamViHoatDongId).ToListAsync();
                        return new
                        {
                            Status = true,
                            HoSo = hoSo,
                            Teps = await _queryTep.ToListAsync(),
                            PhamViHoatDongIdArr = _phamViHoatDongIdArr
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
            }
            return new
            {
                Status = false
            };
        }

        public async Task<long> CreateOrUpdateHoSo(CreateOrUpdateHoSo37Input input)
        {
            try
            {
                long _id = 0;
                if (input.HoSo.Id > 0)
                {
                    _id = await UpdateHoSo(input);
                }
                else
                {
                    //SttHoso
                    input.HoSo.NamHoSo = DateTime.Now.Year;
                    int? sttHoSoMax = _hoSoRepos.GetAll().Where(x => x.DoanhNghiepId == input.HoSo.DoanhNghiepId && x.NamHoSo == input.HoSo.NamHoSo).Max(x => x.SttHoSo);

                    if (sttHoSoMax != null)
                    {
                        input.HoSo.SttHoSo = sttHoSoMax + 1;
                    }
                    else
                    {
                        input.HoSo.SttHoSo = 1;
                    }

                    //Add HoSo
                    _id = await CreateHoSo(input);
                }

                var _arrUploadFileId = (from tep in _hoSoTepDinhKemRepos.GetAll()
                                        where (tep.HoSoId == _id && tep.UploadFileId.HasValue && tep.UploadFileId > 0)
                                        orderby tep.Id
                                        select tep.UploadFileId).ToArray();

                if (_arrUploadFileId != null && _arrUploadFileId.Length > 0)
                {
                    var lstUpload = await (from uploadObj in _uploadFileRepos.GetAll().Where(x => _arrUploadFileId.Contains(x.Id)) select uploadObj).ToListAsync();
                    foreach (var upObj in lstUpload)
                    {
                        upObj.ThuTucId = (int)CommonENum.THU_TUC_ID.THU_TUC_37;
                        upObj.HoSoId = _id;
                        upObj.DaSuDung = true;
                        await _uploadFileRepos.UpdateAsync(upObj);
                    }
                }

                return _id;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private async Task<long> UpdateHoSo(CreateOrUpdateHoSo37Input input)
        {
            try
            {
                var updateData = await _hoSoRepos.GetAsync(input.HoSo.Id);
                if (updateData != null)
                {
                    //update lại tài liệu
                    var userId = _session.UserId;
                    var teps = _hoSoTepDinhKemRepos.GetAll().Where(x => x.HoSoId == updateData.Id && x.IsActive == true);
                    if (teps != null && teps.Count() != 0)
                    {
                        foreach (var tep in teps)
                        {
                            if (updateData.TrangThaiHoSo != (int)TRANG_THAI_HO_SO.DA_HOAN_TAT)
                            {
                                await _hoSoTepDinhKemRepos.DeleteAsync(tep);
                            }
                        }
                    }

                    foreach (var tailieu in input.Teps)
                    {

                        var insertTailieu = tailieu.MapTo<XHoSoTepDinhKem>();
                        string HCC_FILE_PDF = GetUrlFileDefaut();
                        insertTailieu.ThuTucId = (int)CommonENum.THU_TUC_ID.THU_TUC_37;
                        insertTailieu.HoSoId = updateData.Id;
                        if (!String.IsNullOrEmpty(tailieu.DuongDanTep))
                        {
                            insertTailieu.Id = 0;
                            insertTailieu.DaTaiLen = true;
                            insertTailieu.IsActive = true;
                            insertTailieu.TenTep = tailieu.TenTep;
                            insertTailieu.DuongDanTep = tailieu.DuongDanTep;
                            insertTailieu.LoaiTepDinhKem = tailieu.LoaiTepDinhKem;
                        }
                        await _hoSoTepDinhKemRepos.InsertAsync(insertTailieu);
                    }

                    if (input.PhamViHoatDongIdArr.Count > 0)
                    {
                        var listPhamViIdSaved = await _hosoPhamViHDRepos.GetAll().Where(x => x.HoSoId == updateData.Id).Select(x=>x.PhamViHoatDongId).ToListAsync();

                        foreach (var item in input.PhamViHoatDongIdArr)
                        {
                            if (!listPhamViIdSaved.Contains(item))
                            {
                                var hoSoPhamVi = new TT37_HoSoPhamViHD()
                                {
                                    HoSoId = updateData.Id,
                                    PhamViHoatDongId = item,
                                };
                                await _hosoPhamViHDRepos.InsertAsync(hoSoPhamVi);
                            }
                        }
                        foreach (var item in listPhamViIdSaved)
                        {
                            if (!input.PhamViHoatDongIdArr.Contains(item.Value))
                            {
                                await _hosoPhamViHDRepos.DeleteAsync(x => x.PhamViHoatDongId == item && x.HoSoId == updateData.Id);
                            }
                        }

                    }

                    //update lai thong tin textbox ho so
                    input.HoSo.MapTo(updateData);
                    updateData.IsCA = false;
                    updateData.DuongDanTepCA = null;
                    //Loai ho so
                    if (updateData.LoaiHoSoId.HasValue)
                    {
                        var lhs = await _loaiHoSoRepos.GetAsync(updateData.LoaiHoSoId.Value);
                        updateData.TenLoaiHoSo = lhs.TenLoaiHoSo;
                    }
                    #region Sinh Mã Hồ Sơ

                    if (string.IsNullOrEmpty(updateData.MaHoSo))
                    {
                        updateData.MaHoSo = SinhMaHoSo(updateData.Id, updateData.CreationTime, updateData.ThuTucId.Value);
                    }
                    if (string.IsNullOrEmpty(updateData.SoDangKy))
                    {
                        updateData.SoDangKy = _capSoThuTucAppService.SinhSoDangKy(updateData.DoanhNghiepId.Value, (int)CommonENum.THU_TUC_ID.THU_TUC_37);
                    }

                    #endregion Sinh Mã Hồ Sơ

                    if (updateData.TrangThaiHoSo != (int)TRANG_THAI_HO_SO.SUA_DOI_BO_SUNG && updateData.TrangThaiHoSo != (int)TRANG_THAI_HO_SO.MOT_CUA_TRA_LAI)
                    {
                        updateData.TrangThaiHoSo = input.HoSo.TrangThaiHoSo;
                    }
                    await _hoSoRepos.UpdateAsync(updateData);
                }
                CurrentUnitOfWork.SaveChanges();
                return updateData.Id;
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                return 0;
            }
        }

        private async Task<long> CreateHoSo(CreateOrUpdateHoSo37Input input)
        {
            try
            {
                var insertInput = input.HoSo.MapTo<XHoSo>();
                if (insertInput.LoaiHoSoId.HasValue)
                {
                    var lhs = await _loaiHoSoRepos.GetAsync(insertInput.LoaiHoSoId.Value);
                    insertInput.TenLoaiHoSo = lhs.TenLoaiHoSo;
                }
                insertInput.SoDangKy = _capSoThuTucAppService.SinhSoDangKy(input.HoSo.DoanhNghiepId.Value, (int)CommonENum.THU_TUC_ID.THU_TUC_37);
                long id = await _hoSoRepos.InsertAndGetIdAsync(insertInput);

                #region Sinh Mã Hồ Sơ

                var hoSo = _hoSoRepos.Get(id);
                if (string.IsNullOrEmpty(hoSo.MaHoSo))
                {
                    hoSo.MaHoSo = SinhMaHoSo(hoSo.Id, hoSo.CreationTime, hoSo.ThuTucId.Value);
                    await _hoSoRepos.UpdateAsync(hoSo);
                }

                #endregion Sinh Mã Hồ Sơ

                /// upload file tailieu
                if (input.Teps != null)
                {
                    foreach (var tailieu in input.Teps)
                    {
                        if (tailieu.IsSoCongBo == true)
                        {
                            continue;
                        }
                        var insertTailieu = tailieu.MapTo<XHoSoTepDinhKem>();
                        string HCC_FILE_PDF = GetUrlFileDefaut();
                        insertTailieu.ThuTucId = (int)CommonENum.THU_TUC_ID.THU_TUC_37;
                        insertTailieu.HoSoId = id;
                        if (!String.IsNullOrEmpty(tailieu.DuongDanTep))
                        {
                            insertTailieu.DaTaiLen = true;
                            insertTailieu.IsActive = true;
                            insertTailieu.TenTep = tailieu.TenTep;
                            insertTailieu.DuongDanTep = tailieu.DuongDanTep;
                            insertTailieu.LoaiTepDinhKem = tailieu.LoaiTepDinhKem;
                        }
                        await _hoSoTepDinhKemRepos.InsertAsync(insertTailieu);
                    }
                }

                // insert pham vi hoat dong
                if(input.PhamViHoatDongIdArr.Count > 0)
                {
                    foreach (var item in input.PhamViHoatDongIdArr)
                    {
                        var hoSoPhamVi = new TT37_HoSoPhamViHD()
                        {
                            HoSoId = id,
                            PhamViHoatDongId = item,
                        };
                        await _hosoPhamViHDRepos.InsertAsync(hoSoPhamVi);
                    }
                }

                CurrentUnitOfWork.SaveChanges();
                return id;
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                return 0;
            }
        }

        public async Task OpenHuyHoSo(long hoSoId)
        {
            try
            {
                using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.SoftDelete))
                {
                    var hoSo = _hoSoRepos.Get(hoSoId);
                    if (hoSo == null)
                    {
                        return;
                    }

                    if ((hoSo.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.DA_LUU) || (hoSo.TrangThaiHoSo == null))
                    {
                        var teps = _hoSoTepDinhKemRepos.GetAll().Where(x => x.HoSoId == hoSo.Id);
                        foreach (var tep in teps)
                        {
                            await _hoSoTepDinhKemRepos.DeleteAsync(tep);
                        }

                        await _hoSoRepos.DeleteAsync(hoSo);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
            }
        }

        public async Task TaoBanSaoHoSo(long hoSoId)
        {
            var hoSo = await _hoSoRepos.GetAsync(hoSoId);
            if (hoSo != null)
            {
                var hoSoClone = new XHoSoDto();
                hoSoClone = hoSo.MapTo<XHoSoDto>();
                hoSoClone.Id = 0;
                hoSoClone.IsCA = false;
                hoSoClone.PId = null;
                hoSoClone.CreationTime = DateTime.Now;
                hoSoClone.TrangThaiHoSo = null;
                hoSoClone.HoSoXuLyId_Active = null;
                hoSoClone.MotCuaChuyenId = null;
                hoSoClone.PhongBanId = null;
                hoSoClone.IsHoSoBS = null;
                hoSoClone.GiayTiepNhan = null;
                hoSoClone.GiayTiepNhanFull = null;
                hoSoClone.MaHoSo = null;
                hoSoClone.SoDangKy = null;
                hoSoClone.StrThuMucHoSo = "HOSO_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");

                //SttHoso
                hoSoClone.NamHoSo = DateTime.Now.Year;
                int? sttHoSoMax = _hoSoRepos.GetAll().Where(x => x.DoanhNghiepId == hoSoClone.DoanhNghiepId && x.NamHoSo == hoSoClone.NamHoSo).Max(x => x.SttHoSo);

                if (sttHoSoMax != null)
                {
                    hoSoClone.SttHoSo = 1;
                }
                else
                {
                    hoSoClone.SttHoSo = sttHoSoMax + 1;
                }

                var _hsClone = hoSoClone.MapTo<XHoSo>();
                _hsClone.Id = await _hoSoRepos.InsertAndGetIdAsync(_hsClone);

                #region Copy tệp đính kèm

                var tepdk = _hoSoTepDinhKemRepos.GetAll().Where(x => x.HoSoId == hoSo.Id);
                foreach (var item in tepdk)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(item.DuongDanTep))
                        {
                            var fileName = item.DuongDanTep.Split('\\').LastOrDefault();
                            var name = fileName.Split('.').FirstOrDefault();
                            var extention = fileName.Split('.').LastOrDefault();
                            var urlPath = item.DuongDanTep.Replace(name, name + "_" + _hsClone.Id);
                            var tep = new XHoSoTepDinhKem()
                            {
                                CreationTime = DateTime.Now,
                                HoSoId = _hsClone.Id,
                                DuongDanTep = urlPath,
                                TenTep = urlPath,
                                IsCA = item.IsCA,
                                MoTaTep = item.MoTaTep,
                                LoaiTepDinhKem = item.LoaiTepDinhKem,
                                ThuTucId = item.ThuTucId,
                                DaTaiLen = item.DaTaiLen,
                                IsActive = item.IsActive
                            };
                            var dkClone = tep.MapTo<XHoSoTepDinhKem>();
                            await _hoSoTepDinhKemRepos.InsertAsync(dkClone);

                            #region Tạo bản sao tệp đính kèm

                            var pathName = item.DuongDanTep.Replace(fileName, string.Empty);
                            string HCC_FILE_PDF = GetUrlFileDefaut();
                            var folderPath = Path.Combine(HCC_FILE_PDF, pathName);
                            if (!Directory.Exists(folderPath))
                                Directory.CreateDirectory(folderPath);
                            var sourceFile = System.IO.Path.Combine(folderPath, fileName);
                            string destFile = System.IO.Path.Combine(folderPath, name + "_" + _hsClone.Id + "." + extention);
                            if (File.Exists(sourceFile))
                                System.IO.File.Copy(sourceFile, destFile, true);

                            #endregion Tạo bản sao tệp đính kèm
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Fatal(ex.Message);
                    }
                }

                #endregion Copy tệp đính kèm
            }
        }

        #endregion Thêm & Sửa & Hủy Hồ Sơ

       


        #region Nộp hồ sơ để rà soát

        public async Task NopHoSoDeRaSoat(HoSoNopRaSoat37InputDto input)
        {
            try
            {
                using (var unitOfWork = _unitOfWorkManager.Begin())
                {
                    var hoso = _hoSoRepos.Get(input.HoSoId);

                    var hsxl = new XHoSoXuLy();

                    if (hoso.HoSoXuLyId_Active.HasValue)
                    {
                        hsxl = _hoSoXuLyRepos.Get(hoso.HoSoXuLyId_Active.Value);
                    }
                    hsxl.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.MOT_CUA_TIEP_NHAN;
                    hsxl.DonViGui = (int)CommonENum.DON_VI_XU_LY.DOANH_NGHIEP;
                    hsxl.NguoiGuiId = _session.UserId;
                    hsxl.HoSoId = input.HoSoId;
                    hsxl.NgayGui = DateTime.Now;
                    hsxl.NgayTiepNhan = DateTime.Now;
                    hsxl.LuongXuLy = (int)CommonENum.LUONG_XU_LY_TT37.LUONG_RA_SOAT;

                    #region Tính Ngày hẹn trả

                    //hsxl.LoaiHoSoId = hoso.LoaiHoSoId;
                    //var loaiHoSo = await _loaiHoSoRepos.FirstOrDefaultAsync(x => x.Id == hoso.LoaiHoSoId);
                    //if (loaiHoSo != null && loaiHoSo.SoNgayXuLy.HasValue)
                    //{
                    //    DateTime ngayHenTra = _lichLamViecAppService.GetNgayHenTra(hsxl.NgayTiepNhan.Value, loaiHoSo.SoNgayXuLy.Value);
                    //    hsxl.NgayHenTra = ngayHenTra;
                    //}

                    #endregion Tính Ngày hẹn trả

                    var _hsxlId = await _hoSoXuLyRepos.InsertOrUpdateAndGetIdAsync(hsxl);

                    //Thêm History
                    var history = new XHoSoXuLyHistory();
                    history.NgayXuLy = DateTime.Now;
                    history.HoSoId = input.HoSoId;
                    history.HoSoXuLyId = _hsxlId;
                    history.ThuTucId = hoso.ThuTucId;
                    history.NguoiXuLyId = _session.UserId;
                    history.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.DOANH_NGHIEP;
                    history.ActionEnum = (int)CommonENum.FORM_FUNCTION.DOANH_NGHIEP_NOP_HO_SO;
                    await _hoSoXuLyHistoryRepos.InsertOrUpdateAndGetIdAsync(history);

                    //Update trang thai ho so doanh nghiep
                    hoso.IsCA = true;
                    hoso.DuongDanTepCA = input.DuongDanTep;
                    hoso.TrangThaiHoSo = (int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_HO_SO_MOI;
                    hoso.HoSoXuLyId_Active = _hsxlId;
                    hoso.NgayNopRaSoat = DateTime.Now;
                    await _hoSoRepos.UpdateAsync(hoso);
                    unitOfWork.Complete();
                }
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
            }
        }

        public async Task NopHoSoTraLai(long hoSoId)
        {
            try
            {
                var hoSo = _hoSoRepos.FirstOrDefault(hoSoId);

                if (hoSo.Id > 0)
                {
                    if (hoSo.HoSoXuLyId_Active.HasValue)
                    {
                        var hosoxl = _hoSoXuLyRepos.FirstOrDefault(hoSo.HoSoXuLyId_Active.Value);
                        hosoxl.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.MOT_CUA_TIEP_NHAN;
                        hosoxl.DonViGui = (int)CommonENum.DON_VI_XU_LY.DOANH_NGHIEP;
                        hosoxl.NguoiGuiId = _session.UserId;
                        hosoxl.NguoiXuLyId = hoSo.CreatorUserId;
                        hosoxl.YKienGui = null;
                        hosoxl.HoSoIsDat = null;
                        hosoxl.NgayGui = DateTime.Now;
                        hosoxl.LuongXuLy = (int)CommonENum.LUONG_XU_LY_TT37.LUONG_RA_SOAT;
                        #region Lưu lịch sử

                        var _history = new XHoSoXuLyHistory();
                        _history.HoSoXuLyId = hosoxl.Id;
                        _history.ThuTucId = hoSo.ThuTucId;
                        _history.HoSoId = hoSo.Id;
                        _history.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.DOANH_NGHIEP;
                        _history.DonViKeTiep = (int)CommonENum.DON_VI_XU_LY.MOT_CUA_TIEP_NHAN;
                        _history.NguoiXuLyId = _session.UserId;
                        _history.NoiDungYKien = null;
                        _history.ActionEnum = (int)CommonENum.FORM_FUNCTION.DOANH_NGHIEP_NOP_HO_SO;

                        var _historyId = await _hoSoXuLyHistoryRepos.InsertOrUpdateAndGetIdAsync(_history);
                        hosoxl.HoSoXuLyHistoryId_Active = null;

                        #endregion Lưu lịch sử

                        await _hoSoXuLyRepos.UpdateAsync(hosoxl);
                    }
                    hoSo.TrangThaiHoSo = (int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_HO_SO_MOI;
                    await _hoSoRepos.UpdateAsync(hoSo);
                }
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
            }
        }

        #endregion Nộp hồ sơ để rà soát


        #region Nộp hồ sơ bổ sung

        public async Task NopHoSoBoSung(long hoSoId)
        {
            try
            {
                var hoSo = _hoSoRepos.Get(hoSoId);
                if (hoSo == null)
                {
                    return;
                }
                if (hoSo.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.SUA_DOI_BO_SUNG)
                {
                    XHoSoXuLy hoSoXuLyOld = _hoSoXuLyRepos.FirstOrDefault(hoSo.HoSoXuLyId_Active.Value);
                    if (hoSoXuLyOld != null)
                    {
                        XHoSoXuLy hoSoXuLy = new XHoSoXuLy();
                        hoSoXuLy.ThuTucId = hoSo.ThuTucId;
                        hoSoXuLy.HoSoId = hoSo.Id;
                        hoSoXuLy.LoaiHoSoId = hoSo.LoaiHoSoId;
                        hoSoXuLy.QuiTrinh = hoSoXuLyOld.QuiTrinh;
                        hoSoXuLy.IsHoSoBS = true;
                        hoSoXuLy.NgayTiepNhan = DateTime.Now;
                        hoSoXuLy.NgayHenTra = _lichLamViecAppService.GetNgayHenTra(hoSoXuLy.NgayTiepNhan.Value, 7);
                        hoSoXuLy.VanThuId = hoSoXuLyOld.VanThuId;

                        hoSoXuLy.DonViGui = (int)CommonENum.DON_VI_XU_LY.DOANH_NGHIEP;
                        hoSoXuLy.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.MOT_CUA_TIEP_NHAN;
                        hoSoXuLy.NguoiXuLyId = hoSoXuLyOld.VanThuId;

                        hoSoXuLy.LuongXuLy = (int)CommonENum.LUONG_XU_LY_TT37.LUONG_RA_SOAT;
                        hoSoXuLy.NgayGui = DateTime.Now;
                        hoSoXuLy.NguoiGuiId = _session.UserId;
                        hoSoXuLy.YKienGui = null;

                        //Thay đổi hồ sơ xử lý
                        long id = await _hoSoXuLyRepos.InsertAndGetIdAsync(hoSoXuLy);
                        hoSo.HoSoXuLyId_Active = id;   

                        hoSo.TrangThaiHoSo = (int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_BO_SUNG;
                        await _hoSoRepos.UpdateAsync(hoSo);

                        #region Lưu lịch sử

                        var _history = new XHoSoXuLyHistory();
                        _history.HoSoXuLyId = id;
                        _history.ThuTucId = hoSo.ThuTucId;
                        _history.HoSoId = hoSo.Id;
                        _history.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.DOANH_NGHIEP;
                        _history.DonViKeTiep = (int)CommonENum.DON_VI_XU_LY.MOT_CUA_TIEP_NHAN;
                        _history.NguoiXuLyId = _session.UserId;
                        _history.ActionEnum = (int)CommonENum.FORM_FUNCTION.NOP_HO_SO_BO_SUNG;
                        await _hoSoXuLyHistoryRepos.InsertOrUpdateAndGetIdAsync(_history);

                        #endregion Lưu lịch sử
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
            }
        }

        #endregion Nộp hồ sơ bổ sung

        #region File đính kèm - có lưu pdf temp

        public bool CopyFileTaiLieuDaTaiVaoHoSo(CopyFileTaiLieuDaTaiVaoHoSo37Input input)
        {
            bool ret = false;
            try
            {
                if (input.DanhSachFileDaTai == null || input.DanhSachFileDaTai.Count == 0) return true;
                string _hccFile = $@"{ConfigurationManager.AppSettings["HCC_FILE_PDF"]}";

                string _folderMain = GetPathSaveHoSoFolder(input.HoSoId.Value);
                var savePath = Path.Combine($@"{_hccFile}\{_folderMain}");
                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(savePath);
                }

                var tempPath = Path.Combine($@"{_hccFile}\{GetPathSaveHoSoFolder()}\{input.FolderTemp}");
                if (Directory.Exists(tempPath))
                {
                    foreach (var fileDaTai in input.DanhSachFileDaTai)
                    {
                        // move file
                        string tenFile = fileDaTai.TenTep + ".pdf";
                        File.Move(Path.Combine($@"{tempPath}", tenFile), Path.Combine(savePath, tenFile));

                        // them tep dinh kem vao db
                        if (!_hoSoTepDinhKemRepos.GetAll().Any(x => x.HoSoId == input.HoSoId && x.DuongDanTep == fileDaTai.DuongDanTep))
                        {
                            var insertHoSoTep = new HoSoTepDinhKem37()
                            {
                                HoSoId = input.HoSoId,
                                LoaiTepDinhKem = fileDaTai.LoaiTepDinhKem,
                                TenTep = $@"{_folderMain}\{tenFile}",
                                DuongDanTep = $@"{_folderMain}\{tenFile}",
                                MoTaTep = fileDaTai.MoTaTep,
                                IsActive = true,
                                DaTaiLen = true,
                            };
                            _hoSoTepDinhKemRepos.Insert(insertHoSoTep);
                        }
                    }
                    Directory.Delete(tempPath);
                }
                ret = true;
            }
            catch (Exception ex)
            {
                Logger.Error("CopyFileTaiLieuDaTaiVaoHoSo" + JsonConvert.SerializeObject(ex));
            }
            return ret;
        }

        private string GetPathSaveHoSoFolder(long hoSoId = 0)
        {
            var doanhNghiepId = SessionCustom.UserCurrent.DoanhNghiepId;

            var doanhnghiep = _doanhNghiepRepos.FirstOrDefault(x => x.Id == doanhNghiepId);
            string strTinh = (doanhnghiep != null && !string.IsNullOrEmpty(doanhnghiep.Tinh)) ? RemoveUnicode(doanhnghiep.Tinh).ToLower().Trim().Replace(" ", "-") : "tinh";
            string maSoThue = doanhnghiep != null ? doanhnghiep.MaSoThue : "masothue";
            string hoSoIdFolder = $"hoSoId_{hoSoId.ToString().PadLeft(20, '0')}";
            if (hoSoId == 0)
            {
                return $@"HoSo-TT37\\{strTinh}\\{maSoThue}\\{ DateTime.Now.Year.ToString()}";
            }
            return $@"HoSo-TT37\\{strTinh}\\{maSoThue}\\{ DateTime.Now.Year.ToString()}\\{hoSoIdFolder}";
        }

        public List<HoSoTepDinhKem37Dto> GetHoSoTepDinhKem(long hoSoId)
        {
            var query = from tep in _hoSoTepDinhKemRepos.GetAll()
                        where tep.HoSoId == hoSoId
                        where tep.IsActive == true
                        select new HoSoTepDinhKem37Dto
                        {
                            HoSoId = tep.HoSoId,
                            DuongDanTep = tep.DuongDanTep,
                            TenTep = tep.TenTep,
                            MoTaTep = tep.MoTaTep,
                        };
            return query.ToList();
        }

        #endregion File đính kèm - có lưu pdf temp

        public async Task<List<TT37_PhamViHoatDong>> GetListPhamViToDDL()
        {
            var result =await _phamViHoatDongRepos.GetAll().Where(x => x.IsActive == true).ToListAsync();
            return result;
        }

        public List<ItemObj<int>> GetListVanBangToDDL()
        {
            var _list = new List<ItemObj<int>>();
            foreach (object iEnumItem in Enum.GetValues(typeof(VAN_BANG_CHUYEN_MON)))
            {
                _list.Add(new ItemObj<int>
                {
                    Id = (int)iEnumItem,
                    Name = GetEnumDescription((VAN_BANG_CHUYEN_MON)(int)iEnumItem)
                });
            }

            return _list;
        }
    }

    #endregion MAIN
}