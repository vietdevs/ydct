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
    public interface ICuaKhauAppService : IApplicationService
    {
        
    }
    #endregion

    #region MAIN
    public class CuaKhauAppService : PMSAppServiceBase, ICuaKhauAppService
    {
        private readonly IRepository<CuaKhau> _cuaKhauRepos;
        public CuaKhauAppService(IRepository<CuaKhau> cuaKhauRepos)
        {
            _cuaKhauRepos = cuaKhauRepos;
        }
    }
    #endregion
}
