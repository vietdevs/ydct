using Abp.Dependency;
using Castle.Core.Logging;
using newPSG.PMS.MultiTenancy;
using newPSG.PMS.Services;
using Newtonsoft.Json;
using System;
using System.Reflection;
using System.Web.Compilation;

namespace newPSG.PMS.HangFireScheduler
{
    public class HangFileScheduler : ISingletonDependency
    {
        public ILogger Logger { get; set; }

        public HangFileScheduler()
        {
            Logger = NullLogger.Instance;

        }

        private static object lockLienThongTuCongBo = new object();
        private static object lockLienThongDangKyCongBo = new object();
        private static object lockLienThongQuangCao = new object();
        private static object lockLienThongCoSoDuDieuKien = new object();

        public void HangFileSchedulerLienThongTuCongBo()
        {
            lock (lockLienThongTuCongBo)
            {
                string typeThuTuc = typeof(ITuCongBoLTDomainService).AssemblyQualifiedName;
                HangFileSchedulerLienThongCommon(typeThuTuc, "TuCongBo");
            }
        }

        public void HangFileSchedulerLienThongDangKyCongBo()
        {
            lock (lockLienThongDangKyCongBo)
            {
                string typeThuTuc = typeof(IDangKyCongBoLTDomainService).AssemblyQualifiedName;
                HangFileSchedulerLienThongCommon(typeThuTuc, "DangKyCongBo");
            }
        }

        public void HangFileSchedulerLienThongDangKyQuangCao()
        {
            lock (lockLienThongQuangCao)
            {
                string typeThuTuc = typeof(IDangKyQuangCaoLTDomainService).AssemblyQualifiedName;
                HangFileSchedulerLienThongCommon(typeThuTuc, "DangKyQuangCao");
            }
        }

        public void HangFileSchedulerLienThongCoSoDuDieuKien()
        {
            lock (lockLienThongCoSoDuDieuKien)
            {
                string typeThuTuc = typeof(ICoSoDuDieuKienLTDomainService).AssemblyQualifiedName;
                HangFileSchedulerLienThongCommon(typeThuTuc, "CoSoDuDieuKien");
            }
        }

        private void HangFileSchedulerLienThongCommon(string typeThuTuc, string tenThuTuc)
        {
            using (var scope = IocManager.Instance.CreateScope())
            {    
                //Get Tenant  
                var _tenantDomainService = scope.Resolve<ITenantDomainService>();
                var listTenant = _tenantDomainService.GetListTenants();
                if (listTenant != null)
                {
                    foreach (var item in listTenant)
                    {
                        if (item.TinhId != null)
                        {
                            Type lienThong = Type.GetType(typeThuTuc);
                            if (lienThong == null)
                            {
                                continue;
                            }
                            dynamic _lienThongDomain = scope.Resolve(lienThong);
                            var obj = _lienThongDomain.AotuLienThongHoSo(item.Id);

                            Logger.Info($"TenantId={item.Id}_TenancyName={item.TenancyName}_JsonKetQuaLienThong={JsonConvert.SerializeObject(obj)}_{DateTime.Now.ToString("yyyy/MM/dd-HH:mm:ss")}_HangFileSchedulerLienThong{tenThuTuc}");
                        }
                    }
                }
                else
                {
                    Logger.Info($"Error_{DateTime.Now.ToString("yyyy/MM/dd-HH:mm:ss")}_HangFileSchedulerLienThong{tenThuTuc}");
                }
            }
        }
    }
}