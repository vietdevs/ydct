using Abp.Application.Services;
using newPSG.PMS.Dto;
using newPSG.PMS.Logging.Dto;

namespace newPSG.PMS.Logging
{
    public interface IWebLogAppService : IApplicationService
    {
        GetLatestWebLogsOutput GetLatestWebLogs();

        FileDto DownloadWebLogs();
    }
}
