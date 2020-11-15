using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using newPSG.PMS.Authorization;
using newPSG.PMS.Authorization.Roles;
using newPSG.PMS.Authorization.Users;
using newPSG.PMS.Editions;
using newPSG.PMS.EntityDB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace newPSG.PMS.Common
{
    #region Interface
    public interface IThongKeChungAppService : IApplicationService
    {
        //ThongKeDto ThongKeTrangChu();
    }
    #endregion
    
    public class ThongKeChungAppService : PMSAppServiceBase, IThongKeChungAppService
    {
        private readonly EditionManager _editionManager;
        private readonly IRepository<DoanhNghiep, long> _doanhNghiepRepos;
        private readonly IRepository<NhomSanPham> _nhomSanPhamRepos;
        private readonly IRepository<QuocGia> _quocGiaRepos;
        private readonly IRepository<User, long> _userRepos;
        private readonly IRepository<Role> _roleRepos;
        private readonly IRepository<Tinh> _tinhRepos;
        private readonly IRepository<Huyen, long> _huyenRepos;
        private readonly IRepository<UserRole, long> _userRoleRepos;
        private readonly IAbpSession _session;
        private readonly CustomSessionAppSession _mySession;
        private readonly ICacheManager _cacheManager;

        public ThongKeChungAppService(EditionManager editionManager,
                                      IRepository<DoanhNghiep, long> doanhNghiepRepos,
                                      IRepository<NhomSanPham> nhomSanPhamRepos,
                                      IRepository<QuocGia> quocGiaRepos,
                                      IRepository<User, long> userRepos,
                                      IRepository<Role> roleRepos,
                                      IRepository<Tinh> tinhRepos,
                                      IRepository<Huyen, long> huyenRepos,
                                      IRepository<UserRole, long> userRoleRepos,
                                      IAbpSession session,
                                      CustomSessionAppSession mySession,
                                      ICacheManager cacheManager)
        {
            _editionManager = editionManager;
            _doanhNghiepRepos = doanhNghiepRepos;
            _nhomSanPhamRepos = nhomSanPhamRepos;
            _quocGiaRepos = quocGiaRepos;
            _userRepos = userRepos;
            _roleRepos = roleRepos;
            _tinhRepos = tinhRepos;
            _huyenRepos = huyenRepos;
            _userRoleRepos = userRoleRepos;
            _session = session;
            _mySession = mySession;
            _cacheManager = cacheManager;
        }
    }
}
