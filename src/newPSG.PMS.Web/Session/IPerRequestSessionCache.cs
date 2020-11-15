using System.Threading.Tasks;
using newPSG.PMS.Sessions.Dto;

namespace newPSG.PMS.Web.Session
{
    public interface IPerRequestSessionCache
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformationsAsync();
    }
}
