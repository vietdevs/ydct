using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using newPSG.PMS.Auditing.Dto;
using newPSG.PMS.Dto;

namespace newPSG.PMS.Auditing
{
    public interface IAuditLogAppService : IApplicationService
    {
        Task<PagedResultDto<AuditLogListDto>> GetAuditLogs(GetAuditLogsInput input);

        Task<FileDto> GetAuditLogsToExcel(GetAuditLogsInput input);
    }
}