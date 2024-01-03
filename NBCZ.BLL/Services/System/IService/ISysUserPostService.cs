using System.Collections.Generic;
using NBCZ.Model.System;

namespace NBCZ.BLL.Services.System.IService
{
    public interface ISysUserPostService
    {
        public void InsertUserPost(SysUser user);

        public List<long> GetUserPostsByUserId(long userId);

        public string GetPostsStrByUserId(long userId);
        bool Delete(long userId);
    }
}
