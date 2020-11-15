using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using newPSG.PMS.Common.Dto;
using newPSG.PMS.EntityDB;
using Abp.Runtime.Session;
using Microsoft.AspNet.Identity;
using newPSG.PMS.Authorization.Users;
using newPSG.PMS.DoanhNghiepInput;
using Abp.Collections.Extensions;
using Abp.Linq.Extensions;
using System;
using Abp.Authorization;
using newPSG.PMS.Authorization.Users.Dto;
using newPSG.PMS.Authorization.Roles;
using newPSG.PMS.Authorization;
using Abp.Domain.Uow;
using System.Configuration;
using newPSG.PMS.MultiTenancy;
using Abp.Extensions;
using System.Text.RegularExpressions;
using Abp.Application.Services;
using newPSG.PMS.Dto;
using newPSG.PMS.Web;
using Newtonsoft.Json;

namespace newPSG.PMS.Services
{
    public interface IDoanhNghiepPublishAppService : IApplicationService
    {
        Task<PagedResultDto<DoanhNghiepDto>> GetDoanhNghiepFilter(GetDoanhNghiepInput input);
    }
    
    public class DoanhNghiepPublishAppService : PMSAppServiceBase, IDoanhNghiepPublishAppService
    {
        private readonly IRepository<DoanhNghiep, long> _doanhNghiepRepos;
        private readonly IRepository<LoaiHinhDoanhNghiep> _loaiHinhRepos;
        private readonly IRepository<Tinh> _tinhRepos;
        private readonly IRepository<Huyen, long> _huyenRepos;
        private readonly IRepository<Xa, long> _xaRepos;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public DoanhNghiepPublishAppService(
                                     IRepository<DoanhNghiep, long> doanhNghiepRepos,
                                     IRepository<LoaiHinhDoanhNghiep> loaiHinhRepos,
                                     IRepository<Tinh> tinhRepos,
                                     IRepository<Huyen, long> huyenRepos,
                                     IRepository<Xa, long> xaRepos,
                                     IUnitOfWorkManager unitOfWorkManager
        )
        {
            _doanhNghiepRepos = doanhNghiepRepos;
            _loaiHinhRepos = loaiHinhRepos;
            _tinhRepos = tinhRepos;
            _huyenRepos = huyenRepos;
            _xaRepos = xaRepos;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public async Task<PagedResultDto<DoanhNghiepDto>> GetDoanhNghiepFilter(GetDoanhNghiepInput input)
        {
            input.Filter = Utility.StringExtensions.FomatFilterText(input.Filter);
            var query = (from doanhnghiep in _doanhNghiepRepos.GetAll()
                         select new DoanhNghiepDto
                         {
                             TenDoanhNghiep = doanhnghiep.TenDoanhNghiep,
                             DiaChi = doanhnghiep.DiaChi,
                         })
                         .WhereIf(!string.IsNullOrEmpty(input.Filter), u => !string.IsNullOrEmpty(u.TenDoanhNghiep) && u.TenDoanhNghiep.ToLower().Contains(input.Filter));
            var doanhNghiepCount = await query.CountAsync();
            var dataGrids = await query
                .OrderBy(p => p.TenDoanhNghiep)
                .PageBy(input)
               .ToListAsync();

            var DoanhNghiepListDtos = dataGrids.MapTo<List<DoanhNghiepDto>>();
            return new PagedResultDto<DoanhNghiepDto>(doanhNghiepCount, DoanhNghiepListDtos);
        }
    }
}