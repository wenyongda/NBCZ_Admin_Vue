using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NBCZ.Model.System;
using ZR.ServiceCore.Model;

namespace NBCZ.BLL.Services.IService
{
    public interface ISysFileService : IBaseService<SysFile>
    {
        Task<long> InsertFile(SysFile file);

        Task<SysFile> SaveFile(SysFile file, Stream formFile, string rootPath, string uploadUrl);
        
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="rootPath"></param>
        /// <param name="fileName"></param>
        /// <param name="fileDir"></param>
        /// <param name="userId"></param>
        /// <param name="nickName"></param>
        /// <param name="formFile"></param>
        /// <param name="uploadUrl"></param>
        /// <returns>文件对象</returns>
        Task<SysFile> SaveFileToLocal(string rootPath, string fileName, string fileDir, long userId, string nickName,
            Stream formFile, string uploadUrl);

        Task<SysFile> SaveFileToAliyun(SysFile file, Stream formFile);
        /// <summary>
        /// 按时间来创建文件夹
        /// </summary>
        /// <param name="path"></param>
        /// <param name="byTimeStore"></param>
        /// <returns>eg: 2020/11/3</returns>
        string GetdirPath(string path = "", bool byTimeStore = true);

        /// <summary>
        /// 取文件名的MD5值(16位)
        /// </summary>
        /// <param name="str">文件名，不包括扩展名</param>
        /// <returns></returns>
        string HashFileName(string str = null);

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="ids">文件ID</param>
        /// <returns></returns>
        Task<int> DeleteSysFileAsync(long[] ids);
        
        /// <summary>
        /// 条件查询文件列表
        /// </summary>
        /// <param name="sysFile">文件对象</param>
        /// <returns></returns>
        List<SysFile> SelectFileList(SysFile sysFile);
        
        /// <summary>
        /// 文件解密流
        /// </summary>
        /// <param name="fileUrl"></param>
        /// <returns></returns>
        Stream DecryptSysFileStream(string fileUrl);
    }
}
