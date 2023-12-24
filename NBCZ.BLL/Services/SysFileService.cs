using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Extensions;
using NBCZ.BLL.Services.IService;
using NBCZ.Common;
using NBCZ.Model.System;
using SqlSugar;
using ZR.Common;

namespace NBCZ.BLL.Services
{
    /// <summary>
    /// 文件管理
    /// </summary>
    [AppService(ServiceType = typeof(ISysFileService), ServiceLifetime = LifeTime.Transient)]
    public class SysFileService : BaseService<SysFile>, ISysFileService
    {
        private string domainUrl = AppSettings.GetConfig("ALIYUN_OSS:domainUrl");
        private readonly ISysConfigService _sysConfigService;
        private OptionsSetting _optionsSetting;
        public SysFileService(ISysConfigService sysConfigService, IOptions<OptionsSetting> options)
        {
            _sysConfigService = sysConfigService;
            _optionsSetting = options.Value;
        }

        public async Task<SysFile> SaveFile(SysFile file, Stream formFile, string rootPath, string uploadUrl)
        {
            var storeTypeConfig = _sysConfigService.GetSysConfigByKey("sys.file.storetype").ConfigValue;
            switch ((StoreType)Enum.Parse(typeof(StoreType), storeTypeConfig))
            {
                case StoreType.LOCAL:
                    return await SaveFileToLocal(rootPath, file.FileName, 
                        file.StorePath, file.Create_by, file.Create_name, formFile, uploadUrl);
                case StoreType.ALIYUN:
                    return await SaveFileToAliyun(file, formFile);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// 存储本地
        /// </summary>
        /// <param name="rootPath">存储根目录</param>
        /// <param name="fileName">自定文件名</param>
        /// <param name="fileDir">存储文件夹</param>
        /// <param name="userId"></param>
        /// <param name="nickName"></param>
        /// <param name="formFile">上传的文件流</param>
        /// <param name="uploadUrl"></param>
        /// <returns></returns>
        public async Task<SysFile> SaveFileToLocal(string rootPath, string fileName, string fileDir, long userId,
            string nickName, IFormFile formFile, string uploadUrl)
        {
            string fileExt = Path.GetExtension(formFile.FileName);
            fileName = (fileName.IsEmpty() ? HashFileName() : fileName) + fileExt;

            string filePath = GetdirPath(fileDir);
            string finalFilePath = Path.Combine(rootPath, filePath, fileName);
            double fileSize = Math.Round(formFile.Length / 1024.0, 2);

            if (!Directory.Exists(Path.GetDirectoryName(finalFilePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(finalFilePath));
            }
            
            var md5HashFromFile = Tools.GetMD5HashFromFile(formFile.OpenReadStream());
            var exp = Expressionable.Create<SysFile>();
            exp.AndIF(md5HashFromFile.IsNotEmpty(), it => it.FileMd5 == md5HashFromFile);
            var sysFile = await GetSingleAsync(exp.ToExpression());
            if (sysFile != null)
            {
                sysFile.FileExists = true;
                return sysFile;
            }
            
            var notEncryptExt = _sysConfigService.GetSysConfigByKey("sys.file.notEncryptExt")
                .ConfigValue.Split(",");
            var encryptOnOff = !notEncryptExt.Contains(fileExt) && _sysConfigService
                .GetSysConfigByKey("sys.file.encryptOnOff").ConfigValue.ParseToBool();
            if (encryptOnOff)
            {
                var key = new byte[]
                {
                    13, 57, 174, 2, 102, 26, 253, 192, 141, 38, 175, 244, 32, 86, 163, 25, 237, 134,
                    253, 162, 62, 203, 57, 52, 56, 157, 78, 155, 63, 28, 63, 255
                };
                var iv = new byte[]
                {
                    137, 221, 84, 122, 104, 162, 48, 60, 108, 130, 170, 238, 186, 190, 111, 176
                };
                
                var encryptor = Aes.Create().CreateEncryptor(key, iv);
                await using var outputFileStream = new FileStream(finalFilePath, FileMode.Create);
                await using var cryptoStream = new CryptoStream(outputFileStream, encryptor, CryptoStreamMode.Write);
                await formFile.CopyToAsync(cryptoStream);
            }
            else
            {
                await using var stream = new FileStream(finalFilePath, FileMode.Create);
                await formFile.CopyToAsync(stream);
            }
            
            // string uploadUrl = OptionsSetting.Upload.UploadUrl;
            string accessPath = string.Concat(uploadUrl, "/", filePath.Replace("\\", "/"), "/", fileName);
            SysFile file = new(formFile.FileName, fileName, fileExt, fileSize + "kb", filePath, userId, nickName)
            {
                StoreType = (int)StoreType.LOCAL,
                FileType = formFile.ContentType,
                FileUrl = finalFilePath.Replace("\\", "/"),
                AccessUrl = accessPath
            };
            file.IsEncrypted = encryptOnOff ? "1" : "0";
            file.FileExists = false;
            file.FileMd5 = md5HashFromFile;
            file.Id = await InsertFile(file);
            return file;
        }

        /// <summary>
        /// 上传文件到阿里云
        /// </summary>
        /// <param name="file"></param>
        /// <param name="formFile"></param>
        /// <returns></returns>
        public async Task<SysFile> SaveFileToAliyun(SysFile file, IFormFile formFile)
        {
            file.FileName = (file.FileName.IsEmpty() ? HashFileName() : file.FileName) + file.FileExt;
            file.StorePath = GetdirPath(file.StorePath);
            string finalPath = Path.Combine(file.StorePath, file.FileName);
            
            var md5HashFromFile = Tools.GetMD5HashFromFile(formFile.OpenReadStream());
            var exp = Expressionable.Create<SysFile>();
            exp.AndIF(md5HashFromFile.IsNotEmpty(), it => it.FileMd5 == md5HashFromFile);
            var sysFile = await GetSingleAsync(exp.ToExpression());
            if (sysFile != null)
            {
                sysFile.FileExists = true;
                return sysFile;
            }
            var notEncryptExt = _sysConfigService.GetSysConfigByKey("sys.file.notEncryptExt")
                .ConfigValue.Split(",");
            var encryptOnOff = !notEncryptExt.Contains(file.FileExt) && _sysConfigService.GetSysConfigByKey("sys.file.encryptOnOff")
                .ConfigValue.ParseToBool();
            if (encryptOnOff)
            {
                var key = new byte[]
                {
                    13, 57, 174, 2, 102, 26, 253, 192, 141, 38, 175, 244, 32, 86, 163, 25, 237, 134,
                    253, 162, 62, 203, 57, 52, 56, 157, 78, 155, 63, 28, 63, 255
                };
                var iv = new byte[]
                {
                    137, 221, 84, 122, 104, 162, 48, 60, 108, 130, 170, 238, 186, 190, 111, 176
                };
                var encryptor = Aes.Create().CreateEncryptor(key, iv);
                await using var cryptoStream = new CryptoStream(formFile.OpenReadStream(), encryptor, CryptoStreamMode.Write);
                HttpStatusCode statusCode = AliyunOssHelper.PutObjectFromFile(cryptoStream, finalPath, "");
                if (statusCode != HttpStatusCode.OK) return file;
                
            }
            else
            {
                HttpStatusCode statusCode = AliyunOssHelper.PutObjectFromFile(formFile.OpenReadStream(), finalPath, "");
                if (statusCode != HttpStatusCode.OK) return file;
            }

            file.IsEncrypted = encryptOnOff ? "1" : "0";
            file.FileExists = false;
            file.FileMd5 = md5HashFromFile;
            file.FileUrl = finalPath;
            file.AccessUrl = string.Concat(domainUrl, "/", file.StorePath.Replace("\\", "/"), "/", file.FileName);
            file.StoreType = (int)StoreType.ALIYUN;
            file.Id = await InsertFile(file);

            return file;
        }

        /// <summary>
        /// 获取文件存储目录
        /// </summary>
        /// <param name="storePath"></param>
        /// <param name="byTimeStore">是否按年月日存储</param>
        /// <returns></returns>
        public string GetdirPath(string storePath = "", bool byTimeStore = true)
        {
            DateTime date = DateTime.Now;
            string timeDir = date.ToString("yyyy/MMdd");

            if (!string.IsNullOrEmpty(storePath))
            {
                timeDir = Path.Combine(storePath, timeDir);
            }
            return timeDir;
        }

        public string HashFileName(string str = null)
        {
            if (string.IsNullOrEmpty(str))
            {
                str = Guid.NewGuid().ToString();
            }
            return BitConverter.ToString(MD5.HashData(Encoding.Default.GetBytes(str)), 4, 8).Replace("-", "");
        }

        public async Task<int> DeleteSysFileAsync(long[] ids)
        {
            var sysFiles = await GetListAsync(x => ids.Contains(x.Id));
            // var accessUrls = sysFiles.Select(t => t.AccessUrl).ToList();
            var res = 0;
            foreach (var item in sysFiles)
            {
                switch (item.StoreType)
                {
                    case (int)StoreType.LOCAL:
                        File.Delete(item.FileUrl);
                        res += await DeleteByIdAsync(item.Id) ? 1 : 0;
                        break;
                    case (int)StoreType.ALIYUN:
                        var httpStatusCode = AliyunOssHelper.DeleteFile(item.FileUrl);
                        if (httpStatusCode == HttpStatusCode.NoContent)
                        {
                            res += await DeleteByIdAsync(item.Id) ? 1 : 0;
                        }

                        break;
                }
            }

            return res;
        }

        public List<SysFile> SelectFileList(SysFile sysFile)
        {
            var exp = Expressionable.Create<SysFile>();
            exp.AndIF(sysFile.FileName.IsNotEmpty(), it => it.FileName.Contains(sysFile.FileName));
            exp.AndIF(sysFile.FileExt.IsNotEmpty(), it => it.FileExt == sysFile.FileExt);
            exp.AndIF(sysFile.FileSize.IsNotEmpty(), it => it.FileSize == sysFile.FileSize);
            exp.AndIF(sysFile.StorePath.IsNotEmpty(), it => it.StorePath == sysFile.StorePath);
            exp.AndIF(sysFile.StoreType > 0, it => it.StoreType == sysFile.StoreType);
            exp.AndIF(sysFile.FileType.IsNotEmpty(), it => it.FileType == sysFile.FileType);
            exp.AndIF(sysFile.AccessUrl.IsNotEmpty(), it => it.AccessUrl == sysFile.AccessUrl);
            exp.AndIF(sysFile.FileUrl.IsNotEmpty(), it => it.FileUrl == sysFile.FileUrl);
            exp.AndIF(sysFile.Create_by.IsNotEmpty(), it => it.Create_by == sysFile.Create_by);
            exp.AndIF(sysFile.Create_name.IsNotEmpty(), it => it.Create_name == sysFile.Create_name);
            exp.AndIF(sysFile.Create_time != null, it => it.Create_time == sysFile.Create_time);
            exp.AndIF(sysFile.FileMd5.IsNotEmpty(), it => it.FileMd5 == sysFile.FileMd5);
            exp.AndIF(sysFile.Id > 0, it => it.Id == sysFile.Id);
            return GetList(exp.ToExpression());
        }

        public Task<long> InsertFile(SysFile file)
        {
            try
            {
                return Insertable(file).ExecuteReturnSnowflakeIdAsync();//单条插入返回雪花ID;
            }
            catch (Exception ex)
            {
                Console.WriteLine("存储图片失败" + ex.Message);
                throw new Exception(ex.Message);
            }
        }
        
        /// <summary>
        /// 文件解密流
        /// </summary>
        /// <param name="fileUrl"></param>
        /// <returns></returns>
        public Stream DecryptSysFileStream(string fileUrl)
        {
            var key = new byte[]
            {
                13, 57, 174, 2, 102, 26, 253, 192, 141, 38, 175, 244, 32, 86, 163, 25, 237, 134,
                253, 162, 62, 203, 57, 52, 56, 157, 78, 155, 63, 28, 63, 255
            };
            var iv = new byte[]
            {
                137, 221, 84, 122, 104, 162, 48, 60, 108, 130, 170, 238, 186, 190, 111, 176
            };
            try
            {
                var aes = Aes.Create();
                aes.Key = key;
                aes.IV = iv;
                var decryptor = aes.CreateDecryptor();
                var fsInput  = new FileStream(fileUrl.Replace("/","\\"), FileMode.Open, FileAccess.ReadWrite);
                var msOutput = new MemoryStream();
                var cryptoStream = new CryptoStream(fsInput, decryptor, CryptoStreamMode.Read);
                // 从加密文件中读取并解密
                int bytesRead;
                var buffer = new byte[4096];
                while ((bytesRead = cryptoStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    msOutput.Write(buffer, 0, bytesRead);
                }
            
                fsInput.Position = 0;
                cryptoStream.Close();
                // 返回解密后的流
                msOutput.Position = 0;
                return msOutput;
            }
            catch (Exception e)
            {
                // 处理异常，例如记录日志或抛出自定义异常
                Console.WriteLine($"解密文件时发生异常: {e.Message}");
                throw;
            }
            
        }
    }
}
