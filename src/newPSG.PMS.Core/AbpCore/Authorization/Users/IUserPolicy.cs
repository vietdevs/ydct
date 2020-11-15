using System.Threading.Tasks;
using Abp.Domain.Policies;

namespace newPSG.PMS.Authorization.Users
{
    public interface IUserPolicy : IPolicy
    {
        Task CheckMaxUserCountAsync(int tenantId);
    }
}
