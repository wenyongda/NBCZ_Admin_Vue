using System.Collections.Generic;
using ZR.Model;

namespace NBCZ.Model.System.Dto
{
    public class RoleUserQueryDto : PagerInfo
    {
        public long RoleId { get; set; }

        public string UserName { get; set; }
    }

    public class RoleUsersCreateDto
    {
        /// <summary>
        /// 角色id
        /// </summary>
        public long RoleId { get; set; }

        /// <summary>
        /// 用户编码 [1,2,3,4] 
        /// </summary>
        public List<long> UserIds { get; set; }
    }
}
