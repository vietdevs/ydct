using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Linq.Extensions;
using newPSG.PMS.Authorization.Users;
using newPSG.PMS.Common;
using newPSG.PMS.Common.Dto;
using newPSG.PMS.Dto;
using newPSG.PMS.EntityDB;

namespace newPSG.PMS.Services
{
    public interface ITieuChiThamDinhAppService : IApplicationService
    {
        Task<PagedResultDto<dynamic>> GetAllServerPaging(TieuChiThamDinhInputDto input);
        Task<dynamic> GetAll();
        Task<dynamic> GetTieuChiThamDinh_Setting(GetTieuChiThamDinhInputDto input);
        Task<dynamic> GetTieuChiThamDinhCustom(TieuChiThamDinhInputDto input);
        Task<dynamic> GetTieuChiThamDinhFormPId(int PId);
        Task<int> CreateOrUpdate(List<TieuChiThamDinhDto> input);
        Task Delete(int id);
        Task DeleteList(List<TieuChiThamDinhDto> input);
    }

    [AbpAuthorize]
    public class TieuChiThamDinhAppService : PMSAppServiceBase, ITieuChiThamDinhAppService
    {
        private readonly IRepository<TieuChiThamDinh> _tieuChiThamDinhRepos;
        private readonly IRepository<LoaiBienBanThamDinh> _loaiBienBanRepos;
        private readonly ICommonLookupAppService _commonLookupService;

        public TieuChiThamDinhAppService(
            IRepository<TieuChiThamDinh> tieuChiThamDinhRepos,
            IRepository<LoaiBienBanThamDinh> loaiBienBanRepos,
            ICommonLookupAppService commonLookupService)
        {
            _tieuChiThamDinhRepos = tieuChiThamDinhRepos;
            _loaiBienBanRepos = loaiBienBanRepos;
            _commonLookupService = commonLookupService;
        }

        public async Task<PagedResultDto<dynamic>> GetAllServerPaging(TieuChiThamDinhInputDto input)
        {
            try
            {
                var query = (from noiDung in _tieuChiThamDinhRepos.GetAll()
                             join t_loaiBienBan in _loaiBienBanRepos.GetAll() on noiDung.LoaiBienBanThamDinhId equals t_loaiBienBan.Id into tb_loaiBienBan //Left Join
                             from loaiBienBan in tb_loaiBienBan.DefaultIfEmpty()
                             select new
                             {
                                 noiDung.Id,
                                 noiDung.PId,
                                 noiDung.Level,
                                 noiDung.ThuTucId,
                                 noiDung.LoaiBienBanThamDinhId,
                                 noiDung.TieuBanEnum,
                                 noiDung.RoleLevel,
                                 noiDung.MaNoiDung,
                                 noiDung.STT,
                                 noiDung.TieuDeThamDinh,
                                 noiDung.IsValidate,
                                 noiDung.IsTieuDe,
                                 StrLoaiBienBanThamDinh = loaiBienBan.Name,
                             })
                             .WhereIf(input.ThuTucId != null, x => x.ThuTucId == input.ThuTucId)
                             .WhereIf(input.RoleLevel != null, x => x.RoleLevel == input.RoleLevel)
                             .WhereIf(input.TieuBanEnum != null, x => x.TieuBanEnum == input.TieuBanEnum)
                             .WhereIf(input.LoaiBienBanThamDinhId != null, x => x.LoaiBienBanThamDinhId == input.LoaiBienBanThamDinhId);

                var queryNoiDung = query.GroupBy(x => new
                {
                    x.ThuTucId,
                    x.RoleLevel,
                    x.LoaiBienBanThamDinhId,
                    x.TieuBanEnum
                }).Select(x => new
                {
                    x.Key.ThuTucId,
                    x.Key.RoleLevel,
                    x.Key.TieuBanEnum,
                    x.Key.LoaiBienBanThamDinhId,
                    x.FirstOrDefault().StrLoaiBienBanThamDinh,
                    ListNoiDung = x.Select(g => new
                    {
                        g.Id,
                        g.PId,
                        g.Level,
                        g.ThuTucId,
                        g.TieuBanEnum,
                        g.RoleLevel,
                        g.LoaiBienBanThamDinhId,
                        g.MaNoiDung,
                        g.STT,
                        g.TieuDeThamDinh,
                        g.IsValidate,
                        g.IsTieuDe,
                    })
                });

                var count = await queryNoiDung.CountAsync();
                var dataGrids = await queryNoiDung
                    .OrderBy(p => p.ThuTucId).ThenBy(x => x.TieuBanEnum)
                    .PageBy(input)
                   .ToListAsync();

                return new PagedResultDto<dynamic>(count, dataGrids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<dynamic> GetAll()
        {
            try
            {
                var query = await _tieuChiThamDinhRepos.GetAllListAsync();
                return query;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<dynamic> GetTieuChiThamDinh_Setting(GetTieuChiThamDinhInputDto input)
        {
            try
            {
                var bienBan = await _tieuChiThamDinhRepos.GetAll()
                      .Where(x => x.LoaiBienBanThamDinhId == input.LoaiBienBanThamDinhId && x.RoleLevel == input.RoleLevel && x.ThuTucId == input.ThuTucId)
                      .WhereIf(input.TieuBanEnum != null && input.RoleLevel == (int)CommonENum.ROLE_LEVEL.CHUYEN_GIA, x => x.TieuBanEnum == input.TieuBanEnum)
                      .Select(x => new
                      {
                          x.Id,
                          TieuChiThamDinhId = x.Id,
                          x.ThuTucId,
                          x.TieuBanEnum,
                          x.LoaiBienBanThamDinhId,
                          x.MaNoiDung,
                          x.STT,
                          x.TieuDeThamDinh,
                          x.IsValidate,
                      })
                      .OrderBy(x => x.STT)
                      .ToListAsync();

                return bienBan;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<dynamic> GetTieuChiThamDinhCustom(TieuChiThamDinhInputDto input)
        {
            try
            {
                var query = (from noiDung in _tieuChiThamDinhRepos.GetAll()
                             select new
                             {
                                 noiDung.Id,
                                 noiDung.PId,
                                 noiDung.Level,
                                 noiDung.ThuTucId,
                                 noiDung.LoaiBienBanThamDinhId,
                                 noiDung.TieuBanEnum,
                                 noiDung.RoleLevel,
                                 noiDung.MaNoiDung,
                                 noiDung.STT,
                                 noiDung.TieuDeThamDinh,
                                 noiDung.IsValidate,
                                 noiDung.IsTieuDe
                             })
                             .WhereIf(input.ThuTucId != null, x => x.ThuTucId == input.ThuTucId)
                             .WhereIf(input.RoleLevel != null, x => x.RoleLevel == input.RoleLevel)
                             .WhereIf(input.TieuBanEnum != null, x => x.TieuBanEnum == input.TieuBanEnum)
                             .WhereIf(input.LoaiBienBanThamDinhId != null, x => x.LoaiBienBanThamDinhId == input.LoaiBienBanThamDinhId);

                var dataGrids = await query
                    .OrderBy(p => p.STT)
                   .ToListAsync();

                return dataGrids;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<dynamic> GetTieuChiThamDinhFormPId(int PId)
        {
            try
            {
                var query = (from noiDung in _tieuChiThamDinhRepos.GetAll()
                             where noiDung.PId == PId
                             select new TieuChiThamDinhDto
                             {
                                 Id = noiDung.Id,
                                 PId = noiDung.PId,
                                 Level = noiDung.Level,
                                 ThuTucId = noiDung.ThuTucId,
                                 LoaiBienBanThamDinhId = noiDung.LoaiBienBanThamDinhId,
                                 TieuBanEnum = noiDung.TieuBanEnum,
                                 RoleLevel = noiDung.RoleLevel,
                                 MaNoiDung = noiDung.MaNoiDung,
                                 STT = noiDung.STT,
                                 TieuDeThamDinh = noiDung.TieuDeThamDinh,
                                 IsValidate = noiDung.IsValidate,
                                 IsTieuDe = noiDung.IsTieuDe
                             });

                var dataGrids = await query.OrderBy(p => p.STT).ToListAsync();

                return dataGrids;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> CreateOrUpdate(List<TieuChiThamDinhDto> input)
        {
            try
            {
                if (input.Count > 0)
                {
                    var inputFirst = input.FirstOrDefault();
                    var listNoiDungDB = _tieuChiThamDinhRepos.GetAll()
                        .Where(x => x.ThuTucId == inputFirst.ThuTucId
                                    && x.RoleLevel == inputFirst.RoleLevel
                                    && x.TieuBanEnum == inputFirst.TieuBanEnum
                                    && x.LoaiBienBanThamDinhId == inputFirst.LoaiBienBanThamDinhId)
                        .WhereIf(inputFirst.Level > 1, x => x.PId == inputFirst.PId && x.Level == inputFirst.Level)
                        .ToList();


                    foreach (var noiDung in input)
                    {
                        if (noiDung.Id > 0)
                        {
                            var noiDungDB = await _tieuChiThamDinhRepos.GetAsync(noiDung.Id);
                            noiDung.MapTo(noiDungDB);
                            noiDung.MaNoiDung = $"TCTD.{noiDung.ThuTucId}.{noiDung.RoleLevel}.{noiDung.TieuBanEnum ?? 0}.{noiDung.LoaiBienBanThamDinhId}.{noiDung.Level}.{noiDungDB.Id}";
                            await _tieuChiThamDinhRepos.UpdateAsync(noiDungDB);
                            listNoiDungDB.Remove(noiDungDB);
                        }
                        else
                        {
                            var insertInput = noiDung.MapTo<TieuChiThamDinh>();
                            insertInput.Id = await _tieuChiThamDinhRepos.InsertAndGetIdAsync(insertInput);
                            insertInput.MaNoiDung = $"TCTD.{noiDung.ThuTucId}.{noiDung.RoleLevel}.{noiDung.TieuBanEnum ?? 0}.{noiDung.LoaiBienBanThamDinhId}.{noiDung.Level}.{insertInput.Id}";
                            await _tieuChiThamDinhRepos.UpdateAsync(insertInput);
                        }
                    }

                    //Xóa những phân từ không nằm trong DS
                    foreach (var item in listNoiDungDB)
                    {
                        await _tieuChiThamDinhRepos.DeleteAsync(item.Id);
                    }

                    return 1;
                }

                return 0;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task Delete(int id)
        {
            await _tieuChiThamDinhRepos.DeleteAsync(id);
        }

        public async Task DeleteList(List<TieuChiThamDinhDto> input)
        {
            foreach (var noiDung in input)
            {
                await _tieuChiThamDinhRepos.DeleteAsync(noiDung.Id);
            }
        }
    }
}