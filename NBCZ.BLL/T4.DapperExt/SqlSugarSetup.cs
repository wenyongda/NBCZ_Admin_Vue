using System;
using System.Configuration;
using Autofac;
using NLog;
using SqlSugar;

namespace NBCZ.BLL.T4.DapperExt
{
    public static class SqlSugarSetup
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        private static readonly string connStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;

        /// <summary>
        /// 初始化db
        /// </summary>
        /// <param name="builder"></param>
        public static void AddDb(ContainerBuilder builder)
        {
            var sqlSugarClient = new SqlSugarClient(new ConnectionConfig()
                {
                    DbType = DbType.MySqlConnector,
                    ConnectionString = connStr,
                    IsAutoCloseConnection = true,
                    InitKeyType = InitKeyType.Attribute
                });
            sqlSugarClient.Aop.OnLogExecuting = (sql, pars) =>
            {
                // Console.WriteLine(sql);
                // Console.WriteLine(string.Join(",", pars?.Select(it => it.ParameterName + ":" + it.Value)));
                var str = $"Executing SQL: {UtilMethods.GetSqlString(DbType.MySql, sql, pars)}\n";
                if (sql.StartsWith("UPDATE", StringComparison.OrdinalIgnoreCase) || sql.StartsWith("INSERT", StringComparison.OrdinalIgnoreCase))
                    log.Warn(str);
                else if (sql.StartsWith("DELETE", StringComparison.OrdinalIgnoreCase) || sql.StartsWith("TRUNCATE", StringComparison.OrdinalIgnoreCase))
                    log.Error(str);
                else
                    log.Info(str);
            };
            builder.RegisterInstance(sqlSugarClient).As<ISqlSugarClient>();
        }
    }
}
