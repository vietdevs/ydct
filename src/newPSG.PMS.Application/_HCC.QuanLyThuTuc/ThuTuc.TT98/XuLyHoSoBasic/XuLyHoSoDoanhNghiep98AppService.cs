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

using XHoSo = newPSG.PMS.EntityDB.HoSo98;
using XHoSoTepDinhKem = newPSG.PMS.EntityDB.HoSoTepDinhKem98;
using XHoSoXuLy = newPSG.PMS.EntityDB.HoSoXuLy98;
using XHoSoXuLyHistory = newPSG.PMS.EntityDB.HoSoXuLyHistory98;
using XHoSoDto = newPSG.PMS.Dto.HoSo98Dto;
using System.IO;
using System.Configuration;

#endregion Class Riêng Cho Từng Thủ tục

namespace newPSG.PMS.Services
{
    #region INTERFACE

    public interface IXuLyHoSoDoanhNghiep98AppService : IApplicationService
    {
        #region Public Common

        int GetPhongBanIdXuLyHoSo(XHoSo hoSo);

        #endregion Public Common

        #region Thêm & Sửa & Hủy Hồ Sơ

        Task<dynamic> GetHoSoById(long hoSoId);

        Task<long> CreateOrUpdateHoSo(CreateOrUpdateHoSo98Input input);

        Task OpenHuyHoSo(long hoSoId);

        Task TaoBanSaoHoSo(long hoSoId);

        #endregion Thêm & Sửa & Hủy Hồ Sơ

        #region Ký số hồ sơ

        Task<int> UpdateKySoHoSo(UpdateKySo98InputDto input);

        #endregion Ký số hồ sơ

        #region Thanh toán Keypay

        Task<object> ChuyenThanhToanKeyPay(KyVaThanhToan98InputDto input);

        Task<int> UpdateThanhToanKeyPay(Keypay input, bool? isIPN);

        #endregion Thanh toán Keypay

        #region Thanh toán chuyển khoản

        Task<long> ThanhToanChuyenKhoan(ThanhToanChuyenKhoan98InputDto input);

        #endregion Thanh toán chuyển khoản

        #region Nộp hồ sơ để rà soát

        Task NopHoSoDeRaSoat(long HoSoId);

        Task NopHoSoTraLai(long hoSoId);

        #endregion Nộp hồ sơ để rà soát

        #region Nộp hồ sơ mới

        Task<object> ChuyenHoSoDaCoThanhToanBangTay(long hoSoId);

        Task<object> ChuyenHoSoKhongCanThanhToan(long hoSoId);

        Task NopHoSoMoi(XHoSo hoso, bool isAdmin = false, bool isKeToan = false);

        Task NopHoSoMoi(long hoSoId, bool isAdmin = false, bool isKeToan = false);

        #endregion Nộp hồ sơ mới

        #region Nộp hồ sơ bổ sung

        Task NopHoSoBoSung(long hoSoId);

        #endregion Nộp hồ sơ bổ sung

        #region File đính kèm

        List<HoSoTepDinhKem98Dto> GetHoSoTepDinhKem(long hoSoId);

        bool CopyFileTaiLieuDaTaiVaoHoSo(CopyFileTaiLieuDaTaiVaoHoSo98Input input);

        #endregion File đính kèm
    }

    #endregion INTERFACE

    #region MAIN

    [AbpAuthorize]
    public class XuLyHoSoDoanhNghiep98AppService : PMSAppServiceBase, IXuLyHoSoDoanhNghiep98AppService
    {
        private readonly int _thuTucId = (int)CommonENum.THU_TUC_ID.THU_TUC_98;
        private readonly EditionManager _editionManager;
        private readonly IRepository<DoanhNghiep, long> _doanhNghiepRepos;
        private readonly IRepository<XHoSo, long> _hoSoRepos;
        private readonly IRepository<XHoSoTepDinhKem, long> _hoSoTepDinhKemRepos;
        private readonly IRepository<XHoSoXuLy, long> _hoSoXuLyRepos;
        private readonly IRepository<XHoSoXuLyHistory, long> _hoSoXuLyHistoryRepos;
        private readonly IRepository<LoaiHoSo> _loaiHoSoRepos;
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

        public XuLyHoSoDoanhNghiep98AppService(
            EditionManager editionManager,
            IRepository<DoanhNghiep, long> doanhNghiepRepos,
            IRepository<XHoSo, long> hoSoRepos,
            IRepository<XHoSoTepDinhKem, long> hoSoTepDinhKemRepos,
            IRepository<XHoSoXuLy, long> hoSoXuLyRepos,
            IRepository<XHoSoXuLyHistory, long> hoSoXuLyHistoryRepos,
            IRepository<LoaiHoSo> loaiHoSoRepos,
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
                        return new
                        {
                            Status = true,
                            HoSo = hoSo,
                            Teps = await _queryTep.ToListAsync()
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

        public async Task<long> CreateOrUpdateHoSo(CreateOrUpdateHoSo98Input input)
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
                        upObj.ThuTucId = (int)CommonENum.THU_TUC_ID.THU_TUC_98;
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

        private async Task<long> UpdateHoSo(CreateOrUpdateHoSo98Input input)
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
                        insertTailieu.ThuTucId = (int)CommonENum.THU_TUC_ID.THU_TUC_98;
                        insertTailieu.HoSoId = updateData.Id;
                        if (!String.IsNullOrEmpty(tailieu.DuongDanTep))
                        {
                            insertTailieu.Id = 0;
                            insertTailieu.DaTaiLen = true;
                            insertTailieu.IsActive = true;
                            insertTailieu.TenTep = tailieu.TenTep;
                            insertTailieu.DuongDanTep = tailieu.DuongDanTep;
                        }
                        await _hoSoTepDinhKemRepos.InsertAsync(insertTailieu);
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
                        updateData.SoDangKy = _capSoThuTucAppService.SinhSoDangKy(updateData.DoanhNghiepId.Value, (int)CommonENum.THU_TUC_ID.THU_TUC_98);
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

        private async Task<long> CreateHoSo(CreateOrUpdateHoSo98Input input)
        {
            try
            {
                var insertInput = input.HoSo.MapTo<XHoSo>();
                if (insertInput.LoaiHoSoId.HasValue)
                {
                    var lhs = await _loaiHoSoRepos.GetAsync(insertInput.LoaiHoSoId.Value);
                    insertInput.TenLoaiHoSo = lhs.TenLoaiHoSo;
                }
                insertInput.SoDangKy = _capSoThuTucAppService.SinhSoDangKy(input.HoSo.DoanhNghiepId.Value, (int)CommonENum.THU_TUC_ID.THU_TUC_98);
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
                        insertTailieu.ThuTucId = (int)CommonENum.THU_TUC_ID.THU_TUC_98;
                        insertTailieu.HoSoId = id;
                        if (!String.IsNullOrEmpty(tailieu.DuongDanTep))
                        {
                            insertTailieu.DaTaiLen = true;
                            insertTailieu.IsActive = true;
                            insertTailieu.TenTep = tailieu.TenTep;
                            insertTailieu.DuongDanTep = tailieu.DuongDanTep;
                        }
                        _hoSoTepDinhKemRepos.Insert(insertTailieu);
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

        #region Ký số hồ sơ

        public async Task<int> UpdateKySoHoSo(UpdateKySo98InputDto input)
        {
            try
            {
                if (input.HoSoId > 0)
                {
                    var hoSo = await _hoSoRepos.GetAsync(input.HoSoId);
                    if (hoSo != null)
                    {
                        hoSo.IsCA = true;
                        hoSo.DuongDanTepCA = input.DuongDanTep;
                        //Update
                        await _hoSoRepos.UpdateAsync(hoSo);
                    }
                }

                return 1;
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                return 0;
            }
        }

        #endregion Ký số hồ sơ

        #region Thanh toán Keypay

        public async Task<object> ChuyenThanhToanKeyPay(KyVaThanhToan98InputDto input)
        {
            try
            {
                var hoSo = await _hoSoRepos.GetAsync(input.HoSoId);
                if (hoSo == null)
                {
                    return new
                    {
                        StatusCode = TRANG_THAI_GIAO_DICH.DU_LIEU_DAU_VAO_LOI,
                        Description = "Hồ sơ không tồn tại"
                    };
                }

                if (hoSo.TrangThaiHoSo.Value == (int)CommonENum.TRANG_THAI_HO_SO.CHO_DOANH_NGHIEP_THANH_TOAN)
                {
                    decimal tongPhiXuLy = 0;
                    var loaiHoSo = await _loaiHoSoRepos.FirstOrDefaultAsync(x => x.Id == hoSo.LoaiHoSoId);
                    if (loaiHoSo == null)
                    {
                        return new
                        {
                            StatusCode = TRANG_THAI_GIAO_DICH.DU_LIEU_DAU_VAO_LOI,
                            Description = "Loại hồ sơ không tồn tại"
                        };
                    }

                    //Mặc định tenantId của cục
                    int tenantId = GetTenantIdFromHoSo(hoSo);
                    tongPhiXuLy = loaiHoSo.PhiXuLy.HasValue ? loaiHoSo.PhiXuLy.Value : 0;

                    var thanhtoan = new ThanhToan();
                    thanhtoan.HoSoId = hoSo.Id;
                    thanhtoan.DoanhNghiepId = hoSo.DoanhNghiepId;
                    thanhtoan.PhanHeId = (int)CommonENum.THU_TUC_ID.THU_TUC_98;

                    thanhtoan.PhiDaNop = tongPhiXuLy;

                    thanhtoan.KenhThanhToan = (int)CommonENum.KENH_THANH_TOAN.KENH_KEYPAY;
                    thanhtoan.NgayGiaoDich = DateTime.Now;
                    thanhtoan.TrangThaiNganHang = (int)TRANG_THAI_GIAO_DICH.BAT_DAU_GIAO_DICH;
                    thanhtoan.TrangThaiKeToan = (int)TRANG_THAI_KE_TOAN.KE_TOAN_CHUA_XAC_NHAN;
                    thanhtoan.TenantId = tenantId;
                    thanhtoan.MaDonHang = CommonENum.GetEnumDescription(THU_TUC_ID.THU_TUC_98) + ".KP." + hoSo.Id;
                    thanhtoan.MaGiaoDich = await _thanhToanKeyPayAppService.GetMERCHANT_TRANS_ID_FromSettings();
                    thanhtoan.GhiChu = $"Doanh nghiệp {hoSo.TenDoanhNghiep} - [{hoSo.MaSoThue}] thanh toán phí đăng ký cơ sở đủ điều kiện cho cơ sở: {hoSo.TenCoSo}";
                    long phiDaNop = (long)thanhtoan.PhiDaNop;
                    Keypay _keypay = await _thanhToanKeyPayAppService.GetUrlSendToKeypayMD5(tenantId, thanhtoan.MaGiaoDich, thanhtoan.MaDonHang, phiDaNop, thanhtoan.GhiChu);
                    string urlKeypay = (_keypay != null) ? _keypay.buildPayUrl() : "";
                    thanhtoan.RequestJson = _keypay == null ? null : _keypay.RequestJson;

                    var thanhToanId = await _thanhToanRepos.InsertAndGetIdAsync(thanhtoan);

                    hoSo.ThanhToanId_Active = thanhToanId;
                    hoSo.NgayThanhToan = DateTime.Now;
                    await _hoSoRepos.UpdateAsync(hoSo);

                    #region Lưu lịch sử

                    var _history = new XHoSoXuLyHistory();
                    _history.HoSoId = hoSo.Id;
                    _history.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.DOANH_NGHIEP;
                    _history.NguoiXuLyId = _session.UserId;
                    _history.ActionEnum = (int)CommonENum.FORM_FUNCTION.DOANH_NGHIEP_THANH_TOAN;
                    _history.NgayXuLy = DateTime.Now;
                    await _hoSoXuLyHistoryRepos.InsertOrUpdateAndGetIdAsync(_history);

                    #endregion Lưu lịch sử

                    return new
                    {
                        StatusCode = TRANG_THAI_GIAO_DICH.BAT_DAU_GIAO_DICH,
                        Description = "Bắt đầu giao dịch với mã [" + thanhtoan.MaDonHang + "]",
                        Url = urlKeypay
                    };
                }
                else
                {
                    if (hoSo.TrangThaiHoSo.Value == (int)CommonENum.TRANG_THAI_HO_SO.CHO_KE_TOAN_XAC_NHAN)
                    {
                        return new
                        {
                            StatusCode = TRANG_THAI_GIAO_DICH.GIAO_DICH_DANG_CHO_KET_QUA,
                            Description = "Hồ sơ đang chờ xác nhận thanh toán"
                        };
                    }
                    return new
                    {
                        StatusCode = TRANG_THAI_GIAO_DICH.GIAO_DICH_THANH_CONG,
                        Description = "Hồ sơ đã thanh toán và đang được xử lý."
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [UnitOfWork(IsDisabled = true)] //Tắt tính năng AutoSaveChange() của Mr.Zezo
        public async Task<int> UpdateThanhToanKeyPay(Keypay input, bool? isIPN = false)
        {
            if (!string.IsNullOrEmpty(input.Good_code) && input.Good_code.Contains("."))
            {
                var maDonHangSplit = input.Good_code.Split('.');
                long hoSoId = 0;
                long.TryParse(maDonHangSplit.Last(), out hoSoId);
                if (hoSoId > 0)
                {
                    var hoSo = _hoSoRepos.Get(hoSoId);
                    if (hoSo != null)
                    {
                        int tenantId = GetTenantIdFromHoSo(hoSo);

                        var merchantSecure_Hash = await _thanhToanKeyPayAppService.GetMerchantSecure_Hash_MD5_For_TennantId(tenantId, input);
                        if (merchantSecure_Hash != input.Secure_hash)
                        {
                            return 0;
                        }


                        var listThanhToan = from t in _thanhToanRepos.GetAll()
                                            where t.KenhThanhToan == (int)CommonENum.KENH_THANH_TOAN.KENH_KEYPAY
                                            && t.PhanHeId == (int)CommonENum.THU_TUC_ID.THU_TUC_98
                                            && t.HoSoId == hoSo.Id
                                            select t;
                        //Cập nhật thanh toán
                        bool isDaThanhToanThanhCong = false;
                        if (hoSo.ThanhToanId_Active.HasValue)//Check Đã tồn tại hồ sơ thanh toàn thành công
                        {
                            var thanhToanActive = _thanhToanRepos.Get(hoSo.ThanhToanId_Active.Value);
                            if (thanhToanActive != null && thanhToanActive.TrangThaiNganHang == (int)CommonENum.TRANG_THAI_GIAO_DICH.GIAO_DICH_THANH_CONG)
                                isDaThanhToanThanhCong = true;
                        }
                        var thanhtoan = listThanhToan.Where(x => x.MaGiaoDich == input.Merchant_trans_id).FirstOrDefault();

                        if (thanhtoan == null)
                        {
                            decimal phi;
                            decimal.TryParse(input.Nest_code, out phi);

                            thanhtoan = new ThanhToan();
                            thanhtoan.HoSoId = hoSo.Id;
                            thanhtoan.DoanhNghiepId = hoSo.DoanhNghiepId;
                            thanhtoan.PhanHeId = (int)CommonENum.THU_TUC_ID.THU_TUC_98;
                            thanhtoan.GhiChu = "{'MST': '" + hoSo.MaSoThue + "','HoSoId':" + hoSo.Id + "}";
                            thanhtoan.PhiDaNop = phi;

                            //thanhtoan.MaDonHang (res)
                            thanhtoan.MaDonHang = input.Good_code;

                            //thanhtoan.MaGiaoDich (res)
                            thanhtoan.MaGiaoDich = input.Trans_id;

                            thanhtoan.KenhThanhToan = (int)CommonENum.KENH_THANH_TOAN.KENH_KEYPAY;
                            thanhtoan.NgayGiaoDich = DateTime.Now;
                            thanhtoan.TrangThaiNganHang = (int)TRANG_THAI_GIAO_DICH.BAT_DAU_GIAO_DICH;
                            thanhtoan.TrangThaiKeToan = (int)TRANG_THAI_KE_TOAN.KE_TOAN_CHUA_XAC_NHAN;
                            thanhtoan.TenantId = tenantId;
                        }
                        var ketQuaGiaoDich = new
                        {
                            input.Merchant_trans_id,
                            input.Good_code,
                            input.Trans_id,
                            input.Merchant_code,
                            input.Bank_code,
                            input.Command,
                            input.Nest_code,
                            input.Response_code,
                            input.Service_code,
                            input.Ship_fee,
                            input.Tax,
                            input.Current_code,
                            input.Secure_hash
                        };

                        thanhtoan.KetQuaGiaoDichJson = JsonConvert.SerializeObject(ketQuaGiaoDich);

                        if (input.Response_code == CommonENum.KEYPAY_RESPONSE.THANH_CONG)// giao dich thanh cong
                        {
                            decimal phiXacNhan = 0;
                            decimal.TryParse(input.Nest_code, out phiXacNhan);
                            thanhtoan.PhiXacNhan = phiXacNhan;
                            thanhtoan.TrangThaiNganHang = (int)CommonENum.TRANG_THAI_GIAO_DICH.GIAO_DICH_THANH_CONG;
                        }
                        else
                        {
                            thanhtoan.TrangThaiNganHang = (int)CommonENum.TRANG_THAI_GIAO_DICH.GIAO_DICH_KHONG_THANH_CONG;

                            hoSo.TrangThaiHoSo = (int)CommonENum.TRANG_THAI_HO_SO.THANH_TOAN_THAT_BAI;
                        }

                        if (isIPN == true)
                        {
                            thanhtoan.SoTaiKhoanNop = "{IsIPN:true}";
                        }

                        long thanhToanId = _thanhToanRepos.InsertOrUpdateAndGetId(thanhtoan);

                        //Nộp hồ sơ
                        if (input.Response_code == CommonENum.KEYPAY_RESPONSE.THANH_CONG)
                        {
                            if (isDaThanhToanThanhCong==false)//Check trong trường hợp KeyPay Call function này lần nữa mà đã có giao dịch thành công
                            {
                                hoSo.ThanhToanId_Active = thanhToanId;
                                hoSo.NgayXacNhanThanhToan = DateTime.Now;
                                await NopHoSoMoi(hoSo);
                            }
                            await _thanhToanKeyPayAppService.ConfirmTransResult(tenantId, input.Merchant_trans_id, input.Good_code, input.Trans_id, "0");
                        }
                        else
                        {
                            await _hoSoRepos.UpdateAsync(hoSo);
                        }
                        
                        return 1;
                    }
                }
            }
            return 0;
        }

        #endregion Thanh toán Keypay

        #region Thanh toán chuyển khoản

        public async Task<long> ThanhToanChuyenKhoan(ThanhToanChuyenKhoan98InputDto input)
        {
            try
            {
                long idThanhToan = 0;
                var hoSo = _hoSoRepos.Get(input.HoSoId);
                if (hoSo != null)
                {
                    //Mặc định tenantId của cục
                    int tenantId = GetTenantIdFromHoSo(hoSo);

                    ThanhToan thanhtoan = new ThanhToan();
                    thanhtoan.HoSoId = hoSo.Id;
                    thanhtoan.DoanhNghiepId = hoSo.DoanhNghiepId;
                    thanhtoan.GhiChu = input.GhiChu;
                    thanhtoan.PhiDaNop = input.PhiDaNop;
                    thanhtoan.SoTaiKhoanNop = input.SoTaiKhoanNop;
                    thanhtoan.SoTaiKhoanHuong = input.SoTaiKhoanHuong;
                    thanhtoan.MaGiaoDich = input.MaGiaoDich;
                    thanhtoan.MaDonHang = input.MaDonHang;
                    thanhtoan.NgayGiaoDich = DateTime.Now;
                    thanhtoan.DuongDanHoaDonThanhToan = input.DuongDanHoaDonThanhToan;
                    thanhtoan.KenhThanhToan = (int)KENH_THANH_TOAN.HINH_THUC_CHUYEN_KHOAN;
                    thanhtoan.TrangThaiNganHang = (int)TRANG_THAI_GIAO_DICH.GIAO_DICH_DANG_CHO_KET_QUA;
                    thanhtoan.TrangThaiKeToan = (int)TRANG_THAI_KE_TOAN.KE_TOAN_CHUA_XAC_NHAN;
                    thanhtoan.TenantId = tenantId;
                    thanhtoan.PhanHeId = _thuTucId;
                    idThanhToan = _thanhToanRepos.InsertAndGetId(thanhtoan);

                    hoSo.TrangThaiHoSo = (int)TRANG_THAI_HO_SO.CHO_KE_TOAN_XAC_NHAN;
                    hoSo.ThanhToanId_Active = idThanhToan;
                    hoSo.NgayThanhToan = DateTime.Now;
                    await _hoSoRepos.UpdateAsync(hoSo);

                    var _history = new XHoSoXuLyHistory();
                    _history.HoSoId = hoSo.Id;
                    _history.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.DOANH_NGHIEP;
                    _history.NguoiXuLyId = _session.UserId;
                    _history.ActionEnum = (int)CommonENum.FORM_FUNCTION.DOANH_NGHIEP_THANH_TOAN;
                    await _hoSoXuLyHistoryRepos.InsertOrUpdateAndGetIdAsync(_history);
                }
                return idThanhToan;
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                return 0;
            }
        }

        #endregion Thanh toán chuyển khoản

        #region Nộp hồ sơ để rà soát

        public async Task NopHoSoDeRaSoat(long HoSoId)
        {
            try
            {
                using (var unitOfWork = _unitOfWorkManager.Begin())
                {
                    var hoso = _hoSoRepos.Get(HoSoId);

                    var hsxl = new XHoSoXuLy();

                    if (hoso.HoSoXuLyId_Active.HasValue)
                    {
                        hsxl = _hoSoXuLyRepos.Get(hoso.HoSoXuLyId_Active.Value);
                    }
                    hsxl.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.VAN_THU;
                    hsxl.DonViGui = (int)CommonENum.DON_VI_XU_LY.DOANH_NGHIEP;
                    hsxl.NguoiGuiId = _session.UserId;
                    hsxl.HoSoId = HoSoId;
                    hsxl.NgayGui = DateTime.Now;
                    hsxl.NgayTiepNhan = DateTime.Now;
                    var _hsxlId = await _hoSoXuLyRepos.InsertOrUpdateAndGetIdAsync(hsxl);

                    //Thêm History
                    var history = new XHoSoXuLyHistory();
                    history.HoSoId = HoSoId;
                    history.ThuTucId = hoso.ThuTucId;
                    history.NguoiXuLyId = _session.UserId;
                    history.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.DOANH_NGHIEP;
                    history.ActionEnum = (int)CommonENum.FORM_FUNCTION.DOANH_NGHIEP_NOP_HO_SO;
                    await _hoSoXuLyHistoryRepos.InsertOrUpdateAndGetIdAsync(history);

                    //Update trang thai ho so doanh nghiep
                    hoso.TrangThaiHoSo = (int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_HO_SO_MOI;
                    hoso.HoSoXuLyId_Active = _hsxlId;
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

        #region Nộp hồ sơ mới

        public async Task<object> ChuyenHoSoDaCoThanhToanBangTay(long hoSoId)
        {
            var hoSo = await _hoSoRepos.GetAsync(hoSoId);
            if (hoSo != null && hoSo.ThanhToanId_Active.HasValue && hoSo.ThanhToanId_Active.Value > 0)
            {
                var ttFix = _thanhToanRepos.Get(hoSo.ThanhToanId_Active.Value);
                if (ttFix != null)
                {
                    ttFix.TrangThaiNganHang = (int)CommonENum.TRANG_THAI_GIAO_DICH.GIAO_DICH_THANH_CONG;
                    ttFix.SoTaiKhoanNop = "{CheckThanhToanBangTay: true}";
                    await _thanhToanRepos.UpdateAsync(ttFix);
                    if (hoSo.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.CHO_KE_TOAN_XAC_NHAN)
                    {
                        await NopHoSoMoi(hoSo, true, true);
                    }
                    else if (hoSo.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.DA_LUU || hoSo.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.THANH_TOAN_THAT_BAI)
                    {
                        await NopHoSoMoi(hoSo, true, false);
                    }
                }
            }
            else
            {
                var thanhtoan = (
                from t in _thanhToanRepos.GetAll()
                where t.HoSoId == hoSo.Id
                select t).FirstOrDefault();

                if (thanhtoan != null)
                {
                    thanhtoan.TrangThaiNganHang = (int)CommonENum.TRANG_THAI_GIAO_DICH.GIAO_DICH_THANH_CONG;
                    thanhtoan.SoTaiKhoanNop = "{CheckThanhToanBangTay: true}";
                    await _thanhToanRepos.UpdateAsync(thanhtoan);
                    await NopHoSoMoi(hoSo);
                }
            }

            return new
            {
                StatusCode = TRANG_THAI_GIAO_DICH.GIAO_DICH_THANH_CONG,
                Description = "Hồ sơ đã được nộp bằng tay tới cục"
            };
        }

        public async Task<object> ChuyenHoSoKhongCanThanhToan(long hoSoId)
        {
            var hoSo = await _hoSoRepos.GetAsync(hoSoId);
            if (hoSo != null)
            {
                await NopHoSoMoi(hoSo, false, false);
            }

            return new
            {
                StatusCode = TRANG_THAI_GIAO_DICH.GIAO_DICH_THANH_CONG,
                Description = "Hồ sơ đã được nộp bằng tay tới cục"
            };
        }

        public async Task NopHoSoMoi(XHoSo hoSo, bool isAdmin = false, bool isKeToan = false)
        {
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {

                XHoSoXuLy hoSoXuLy = new XHoSoXuLy();
                if (hoSo.HoSoXuLyId_Active.HasValue && hoSo.HoSoXuLyId_Active.Value > 0)
                {
                    hoSoXuLy = _hoSoXuLyRepos.Get(hoSo.HoSoXuLyId_Active.Value);
                }
                hoSoXuLy.ThuTucId = hoSo.ThuTucId;
                hoSoXuLy.HoSoId = hoSo.Id;
                hoSoXuLy.LoaiHoSoId = hoSo.LoaiHoSoId;
                hoSoXuLy.IsHoSoBS = false;
                hoSoXuLy.NgayTiepNhan = DateTime.Now;
                hoSoXuLy.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.MOT_CUA_TIEP_NHAN;
                hoSoXuLy.DonViGui = (int)CommonENum.DON_VI_XU_LY.DOANH_NGHIEP;

                if (isKeToan)
                {
                    hoSoXuLy.DonViGui = (int)CommonENum.DON_VI_XU_LY.KE_TOAN;
                }

                #region Tính Ngày hẹn trả

                hoSoXuLy.LoaiHoSoId = hoSo.LoaiHoSoId;
                var loaiHoSo = await _loaiHoSoRepos.FirstOrDefaultAsync(x => x.Id == hoSo.LoaiHoSoId);
                if (loaiHoSo != null && loaiHoSo.SoNgayXuLy.HasValue)
                {
                    DateTime ngayHenTra = _lichLamViecAppService.GetNgayHenTra(hoSoXuLy.NgayTiepNhan.Value, loaiHoSo.SoNgayXuLy.Value);
                    hoSoXuLy.NgayHenTra = ngayHenTra;
                }

                #endregion Tính Ngày hẹn trả

                #region Lưu lịch sử

                var _history = new XHoSoXuLyHistory();
                _history.ThuTucId = hoSo.ThuTucId;
                _history.HoSoXuLyId = hoSoXuLy.Id;
                _history.HoSoId = hoSoXuLy.HoSoId;
                _history.NguoiXuLyId = _session.UserId;
                _history.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.DOANH_NGHIEP;
                _history.ActionEnum = (int)CommonENum.FORM_FUNCTION.DOANH_NGHIEP_THANH_TOAN;
                if (isKeToan)
                {
                    _history.DonViXuLy = (int)CommonENum.DON_VI_XU_LY.KE_TOAN;
                    _history.ActionEnum = (int)CommonENum.FORM_FUNCTION.KE_TOAN_XAC_NHAN_THANH_TOAN;
                }
                _history.NgayXuLy = DateTime.Now;
                var _historyId = _hoSoXuLyHistoryRepos.InsertOrUpdateAndGetId(_history);
                hoSoXuLy.HoSoXuLyHistoryId_Active = null;//reset

                #endregion Lưu lịch sử

                if (isAdmin)
                {
                    if (hoSo.DoanhNghiepId.HasValue)
                    {
                        var userDN = await _userRepos.GetAll().Where(x => x.DoanhNghiepId == hoSo.DoanhNghiepId).FirstOrDefaultAsync();
                        if (userDN != null)
                        {
                            hoSoXuLy.NguoiGuiId = userDN.Id;
                        }
                    }
                }
                else
                {
                    hoSoXuLy.NguoiGuiId = _session.UserId;
                }
                hoSoXuLy.NguoiXuLyId = null;
                var idTiepNhan = _hoSoXuLyRepos.InsertOrUpdateAndGetId(hoSoXuLy);
                hoSo.TrangThaiHoSo = (int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_DA_TIEP_NHAN;
                hoSo.HoSoXuLyId_Active = idTiepNhan;
                hoSo.NgayTiepNhan = DateTime.Now;
                _hoSoRepos.Update(hoSo);
                unitOfWork.Complete();
            }
        }

        public async Task NopHoSoMoi(long hoSoId, bool isAdmin = false, bool isKeToan = false)
        {
            var hoSo = _hoSoRepos.Get(hoSoId);
            if (hoSo != null && (hoSo.TrangThaiHoSo == null || hoSo.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.DA_LUU))
            {
                await NopHoSoMoi(hoSo, isAdmin, isKeToan);
            }
        }

        #endregion Nộp hồ sơ mới

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
                        _history.DonViKeTiep = (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET;
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

        public bool CopyFileTaiLieuDaTaiVaoHoSo(CopyFileTaiLieuDaTaiVaoHoSo98Input input)
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
                            var insertHoSoTep = new HoSoTepDinhKem98()
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
                return $@"HoSo-TT98\\{strTinh}\\{maSoThue}\\{ DateTime.Now.Year.ToString()}";
            }
            return $@"HoSo-TT98\\{strTinh}\\{maSoThue}\\{ DateTime.Now.Year.ToString()}\\{hoSoIdFolder}";
        }

        public List<HoSoTepDinhKem98Dto> GetHoSoTepDinhKem(long hoSoId)
        {
            var query = from tep in _hoSoTepDinhKemRepos.GetAll()
                        where tep.HoSoId == hoSoId
                        where tep.IsActive == true
                        select new HoSoTepDinhKem98Dto
                        {
                            HoSoId = tep.HoSoId,
                            DuongDanTep = tep.DuongDanTep,
                            TenTep = tep.TenTep,
                            MoTaTep = tep.MoTaTep,
                        };
            return query.ToList();
        }

        #endregion File đính kèm - có lưu pdf temp
    }

    #endregion MAIN
}