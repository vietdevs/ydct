using Abp.Notifications;
using newPSG.PMS.Dto;

namespace newPSG.PMS.Notifications.Dto
{
    public class GetUserNotificationsInput : PagedInputDto
    {
        public UserNotificationState? State { get; set; }
    }
}