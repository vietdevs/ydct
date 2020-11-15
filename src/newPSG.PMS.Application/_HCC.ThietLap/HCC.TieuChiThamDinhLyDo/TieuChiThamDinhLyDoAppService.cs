using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Auditing;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Extensions;
using Abp.UI;
using newPSG.PMS.Authorization.Users;
using newPSG.PMS.Common.Dto;
using newPSG.PMS.Dto;
using newPSG.PMS.EntityDB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace newPSG.PMS.Service
{
    public interface ITieuChiThamDinhLyDoAppService : IApplicationService
    {
        Task<PagedResultDto<dynamic>> GetAllServerPaging(TieuChiThamDinh_LyDoDtoInput input);
        Task<int> CreateOrUpdate(List<TieuChiThamDinh_LyDoDto> input);
        Task Delete(int tieuChiThamDinhId);
    }

    public class TieuChiThamDinhLyDoAppService : PMSAppServiceBase, ITieuChiThamDinhLyDoAppService
    {
        private readonly IRepository<TieuChiThamDinh_LyDo> _tieuChiThamDinh_LyDoRepos;
        private readonly IRepository<TieuChiThamDinh> _tieuChiThamDinhRepos;
        private readonly IRepository<User, long> _userRepos;

        public TieuChiThamDinhLyDoAppService(
            IRepository<TieuChiThamDinh_LyDo> TieuChiThamDinh_LyDoRepos,
            IRepository<TieuChiThamDinh> tieuChiThamDinhRepos,
            IRepository<User, long> userRepos)
        {
            _tieuChiThamDinh_LyDoRepos = TieuChiThamDinh_LyDoRepos;
            _tieuChiThamDinhRepos = tieuChiThamDinhRepos;
            _userRepos = userRepos;
        }


        public async Task<PagedResultDto<dynamic>> GetAllServerPaging(TieuChiThamDinh_LyDoDtoInput input)
        {
            var query = (from data in _tieuChiThamDinh_LyDoRepos.GetAll()
                         join r_nd in _tieuChiThamDinhRepos.GetAll() on data.TieuChiThamDinhId equals r_nd.Id into tb_nd
                         from noiDung in tb_nd.DefaultIfEmpty()
                         select new
                         {
                             noiDung.TieuDeThamDinh,
                             TieuChiThamDinhId = noiDung.Id,
                             noiDung.ThuTucId,
                             noiDung.RoleLevel,
                             noiDung.TieuBanEnum,

                             data.Id,
                             data.MaLyDo,
                             data.LyDo
                         });

            var queryGrpup = query.GroupBy(x => x.TieuChiThamDinhId)
                .Select(x => new
                {
                    TieuChiThamDinhId = x.Key,
                    x.FirstOrDefault().TieuDeThamDinh,
                    x.FirstOrDefault().ThuTucId,
                    x.FirstOrDefault().RoleLevel,
                    x.FirstOrDefault().TieuBanEnum,
                    ListLyDo = x.Select(a => new
                    {
                        a.Id,
                        a.MaLyDo,
                        a.LyDo
                    })
                });

            var count = await queryGrpup.CountAsync();
            var dataGrids = await queryGrpup
                .OrderByDescending(p => p.ThuTucId).ThenBy(x => x.TieuDeThamDinh)
                .PageBy(input)
               .ToListAsync();

            return new PagedResultDto<dynamic>(count, dataGrids);
        }

        public async Task<int> CreateOrUpdate(List<TieuChiThamDinh_LyDoDto> input)
        {
            try
            {
                if (input.Count > 0)
                {
                    var inputFirst = input.FirstOrDefault();
                    var listLyDo = _tieuChiThamDinh_LyDoRepos.GetAll()
                        .Where(x => x.TieuChiThamDinhId == inputFirst.TieuChiThamDinhId)
                        .ToList();


                    foreach (var lyDo in input)
                    {
                        if (lyDo.Id > 0)
                        {
                            var noiDungDB = await _tieuChiThamDinh_LyDoRepos.GetAsync(lyDo.Id);
                            lyDo.MapTo(noiDungDB);
                            noiDungDB.MaLyDo = $"TCTDLD.{lyDo.TieuChiThamDinhId}.{noiDungDB.Id}";
                            await _tieuChiThamDinh_LyDoRepos.UpdateAsync(noiDungDB);
                            listLyDo.Remove(noiDungDB);
                        }
                        else
                        {
                            var insertInput = lyDo.MapTo<TieuChiThamDinh_LyDo>();
                            insertInput.Id = await _tieuChiThamDinh_LyDoRepos.InsertAndGetIdAsync(insertInput);
                            insertInput.MaLyDo = $"TCTDLD.{lyDo.TieuChiThamDinhId}.{insertInput.Id}";
                            await _tieuChiThamDinh_LyDoRepos.UpdateAsync(insertInput);
                        }
                    }

                    //Xóa những phân từ không nằm trong DS
                    foreach (var item in listLyDo)
                    {
                        await _tieuChiThamDinh_LyDoRepos.DeleteAsync(item.Id);
                    }

                    return 1;
                }

                return 0;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Xảy ra lỗi trong quá trình xử lý", ex.Message);
            }
        }

        public async Task Delete(int tieuChiThamDinhId)
        {
            await _tieuChiThamDinh_LyDoRepos.DeleteAsync(x => x.TieuChiThamDinhId == tieuChiThamDinhId);
        }
    }
}