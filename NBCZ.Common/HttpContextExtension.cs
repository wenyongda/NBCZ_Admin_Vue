using System.Security.Claims;
using System.Web;

namespace NBCZ.Common
{
    public static partial class HttpContextExtension
    {
        // /// <summary>
        // /// 获取登录用户id
        // /// </summary>
        // /// <param name="context"></param>
        // /// <returns></returns>
        // public static long GetUId(this HttpContext context)
        // {
        //     var uid = context.User(ClaimTypes.PrimarySid);
        //     return !string.IsNullOrEmpty(uid) ? long.Parse(uid) : 0;
        // }
    }
}