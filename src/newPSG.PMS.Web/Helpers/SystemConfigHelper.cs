using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using newPSG.PMS.EntityDB;
using newPSG.PMS.Services;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace newPSG.PMS.Web.Helpers
{
    public static class SystemConfigHelper
    {
        public static string GetSettingValue(string settingkey)
        {
            using (var scope = IocManager.Instance.CreateScope())
            {
                var _systemConfigRepos = scope.Resolve<ICauHinhChungClientAppService>();
                var setting = _systemConfigRepos.GetCauHinhChungByKey(settingkey);
                if (setting != null)
                    return setting.GiaTri;
            }
            return "";
        }
    }
}