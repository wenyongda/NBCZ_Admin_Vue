using System.Collections.Generic;
using System.Web.Http;
using NBCZ.BLL.Services.IService;
using NBCZ.Common.CustomException;
using NBCZ.Common.Extensions;
using NBCZ.Model.System;
using SqlSugar;
using ZR.Common;
using ZR.Model;

namespace NBCZ.Web.Api.Controllers.System
{
    /// <summary>
    /// 岗位管理
    /// </summary>
    // [Verify]
    [RoutePrefix("system/post")]
    public class SysPostController : BaseController
    {
        private readonly ISysPostService _postService;
        public SysPostController(ISysPostService postService)
        {
            _postService = postService;
        }

        /// <summary>
        /// 岗位列表查询
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("list")]
        // [ActionPermissionFilter(Permission = "system:post:list")]
        public IHttpActionResult List([FromUri] SysPost post, [FromUri] PagerInfo pagerInfo)
        {
            var predicate = Expressionable.Create<SysPost>();
            predicate = predicate.AndIF(post.Status.IfNotEmpty(), it => it.Status == post.Status);
            var list = _postService.GetPages(predicate.ToExpression(), pagerInfo, s => new { s.PostSort });

            return SUCCESS(list);
        }

        /// <summary>
        /// 岗位查询
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        [HttpGet, Route("{postId}")]
        // [ActionPermissionFilter(Permission = "system:post:query")]
        public IHttpActionResult Query(long postId = 0)
        {
            return SUCCESS(_postService.GetId(postId));
        }

        /// <summary>
        /// 岗位管理
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        [HttpPost]
        // [ActionPermissionFilter(Permission = "system:post:add")]
        // [Log(Title = "岗位添加", BusinessType = BusinessType.INSERT)]
        public IHttpActionResult Add([FromBody] SysPost post)
        {
            if (UserConstants.NOT_UNIQUE.Equals(_postService.CheckPostNameUnique(post)))
            {
                throw new CustomException($"修改岗位{post.PostName}失败，岗位名已存在");
            }
            if (UserConstants.NOT_UNIQUE.Equals(_postService.CheckPostCodeUnique(post)))
            {
                throw new CustomException($"修改岗位{post.PostName}失败，岗位编码已存在");
            }
            // post.ToCreate(HttpContext);

            return ToResponse(_postService.Add(post));
        }

        /// <summary>
        /// 岗位管理
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        [HttpPut]
        // [ActionPermissionFilter(Permission = "system:post:edit")]
        // [Log(Title = "岗位编辑", BusinessType = BusinessType.UPDATE)]
        public IHttpActionResult Update([FromBody] SysPost post)
        {
            if (UserConstants.NOT_UNIQUE.Equals(_postService.CheckPostNameUnique(post)))
            {
                throw new CustomException($"修改岗位{post.PostName}失败，岗位名已存在");
            }
            if (UserConstants.NOT_UNIQUE.Equals(_postService.CheckPostCodeUnique(post)))
            {
                throw new CustomException($"修改岗位{post.PostName}失败，岗位编码已存在");
            }
            // post.ToUpdate(HttpContext);
            return ToResponse(_postService.Update(post));
        }

        /// <summary>
        /// 岗位删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete, Route("{id}")]
        // [ActionPermissionFilter(Permission = "system:post:remove")]
        // [Log(Title = "岗位删除", BusinessType = BusinessType.DELETE)]
        public IHttpActionResult Delete(string id)
        {
            int[] ids = Tools.SpitIntArrary(id);
            return ToResponse(_postService.Delete(ids));
        }

        /// <summary>
        /// 获取岗位选择框列表
        /// </summary>
        [HttpGet, Route("optionselect")]
        public IHttpActionResult Optionselect()
        {
            List<SysPost> posts = _postService.GetAll();
            return SUCCESS(posts);
        }

        // /// <summary>
        // /// 岗位导出
        // /// </summary>
        // /// <returns></returns>
        // [Log(BusinessType = BusinessType.EXPORT, IsSaveResponseData = false, Title = "岗位导出")]
        // [HttpGet("export")]
        // [ActionPermissionFilter(Permission = "system:post:export")]
        // public IHttpActionResult Export()
        // {
        //     var list = _postService.GetAll();
        //
        //     var result = ExportExcelMini(list, "post", "岗位列表");
        //     return ExportExcel(result.Item2, result.Item1);
        // }
    }
}
