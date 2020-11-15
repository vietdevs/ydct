using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using newPSG.PMS.Authorization.Users;
using newPSG.PMS.Common.Dto;
using newPSG.PMS.EntityDB;
using System.Collections.Generic;
using System.Linq;

namespace newPSG.PMS.Services
{
    public interface IDanhMucCommonAppService : IApplicationService
    {
        List<ItemDto<long>> GetLanhDaoCuc();

        List<CuaKhau> GetAllCuaKhau();
        List<ItemObj<int>> GetListRole_Level();
    }

    [AbpAuthorize]
    public class DanhMucCommonAppService : PMSAppServiceBase, IDanhMucCommonAppService
    {
        private readonly IRepository<User, long> _userRepos;
        private readonly IRepository<CuaKhau> _cuaKhauRepos;


        public DanhMucCommonAppService(IRepository<User, long> userRepos
            , IRepository<CuaKhau> cuaKhauRepos)
        {
            _userRepos = userRepos;
            _cuaKhauRepos = cuaKhauRepos;
        }
        public List<ItemDto<long>> GetLanhDaoCuc()
        {
            var query = from u in _userRepos.GetAll()
                                   where u.RoleLevel == (int)CommonENum.ROLE_LEVEL.LANH_DAO_CUC
                                   orderby u.Stt descending
                                   select new ItemDto<long>
                                   {
                                       Id = u.Id,
                                       Name = u.Surname + " " + u.Name
                                   };

            return query.ToList();
        }

        public List<CuaKhau> GetAllCuaKhau()
        {
            var query = from ck in _cuaKhauRepos.GetAllList(x => x.IsActive)
                        select new CuaKhau
                        {
                            Id = ck.Id,
                            TenCuaKhauNuocNgoai = ck.TenCuaKhauNuocNgoai,
                            TenCuaKhauVN = ck.TenCuaKhauVN,
                        };
            return query.ToList();
        }
        public List<ItemObj<int>> GetListRole_Level()
        {
            var res = CommonENum.GetListRole_Level();
            return res;
        }
    }
}
