using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Runtime.Caching;
using newPSG.PMS.EntityDB;
using System.Linq;
using System.Web.Mvc;

namespace newPSG.PMS.Web.Controllers
{
    public class ThongBaoController : PMSControllerBase
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<ThongBao> _thongBaoRepos;
        private readonly ICacheManager _cacheManager;

        public ThongBaoController(IUnitOfWorkManager unitOfWorkManager,
            IRepository<ThongBao> thongBaoRepos,
             ICacheManager cacheManager)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _thongBaoRepos = thongBaoRepos;
            _cacheManager = cacheManager;
        }

        public ActionResult Index()
        {
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                var query = _thongBaoRepos.GetAll().Where(x => x.IsActive).OrderBy(x => x.SortOrder).ToList();
                unitOfWork.Complete();
                return View(query);
            }
        }

        public ActionResult Detail(string alias)
        {
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                var query = _thongBaoRepos.GetAll().Where(x => x.IsActive && x.DuongDan == alias).FirstOrDefault();
                unitOfWork.Complete();
                return View(query);
            }
        }
    }
}