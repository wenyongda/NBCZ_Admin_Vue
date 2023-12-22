﻿
using NBCZ.DBUtility;

namespace NBCZ.BLL
{
    /// <summary>
    /// 基础服务定义
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBaseService<T> : IBaseRepository<T> where T : class, new()
    {
    }
}