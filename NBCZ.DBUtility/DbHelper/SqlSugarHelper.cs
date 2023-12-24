using SqlSugar;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBCZ.DBUtility.DbHelper
{
    public static class SqlSugarHelper
    {
        //private static ILog log = LogManager.GetLogger(typeof(SqlSugarHelper));

        private static readonly string connStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;

        public static SqlSugarScope Db = new SqlSugarScope(new ConnectionConfig()
        {
            DbType = DbType.MySqlConnector,
            ConnectionString = connStr,
            IsAutoCloseConnection = true,
            InitKeyType = InitKeyType.Attribute,
            ConfigureExternalServices = new ConfigureExternalServices()
            {
                DataInfoCacheService = new SqlSugarCache()
            }
        },
            db => {
                db.Aop.OnLogExecuting = (sql, pars) =>
                {
                    // Console.WriteLine(sql);
                    // Console.WriteLine(string.Join(",", pars?.Select(it => it.ParameterName + ":" + it.Value)));
                    var str = $"Executing SQL: {UtilMethods.GetSqlString(DbType.MySql, sql, pars)}\n";
                    if (sql.StartsWith("UPDATE", StringComparison.OrdinalIgnoreCase) || sql.StartsWith("INSERT", StringComparison.OrdinalIgnoreCase))
                        //log.Warn(str);
                        Console.WriteLine(str);
                    else if (sql.StartsWith("DELETE", StringComparison.OrdinalIgnoreCase) || sql.StartsWith("TRUNCATE", StringComparison.OrdinalIgnoreCase))
                        //log.Error(str);
                        Console.WriteLine(str);
                    else
                        //log.Info(str);
                        Console.WriteLine(str);
                };
            });
    }
}
