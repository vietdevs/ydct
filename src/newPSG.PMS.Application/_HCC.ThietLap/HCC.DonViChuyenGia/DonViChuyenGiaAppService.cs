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
    public interface IDonViChuyenGiaAppService : IApplicationService
    {
        Task<dynamic> GetDonViDDL();

        Task<int> CreateOrUpdate(DonViChuyenGiaDto input);

        Task UpdateTrungDonViChuyenGiaId(int id, long truongDonViId);

        Task<DonViChuyenGiaDto> GetForEdit(EntityDto<int> input);

        Task Delete(EntityDto<int> input);

        Task<PagedResultDto<DonViChuyenGiaDto>> GetAllServerPaging(DonViChuyenGiaDtoInput input);
        Task<object> GetAll();
    }

    [DisableAuditing]
    public class DonViChuyenGiaAppService : PMSAppServiceBase, IDonViChuyenGiaAppService
    {
        private readonly IRepository<DonViChuyenGia> _DonViChuyenGiaRepos;
        private readonly IRepository<User, long> _userRepos;
        private readonly IRepository<Tinh> _tinhRepos;
        private readonly IRepository<Huyen, long> _huyenRepos;
        private readonly IRepository<Xa, long> _xaRepos;

        public DonViChuyenGiaAppService(
            IRepository<DonViChuyenGia> DonViChuyenGiaRepos,
            IRepository<User, long> userRepos,
            IRepository<Tinh> tinhRepos,
            IRepository<Huyen, long> huyenRepos,
            IRepository<Xa, long> xaRepos)
        {
            _DonViChuyenGiaRepos = DonViChuyenGiaRepos;
            _userRepos = userRepos;
            _tinhRepos = tinhRepos;
            _huyenRepos = huyenRepos;
            _xaRepos = xaRepos;
        }

        public async Task<dynamic> GetDonViDDL()
        {
            try
            {
                var query = _DonViChuyenGiaRepos.GetAll()
                    .Select(x => new
                    {
                        Id = x.Id,
                        Name = x.Name
                    });

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> CreateOrUpdate(DonViChuyenGiaDto input)
        {
            try
            {
                if (input.Id > 0)
                {
                    // update
                    var updateData = await _DonViChuyenGiaRepos.GetAsync(input.Id);
                    input.MapTo(updateData);
                    await _DonViChuyenGiaRepos.UpdateAsync(updateData);
                    return updateData.Id;
                }
                else
                {
                    var insertInput = input.MapTo<DonViChuyenGia>();
                    int id = await _DonViChuyenGiaRepos.InsertAndGetIdAsync(insertInput);
                    await CurrentUnitOfWork.SaveChangesAsync();
                    return id;
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Xảy ra lỗi trong quá trình xử lý", ex.Message);
            }
        }

        public async Task UpdateTrungDonViChuyenGiaId(int id, long truongDonViId)
        {
            try
            {
                var donVi = await _DonViChuyenGiaRepos.GetAsync(id);
                donVi.TruongDonViId = truongDonViId;
                await _DonViChuyenGiaRepos.UpdateAsync(donVi);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task Delete(EntityDto<int> input)
        {
            await _DonViChuyenGiaRepos.DeleteAsync(input.Id);
        }

        public async Task<DonViChuyenGiaDto> GetForEdit(EntityDto<int> input)
        {
            var data = await _DonViChuyenGiaRepos.FirstOrDefaultAsync(input.Id);
            var res = ObjectMapper.Map<DonViChuyenGiaDto>(data);
            return res;
        }

        public async Task<PagedResultDto<DonViChuyenGiaDto>> GetAllServerPaging(DonViChuyenGiaDtoInput input)
        {
            var query = (from data in _DonViChuyenGiaRepos.GetAll()
                         join r_tinh in _tinhRepos.GetAll() on data.TinhId equals r_tinh.Id into tb_tinh
                         from tinh in tb_tinh.DefaultIfEmpty()

                         join r_huyen in _huyenRepos.GetAll() on data.HuyenId equals r_huyen.Id into tb_huyen
                         from huyen in tb_huyen.DefaultIfEmpty()

                         join r_xa in _xaRepos.GetAll() on data.XaId equals r_xa.Id into tb_xa
                         from xa in tb_xa.DefaultIfEmpty()

                         join r_user in _userRepos.GetAll() on data.TruongDonViId equals r_user.Id into tb_user
                         from user in tb_user.DefaultIfEmpty()
                         select new DonViChuyenGiaDto
                         {
                             Id = data.Id,
                             Name = data.Name,
                             ThuTucId = data.ThuTucId,
                             IsTrongCuc = data.IsTrongCuc,
                             TruongDonViId = data.TruongDonViId,
                             TinhId = data.TinhId,
                             HuyenId = data.HuyenId,
                             XaId = data.XaId,

                             StrTinh = tinh.Ten,
                             StrHuyen = huyen.Ten,
                             StrXa = xa.Ten,

                             TenTruongDonVi = user.Surname + " " + user.Name,
                         });
            //.WhereIf(!string.IsNullOrEmpty(input.Filter), u => u.Name.LocDauLowerCaseDB().Contains(input.Filter.Trim().LocDauLowerCaseDB()));

            var count = await query.CountAsync();
            var dataGrids = await query
                .OrderByDescending(p => p.Id)
                .PageBy(input)
               .ToListAsync();

            return new PagedResultDto<DonViChuyenGiaDto>(count, dataGrids);
        }

        public async Task<object> GetAll()
        {
            var query = from data in _DonViChuyenGiaRepos.GetAll()
                        orderby data.Id
                        select new
                        {
                            Id = data.Id,
                            Name = data.Name,
                            IsTrongCuc = data.IsTrongCuc,
                            StrTrongCuc = data.IsTrongCuc ? "ĐV Trong cục" : "ĐV Ngoài cục"
                        };
            var items = await query.ToListAsync();
            return items;
        }
    }
}