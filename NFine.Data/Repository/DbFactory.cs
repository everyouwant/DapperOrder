using System;
using WISE.Data;

namespace NFine.Data
{
    /// <summary>
    /// 数据库建立工厂
    /// </summary>
    public class DbFactory
    {
        /// <summary>
        /// 连接基础库
        /// </summary>
        /// <returns></returns>
        public static IDatabase Base()
        {
            DbHelper.DbType = DatabaseType.SqlServer;
            return  SqlDatabase.DataBase;
        }
    }
}
