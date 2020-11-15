using System.Collections.Generic;
using newPSG.PMS.Authorization.Users.Dto;
using newPSG.PMS.Dto;

namespace newPSG.PMS.Authorization.Users.Exporting
{
    public interface IUserListExcelExporter
    {
        FileDto ExportToFile(List<UserListDto> userListDtos);
    }
}