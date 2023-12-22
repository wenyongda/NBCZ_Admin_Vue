using NBCZ.BLL.Services.IService;
using NBCZ.Common;
using NBCZ.DBUtility;
using NBCZ.Model;
using SqlSugar;

namespace NBCZ.BLL.Services
{
    [AppService(ServiceType = typeof(IPubUserService), ServiceLifetime = LifeTime.Transient)]
    public class PubUserService : BaseService<Pub_User>, IPubUserService
    {

        public Pub_User SelectUserById(int userId)
        {
            var user = Queryable().Where(f => f.Id == userId).First();
            return user;
        }
    }
}