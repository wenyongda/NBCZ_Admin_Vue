using System;
using System.Configuration;
using System.Linq;
using Autofac;
using log4net;
using SqlSugar;

namespace NBCZ.BLL.T4.DapperExt
{
    public static class SqlSugarSetup
    {
        private static ILog log = LogManager.GetLogger(typeof(SqlSugarSetup));
        /// <summary>
        /// 初始化db
        /// </summary>
        /// <param name="builder"></param>
        public static void AddDb(ContainerBuilder builder)
        {
            var connStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
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
