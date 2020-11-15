using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Runtime.Caching;
using newPSG.PMS.EntityDB;
using System.Web.Mvc;
using System.Linq;

namespace newPSG.PMS.Web.Controllers
{
    public class ThuTucHanhChinhController : PMSControllerBase
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<BoThuTuc> _boThuTucRepos;
        private readonly ICacheManager _cacheManager;

        public ThuTucHanhChinhController(IUnitOfWorkManager unitOfWorkManager,
            IRepository<BoThuTuc> boThuTucRepos,
             ICacheManager cacheManager)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _boThuTucRepos = boThuTucRepos;
            _cacheManager = cacheManager;
        }

        public PartialViewResult _ThuTucTrangChu(int number)
        {
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                var query = _boThuTucRepos.GetAll().Where(x => x.IsActive && x.IsHome).OrderBy(x => x.SortOrder).Take(number).ToList();
                unitOfWork.Complete();
                return PartialView(query);
            }
        }

        public ActionResult Index()
        {
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                var query = _boThuTucRepos.GetAll().Where(x => x.IsActive).OrderBy(x => x.SortOrder).ToList();
                unitOfWork.Complete();
                return View(query);
            }
        }

        public ActionResult Detail(string alias)
        {
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                var query = _boThuTucRepos.GetAll().Where(x => x.IsActive && x.DuongDan == alias).FirstOrDefault();
                unitOfWork.Complete();
                return View(query);
            }
        }

        public ActionResult Congbocosodudieukienkiemnghiem()
        {
            return View();
        }

        public ActionResult Congbocosodudieukienkhaonghiem()
        {
            return View();
        }
    }
}