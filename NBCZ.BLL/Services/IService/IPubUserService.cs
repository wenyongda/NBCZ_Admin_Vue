using NBCZ.Model;

namespace NBCZ.BLL.Services.IService
{
    public interface IPubUserService : IBaseService<Pub_User>
    {
        Pub_User SelectUserById(int userId);
    }
}