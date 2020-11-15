using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using newPSG.PMS.Common.Dto;
using newPSG.PMS.Dto;
using newPSG.PMS.EntityDB;

namespace newPSG.PMS.Services
{
    #region INTERFACE
    public interface INhomSanPhamAppService : IApplicationService
    {
        Task<PagedResultDto<NhomSanPhamDto>> GetAllServerPaging(NhomSanPhamInputDto input);
        Task<int> CreateOrUpdate(NhomSanPhamDto input);
        Task Delete(int Id);
        Task<List<ItemDto<int>>> GetAllToDDL();
    }
    #endregion

    #region MAIN
    public class NhomSanPhamAppService : PMSAppServiceBase, INhomSanPhamAppService
    {
        private readonly IRepository<NhomSanPham> _nhomSanPhamRepos;
        public NhomSanPhamAppService(IRepository<NhomSanPham> nhomSanPhamRepos)
        {
            _nhomSanPhamRepos = nhomSanPhamRepos;
        }

        public async Task<PagedResultDto<NhomSanPhamDto>> GetAllServerPaging(NhomSanPhamInputDto input)
        {
            var query = (from nhomsanpham in _nhomSanPhamRepos.GetAll()
                         select new NhomSanPhamDto
                         {
                             Id = nhomsanpham.Id,
                             IsActive = nhomsanpham.IsActive,
                             MoTa = nhomsanpham.MoTa,
                             TenNhomSanPham = nhomsanpham.TenNhomSanPham,
                             NiisId = nhomsanpham.NiisId
                         })
            .WhereIf(!string.IsNullOrEmpty(input.Filter), u => u.TenNhomSanPham.LocDauLowerCaseDB().Contains(input.Filter.LocDauLowerCaseDB()) || u.MoTa.LocDauLowerCaseDB().Contains(input.Filter.LocDauLowerCaseDB()));

            var rowCount = await query.CountAsync();
            var dataGrids = await query
                .OrderBy(p => p.Id)
                .PageBy(input)
               .ToListAsync();

            return new PagedResultDto<NhomSanPhamDto>(rowCount, dataGrids);
        }

        public async Task<int> CreateOrUpdate(NhomSanPhamDto input)
        {
            if (input.Id > 0)
            {
                // update
                var updateData = await _nhomSanPhamRepos.GetAsync(input.Id);

                input.MapTo(updateData);

                await _nhomSanPhamRepos.UpdateAsync(updateData);
                return updateData.Id;
            }
            else
            {
                try
                {
                    var insertInput = input.MapTo<NhomSanPham>();
                    int newId = await _nhomSanPhamRepos.InsertAndGetIdAsync(insertInput);
                    return newId;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task Delete(int Id)
        {
            await _nhomSanPhamRepos.DeleteAsync(Id);
        }

        public async Task<List<ItemDto<int>>> GetAllToDDL()
        {
            var query = from cv in _nhomSanPhamRepos.GetAll()
                        where ((cv.IsActive == true))
                        orderby cv.TenNhomSanPham
                        select new ItemDto<int>
                        {
                            Id = cv.Id,
                            Name = cv.TenNhomSanPham
                        };

            return await query.ToListAsync();
        }
    }
    #endregion
}
