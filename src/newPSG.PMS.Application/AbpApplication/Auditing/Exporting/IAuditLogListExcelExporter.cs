using System.Collections.Generic;
using newPSG.PMS.Auditing.Dto;
using newPSG.PMS.Dto;

namespace newPSG.PMS.Auditing.Exporting
{
    public interface IAuditLogListExcelExporter
    {
        FileDto ExportToFile(List<AuditLogListDto> auditLogListDtos);
    }
}
