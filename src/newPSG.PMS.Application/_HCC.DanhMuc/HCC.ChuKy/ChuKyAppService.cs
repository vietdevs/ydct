using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.IO;
using Abp.Linq.Extensions;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using Abp.UI;
using newPSG.PMS.Dto;
using newPSG.PMS.EntityDB;
using newPSG.PMS.Storage;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace newPSG.PMS.Services
{
    #region INTERFACE
    public interface IChuKyAppService : IApplicationService
    {
        Task<PagedResultDto<ChuKyDto>> GetAllChuKyServerPagingAsync(ChuKyInputDto input);
        Task<long> CreateOrUpdateChuKyAsync(ChuKyDto input);
        Task DeleteChuKyAsync(long Id);
        Task UpdateChuKyImageUrlAsync(long input, string chuKyImageUrl, string maChuKy);
    }
    #endregion

    #region MAIN
    [AbpAuthorize]
    public class ChuKyAppService : PMSAppServiceBase, IChuKyAppService
    {
        private readonly IRepository<ChuKy, long> _chuKyRepos;
        private readonly IAbpSession _abpSession;
        private readonly ICacheManager _cacheManager;
        private readonly IAppFolders _appFolders;
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly CustomSessionAppSession _mySession;
        private readonly IRepository<DoanhNghiep, long> _doanhNghiepRepos;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public ChuKyAppService(IRepository<ChuKy, long> chuKyRepos,
                               IAbpSession abpSession,
                               IAppFolders appFolders,
                               IBinaryObjectManager binaryObjectManager,
                               ICacheManager cacheManager,
                               CustomSessionAppSession mySession,
                               IRepository<DoanhNghiep, long> doanhNghiepRepos,
                               IUnitOfWorkManager unitOfWorkManager)
        {
            _appFolders = appFolders;
            _chuKyRepos = chuKyRepos;
            _abpSession = abpSession;
            _binaryObjectManager = binaryObjectManager;
            _cacheManager = cacheManager;
            _mySession = mySession;
            _doanhNghiepRepos = doanhNghiepRepos;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public async Task<PagedResultDto<ChuKyDto>> GetAllChuKyServerPagingAsync(ChuKyInputDto input)
        {
            var query = (from chuKy in _chuKyRepos.GetAll()
                         where chuKy.UserId == input.UserId && chuKy.IsActive == true
                         select new ChuKyDto
                         {
                             Id = chuKy.Id,
                             IsActive = chuKy.IsActive,
                             LoaiChuKy = chuKy.LoaiChuKy,
                             MoTa = chuKy.MoTa,
                             MaChuKy = chuKy.MaChuKy,
                             TenChuKy = chuKy.TenChuKy,
                             UrlImage = chuKy.UrlImage,
                             DataImage = chuKy.DataImage,
                             ChanChuKy = chuKy.ChanChuKy,
                             ChieuCao = chuKy.ChieuCao,
                             ChieuRong = chuKy.ChieuRong
                         })
            .WhereIf(!string.IsNullOrEmpty(input.Filter), u => u.TenChuKy.LocDauLowerCaseDB().Contains(input.Filter.Trim().LocDauLowerCaseDB()) || u.MaChuKy.LocDauLowerCaseDB().Contains(input.Filter.Trim().LocDauLowerCaseDB()) || u.MoTa.LocDauLowerCaseDB().Contains(input.Filter.Trim().LocDauLowerCaseDB()))
            .WhereIf(input.LoaiChuKy.HasValue, u => u.LoaiChuKy == input.LoaiChuKy.Value);


            var chuKyCount = await query.CountAsync();
            var dataGrids = await query
                .OrderBy(p => p.Id)
                .PageBy(input)
               .ToListAsync();
            return new PagedResultDto<ChuKyDto>(chuKyCount, dataGrids);
        }

        public async Task<long> CreateOrUpdateChuKyAsync(ChuKyDto input)
        {
            if (input.Id > 0)
            {
                using (var unitOfWork = _unitOfWorkManager.Begin())
                {
                    // update
                    long parentId = input.Id;
                    input.Id = 0;
                    var insertInput = input.MapTo<ChuKy>();
                    insertInput.IsActive = true;
                    insertInput.PId = (input.PId ?? parentId);

                    await _chuKyRepos.InsertAsync(insertInput);

                    InActiveChuKyByIdAsync(parentId, input.IsDaXuLy);
                    await CurrentUnitOfWork.SaveChangesAsync();
                    unitOfWork.Complete();
                    return insertInput.Id;
                }
            }
            else
            {
                try
                {
                    var insertInput = input.MapTo<ChuKy>();
                    await _chuKyRepos.InsertAsync(insertInput);
                    await CurrentUnitOfWork.SaveChangesAsync();
                    return insertInput.Id;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task DeleteChuKyAsync(long id)
        {
            var chuKy = (from chuky in _chuKyRepos.GetAll()
                         where chuky.Id == id || chuky.PId == id
                         select chuky).ToList();
            foreach (var item in chuKy)
            {
                await _chuKyRepos.DeleteAsync(item);
            }
        }

        public async Task UpdateChuKyImageUrlAsync(long input, string fileName, string maChuKy)
        {
            var tempProfilePicturePath = Path.Combine(_appFolders.TempFileDownloadFolder, fileName);

            byte[] byteArray;

            using (var fsTempProfilePicture = new FileStream(tempProfilePicturePath, FileMode.Open))
            {
                using (var bmpImage = new Bitmap(fsTempProfilePicture))
                {
                    using (var stream = new MemoryStream())
                    {
                        bmpImage.Save(stream, bmpImage.RawFormat);
                        stream.Close();
                        byteArray = stream.ToArray();

                        var updateData = await _chuKyRepos.GetAsync(input);
                        string HCC_FILE_PDF = GetUrlFileDefaut();


                        //Tạo thư mục ngoài
                        string parentFolder = "unknown";
                        if (_mySession.UserSession.DoanhNghiepId.HasValue && _mySession.UserSession.DoanhNghiepId != 0)
                        {
                            var doanhnghiep = _doanhNghiepRepos.FirstOrDefault(x => x.Id == _mySession.UserSession.DoanhNghiepId);
                            if (doanhnghiep != null)
                            {
                                parentFolder = !string.IsNullOrEmpty(doanhnghiep.Tinh) ? RemoveUnicode(doanhnghiep.Tinh).ToLower().Trim().Replace(" ", "-") : "unknown";
                            }
                        }
                        else
                        {
                            parentFolder = "_can-bo";
                        }

                        string baseFolder = @"ChuKy\" + parentFolder + @"\" + updateData.UserId;
                        var folderPath = Path.Combine(HCC_FILE_PDF, baseFolder);
                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }
                        var fileInfo = new FileInfo(tempProfilePicturePath);
                        var saveFileName = updateData.UserId + "_" + updateData.Id + "_" + maChuKy + fileInfo.Extension;
                        var saveFileUrl = Path.Combine(baseFolder, saveFileName);
                        var saveFilePath = Path.Combine(folderPath, saveFileName);
                        bmpImage.Save(saveFilePath);

                        updateData.DataImage = byteArray;
                        updateData.UrlImage = saveFileUrl;
                        await _chuKyRepos.UpdateAsync(updateData);
                    }
                }
            }

            FileHelper.DeleteIfExists(tempProfilePicturePath);
        }

        #region Private Functions

        private void InActiveChuKyByIdAsync(long id, bool? IsDaXuLy)
        {
            var chuKy = (from chuky in _chuKyRepos.GetAll()
                         where chuky.Id == id
                         select chuky).FirstOrDefault();
            if (chuKy != null)
            {
                chuKy.IsActive = false;
                chuKy.IsDaXuLy = IsDaXuLy;
            }
            _chuKyRepos.UpdateAsync(chuKy);
        }
        #endregion

    }
    #endregion
}
