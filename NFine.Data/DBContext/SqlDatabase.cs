using Dapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Configuration;
using NFine.Data.Extensions;
using WISE.Data;
using NFine.Code;
using SQLinq;
using SQLinq.Dapper;

namespace NFine.Data
{
    /// <summary>
    /// 操作数据库
    /// </summary>
    public class SqlDatabase : IDatabase, IDisposable
    {
        /// <summary>
        /// 当前提供者
        /// </summary>
        public static IDatabase  DataBase
        {
            get { return new SqlDatabase(); }
        }
        //#region 构造函数
        ///// <summary>
        ///// 构造方法
        ///// </summary>
        //public SqlDatabase(string connString)
        //{
        //    DbHelper.DbType = DatabaseType.SqlServer;
        //    connectionString = connString;
        //}
        //#endregion
        /// <summary>
        /// 事务开始
        /// </summary>
        /// <returns></returns>
        public IDatabase BeginTrans()
        {
            DbConnection dbConnection = Connection;
            if (dbConnection.State == ConnectionState.Closed)
            {
                dbConnection.Open();
            }
            dbTransaction = dbConnection.BeginTransaction();
            return this;
        }
        #region 属性
        /// <summary>
        /// 获取 数据库连接串
        /// </summary>
        public string connectionString { get; set; } = "NFineDbContext";
        protected DbConnection Connection
        {
            get
            {
                try
                {
                    DbConnection dbconnection = null;
                    if (ConfigurationManager.ConnectionStrings[connectionString] == null)
                    {
                        dbconnection = new SqlConnection(connectionString);
                    }
                    else
                    {
                        dbconnection = new SqlConnection(ConfigurationManager.ConnectionStrings[connectionString].ConnectionString);
                    }
                    dbconnection.Open();
                    return dbconnection;
                }
                catch (SqlException se)
                {
                    throw new Exception(se.Message);
                }
                catch (NullReferenceException)
                {
                    throw new Exception("源路径不存在");
                }
                catch (ArgumentException)
                {
                    throw new Exception("源路径为空");
                }
            }
        }
        /// <summary>
        /// 事务对象
        /// </summary>
        public DbTransaction dbTransaction { get; set; }
        #endregion

        #region 事物提交
        
        /// <summary>
        /// 提交当前操作的结果
        /// </summary>
        public int Commit()
        {
            try
            {
                if (dbTransaction != null)
                {
                    dbTransaction.Commit();
                    this.Close();
                }
                return 1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (dbTransaction == null)
                {
                    this.Close();
                }
            }
        }
        public void Dispose()
        {
            if (dbTransaction != null)
            {
                this.dbTransaction.Dispose();
            }
            this.Dispose();
        }
        /// <summary>
        /// 把当前操作回滚成未提交状态
        /// </summary>
        public void Rollback()
        {
            this.dbTransaction.Rollback();
            this.dbTransaction.Dispose();
            this.Close();
        }
        /// <summary>
        /// 关闭连接 内存回收
        /// </summary>
        public void Close()
        {
            if (dbTransaction == null)
            {
                return;
            }
            DbConnection dbConnection = dbTransaction.Connection;
            if (dbConnection != null && dbConnection.State != ConnectionState.Closed)
            {
                dbConnection.Close();
            }

        }
        #endregion

        #region 执行 SQL 语句
        public int ExecuteBySql(string strSql)
        {
            if (dbTransaction == null)
            {
                using (var connection = Connection)
                {
                    return connection.Execute(strSql);
                }
            }
            else
            {
                dbTransaction.Connection.Execute(strSql, null, dbTransaction);
                return 0;

            }
        }
        public int ExecuteBySql(string strSql, params DbParameter[] dbParameter)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            foreach (var item in dbParameter)
            {
                dynamicParameters.Add(item.ParameterName, item.Value);
            }
            if (dbTransaction == null)
            {
                using (var connection = Connection)
                {
                    return connection.Execute(strSql, dynamicParameters);
                }
            }
            else
            {
                dbTransaction.Connection.Execute(strSql, dynamicParameters, dbTransaction);
                return 0;

            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="dynamicParameters"></param>
        /// <returns></returns>
        public int ExecuteBySql(string strSql, DynamicParameters dynamicParameters)
        {
            if (dbTransaction == null)
            {
                using (var connection = Connection)
                {
                    BeginTrans();
                    dbTransaction.Connection.Execute(strSql, dynamicParameters, dbTransaction);
                    return Commit();

                }
            }
            else
            {
                dbTransaction.Connection.Execute(strSql, dynamicParameters, dbTransaction);
                return 0;

            }
        }
        public int ExecuteByProc(string procName)
        {
            if (dbTransaction == null)
            {
                using (var connection = Connection)
                {
                    return connection.Execute(procName);
                }
            }
            else
            {
                dbTransaction.Connection.Execute(procName, null, dbTransaction);
                return 0;

            }
        }
        public int ExecuteByProc(string procName, params DbParameter[] dbParameter)
        {
            if (dbTransaction == null)
            {
                using (var connection = Connection)
                {
                    DynamicParameters dp = DatabaseCommon.GetDynamicParameter(dbParameter);
                    return connection.Execute(procName, dp, null, null, CommandType.StoredProcedure);
                }
            }
            else
            {
                dbTransaction.Connection.Execute(procName, dbParameter, dbTransaction);
                return 0;

            }
        }
        public string ExecuteByProcRes(string procName, params DbParameter[] dbParameter)
        {
            if (dbTransaction == null)
            {
                using (var connection = Connection)
                {
                    DynamicParameters dp = DatabaseCommon.GetDynamicParameter(dbParameter);
                    dp.Add("@RES", string.Empty, DbType.String, ParameterDirection.Output);
                    connection.Execute(procName, dp, null, null, CommandType.StoredProcedure);
                    var a = dp.Get<string>("@RES");
                    return dp.Get<string>("@RES");
                }
            }
            else
            {
                dbTransaction.Connection.Execute(procName, dbParameter, dbTransaction);
                return null;

            }
        }
        public int ExecuteByProcReturn(string procName, DbParameter[] dbParameter)
        {
            if (dbTransaction == null)
            {
                using (var connection = Connection)
                {
                    DynamicParameters dp = DatabaseCommon.GetDynamicParameter(dbParameter);
                    dp.Add("@RES", string.Empty, DbType.String, ParameterDirection.ReturnValue);
                    connection.Execute(procName, dp, null, null, CommandType.StoredProcedure);
                    return dp.Get<int>("@RES");
                }
            }
            else
            {
                dbTransaction.Connection.Execute(procName, dbParameter, dbTransaction);
                return 0;

            }
        }

        public IEnumerable<T> ExecuteByProcTable<T>(string procName, DbParameter[] dbParameter)
        {
            if (dbTransaction == null)
            {
                using (var dbConnection = Connection)
                {
                    DynamicParameters dp = DatabaseCommon.GetDynamicParameter(dbParameter);
                    return dbConnection.Query<T>(procName, dp, commandType: CommandType.StoredProcedure);
                }
            }
            else
            {
                dbTransaction.Connection.Execute(procName, dbParameter, dbTransaction);
                return null;

            }
        }
        public DataTable ExecuteByProcReportTable(string procName, DbParameter[] dbParameter)
        {
            if (dbTransaction == null)
            {
                using (var dbConnection = Connection)
                {
                    DynamicParameters dp = DatabaseCommon.GetDynamicParameter(dbParameter);
                    var IDataReader = new DbHelper(dbConnection).ExecuteReader(CommandType.StoredProcedure, procName, dbParameter);
                    return ConvertExtension.IDataReaderToDataTable(IDataReader);
                }
            }
            else
            {
                dbTransaction.Connection.Execute(procName, dbParameter, dbTransaction);
                return null;

            }
        }

        public string FirstColRowValue(string SQL)
        {
            using (var dbConnection = Connection)
            {
                var IDataReader = new DbHelper(dbConnection).ExecuteReader(CommandType.Text, SQL);
                var data = ConvertExtension.IDataReaderToDataTable(IDataReader);
                if (data.Rows.Count == 0)
                {
                    return "";
                }
                else
                {
                    return Convert.ToString(data.Rows[0][0]);
                }
            }

        }
        public int ExecuteBySql(List<string> SQL)
        {
            int count = 0;
            if (dbTransaction == null)
            {
                BeginTrans();
                using (var connection = Connection)
                {
                    foreach (string item in SQL)
                    {
                        count += connection.Execute(item);
                    }

                }
                Commit();
                return count;
            }
            else
            {
                foreach (var item in SQL)
                {
                    dbTransaction.Connection.Execute(item, null, dbTransaction);
                }
                return 0;

            }
        }
        #endregion

        #region 对象实体 添加、修改、删除Dapper下未作实现，直接调用执行SQl方法
        /// <summary>
        /// 实体插入dapper作实现
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Insert<T>(T entity, string NotUpdateColName = "") where T : class
        {
            return ExecuteBySql(DatabaseCommon.InsertSql<T>(entity, NotUpdateColName).ToString(), DatabaseCommon.GetDynamicParameter<T>(entity));
        }

        /// <summary>
        /// 实体更新可用状态
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public void UpdateState<T>(int state, string key_value)
        {
            string tableName = EntityAttribute.GetEntityTable<T>();
            string columName = DatabaseCommon.GetEnableColumn<T>();
            string primaryKey = DatabaseCommon.GetKeyField<T>().ToString();
            SqlParameter[] paramsMenber ={
                                             new SqlParameter("@TABLE",tableName) ,
                                            new SqlParameter("@COLUMN",columName) ,
                                            new SqlParameter("@STATE",state),
                                              new SqlParameter("@PRIMARYKEY",primaryKey),
                                            new SqlParameter("@KEY_VALUE",key_value)
                                         };
            ExecuteByProcReturn("P_UPDATESTATE", paramsMenber);
        }

        /// <summary>
        /// 实体插入(dapper)不作实现
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        public int Insert<T>(IEnumerable<T> entities) where T : class
        {
            if (dbTransaction == null)
            {
                BeginTrans();
                foreach (var item in entities)
                {
                    Insert<T>(item);
                }
                return Commit();
            }
            else
            {
                foreach (var item in entities)
                {
                    Insert<T>(item);
                }
                return 0;

            }
        }
        public int Delete<T>() where T : class
        {
            return ExecuteBySql(DatabaseCommon.DeleteSql(EntityAttribute.GetEntityTable<T>()).ToString());
        }
        public int Delete<T>(T entity) where T : class
        {
            return ExecuteBySql(DatabaseCommon.DeleteSql<T>(entity).ToString(), DatabaseCommon.GetParameter<T>(entity));
        }
        public int Delete<T>(IEnumerable<T> entities) where T : class
        {
            if (dbTransaction == null)
            {
                BeginTrans();
                foreach (var item in entities)
                {
                    Delete<T>(item);
                }
                return Commit();
            }
            else
            {
                foreach (var item in entities)
                {
                    Delete<T>(item);
                }
                return 0;
            }
        }
        public int Delete<T>(Expression<Func<T, bool>> condition) where T : class, new()
        {
            bool isTrans = true;
            if (dbTransaction == null)
            {
                BeginTrans();
                isTrans = false;
            }
            string conditionsql = DatabaseCommon.DealExp<T>(condition.Body, OperateType.Delete).ToString();
            DbCommand cmd = dbTransaction.Connection.CreateCommand();
            cmd.Transaction = dbTransaction;
            cmd.CommandText = conditionsql;
            cmd.ExecuteNonQuery();
            if (!isTrans)
            {
                return Commit();
            }
            return 0;
        }
        public int Delete<T>(object keyValue) where T : class
        {
            DynamicParameters dynamicParameters = new global::Dapper.DynamicParameters();
            dynamicParameters.Add("@primarykey", keyValue);
            return ExecuteBySql("Delete " + EntityAttribute.GetEntityTable<T>() + " where " + EntityAttribute.GetEntityKey<T>() + "=@primarykey", dynamicParameters);
        }
        public int Delete<T>(object[] keyValue) where T : class
        {
            DynamicParameters dynamicParameters = new global::Dapper.DynamicParameters();
            string whereString = string.Empty;
            string keyString = EntityAttribute.GetEntityKey<T>();
            for (int i = 0; i < keyValue.Length; i++)
            {
                string ParametersName = string.Format("@primarykey{0}", i);
                dynamicParameters.Add(ParametersName, keyValue[i]);
                whereString += string.Format("{0} {1} = {2}", i == 0 ? string.Empty : " OR ", keyString, ParametersName);
            }
            string deleteString = "Delete " + EntityAttribute.GetEntityTable<T>() + " where ";
            ExecuteBySql(string.Format("{0}{1}", deleteString, whereString), dynamicParameters);
            return dbTransaction == null ? Commit() : 0;
        }
        public int Delete<T>(object propertyValue, string propertyName) where T : class
        {
            bool isTrans = true;
            if (dbTransaction == null)
            {
                BeginTrans();
                isTrans = false;
            }
            using (var dbConnection = Connection)
            {
                IEnumerable<T> entitys = dbConnection.Query<T>("select * from " + EntityAttribute.GetEntityTable<T>() + " where " + propertyName + "=@propertyValue", new { propertyValue = propertyValue.ToString() });
                foreach (var entity in entitys)
                {
                    Delete<T>(entity);
                }
                if (!isTrans)
                {
                    return Commit();
                }
            }
            return 0;
        }
        public int Update<T>(T entity) where T : class
        {
            return ExecuteBySql(DatabaseCommon.UpdateSql<T>(entity).ToString(), DatabaseCommon.GetParameter<T>(entity));
        }
        public int UpdateColNull<T>(T entity) where T : class
        {
            return ExecuteBySql(DatabaseCommon.UpdateNullSql<T>(entity).ToString());
        }
        public int Update<T>(IEnumerable<T> entities) where T : class
        {
            if (dbTransaction == null)
            {
                BeginTrans();
                foreach (var item in entities)
                {
                    Update<T>(item);
                }
                return Commit();
            }
            else
            {
                foreach (var item in entities)
                {
                    Update<T>(item);
                }
                return 0;
            }
        }
        public int Update<T>(Expression<Func<T, bool>> condition) where T : class, new()
        {
            bool isTrans = true;
            if (dbTransaction == null)
            {
                BeginTrans();
                isTrans = false;
            }
            IEnumerable<T> entities = dbTransaction.Connection.Query<T>(new SQLinq<T>(EntityAttribute.GetEntityTable<T>()).Where(condition));
            Update<T>(entities);
            if (!isTrans)
            {
                return Commit();
            }
            return 0;
        }
        #endregion

        #region 对象实体 查询
        public T FindEntity<T>(object keyValue) where T : class
        {
            using (var dbConnection = Connection)
            {
                Type type = typeof(T);
                string viewName = "";
                var viewAttribute = type.GetCustomAttributes(true).OfType<ViewAttribute>();
                var descriptionAttributes = viewAttribute as ViewAttribute[] ?? viewAttribute.ToArray();
                if (descriptionAttributes.Any())
                    viewName = descriptionAttributes.ToList()[0].viewName;
                string tableName = string.IsNullOrWhiteSpace(viewName) ? EntityAttribute.GetEntityTable<T>() : viewName;

                var data = dbConnection.Query<T>("select * from " + tableName + " where " + EntityAttribute.GetEntityKey<T>() + "=@key", new { key = keyValue.ToString() });
                return data.FirstOrDefault();
            }
        }
        public T FindEntity<T>(Expression<Func<T, bool>> condition) where T : class, new()
        {
            using (var dbConnection = Connection)
            {
                string conditionsql = DatabaseCommon.DealExp<T>(condition.ClearLeft(), OperateType.Select).ToString();
                var data = dbConnection.Query<T>(conditionsql);
                return data.FirstOrDefault();
            }
        }
        /// <summary>
        /// 未实现
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IQueryable<T> IQueryable<T>() where T : class, new()
        {
            string tablename = EntityAttribute.GetEntityTable<T>();
            string sql = string.Format("select * from {0} where 1=1", tablename);
            using (var dbConnection = Connection)
            {
                var data = dbConnection.Query<T>(sql);
                return data.AsQueryable();
            }
        }

        public IQueryable<T> ITranslateQueryable<T>() where T : class, new()
        {
            string tablename = EntityAttribute.GetEntityTable<T>();
            string sql = string.Format("select * from {0} where 1=1", tablename);
            using (var dbConnection = Connection)
            {
                var translateInfo = DatabaseCommon.GetTranslateValue<T>();
                if (translateInfo.Count(x => x.Length > 0) > 0)
                {
                    sql = DatabaseCommon.PottingSql<T>(translateInfo, sql, tablename);
                }
                var data = dbConnection.Query<T>(sql);
                return data.AsQueryable();
            }
        }
        public IQueryable<T> IQueryable<T>(string sql) where T : class, new()
        {
            string tablename = EntityAttribute.GetEntityTable<T>();
            using (var dbConnection = Connection)
            {
                var translateInfo = DatabaseCommon.GetTranslateValue<T>();
                if (translateInfo.Count(x => x.Length > 0) > 0)
                {
                    sql = DatabaseCommon.PottingSql<T>(translateInfo, sql, tablename);
                }
                var data = dbConnection.Query<T>(sql);
                return data.AsQueryable();
            }
        }

        /// <summary>
        /// 未实现
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition"></param>
        /// <returns></returns>
        public IQueryable<T> IQueryable<T>(Expression<Func<T, bool>> condition) where T : class, new()
        {
            using (var dbConnection = Connection)
            {
                string conditionsql = DatabaseCommon.DealExp<T>(condition.Body, OperateType.Select).ToString();
                var data = dbConnection.Query<T>(string.Format(conditionsql));
                return data.AsQueryable();
            }
        }

        public IEnumerable<T> FindList<T>() where T : class, new()
        {
            using (var dbConnection = Connection)
            {
                return dbConnection.Query<T>(new SQLinq<T>()).ToList();
            }
        }
        public IEnumerable<T> FindList<T>(Func<T, object> keySelector) where T : class, new()
        {
            using (var dbConnection = Connection)
            {
                return dbConnection.Query<T>(new SQLinq<T>()).OrderBy(keySelector).ToList();
            }
        }
        public IEnumerable<T> FindList<T>(Expression<Func<T, bool>> condition) where T : class, new()
        {
            using (var dbConnection = Connection)
            {
                return dbConnection.Query<T>(new SQLinq<T>().Where(condition)).ToList();
            }
        }
        public IEnumerable<T> FindList<T>(string strSql) where T : class
        {
            return FindList<T>(strSql, new DbParameter[0]);
        }
        public IEnumerable<T> FindList<T>(string strSql, DbParameter[] dbParameter) where T : class
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            foreach (var item in dbParameter)
            {
                dynamicParameters.Add(item.ParameterName, item.Value);
            }
            using (var dbConnection = Connection)
            {
                return dbConnection.Query<T>(strSql, dynamicParameters);
            }
        }
        public IEnumerable<T> FindList<T>(string orderField, bool isAsc, int pageSize, int pageIndex, out int total) where T : class, new()
        {
            using (var dbConnection = Connection)
            {
                StringBuilder sb = new StringBuilder();
                int num = (pageIndex - 1) * pageSize;
                int num1 = pageIndex * pageSize;

                sb.Append("Select * From (Select ROW_NUMBER() Over ( order by " + orderField + ")");
                sb.Append(" As rowNum, * From " + EntityAttribute.GetEntityTable<T>() + ") As N Where rowNum > " + num + " And rowNum <= " + num1 + "");

                var dataQuery = dbConnection.Query<T>(sb.ToString());

                total = Convert.ToInt32(new DbHelper(dbConnection).ExecuteScalar(CommandType.Text, "Select Count(1)  From " + EntityAttribute.GetEntityTable<T>()));
                return dataQuery.ToList();
            }
        }
        public IEnumerable<T> FindList<T>(Expression<Func<T, bool>> condition, string orderField, bool isAsc, int pageSize, int pageIndex, out int total) where T : class, new()
        {
            using (var dbConnection = Connection)
            {
                string strSql = DatabaseCommon.DealExp<T>(condition, OperateType.Select).ToString();
                int num = (pageIndex - 1) * pageSize;
                int num1 = (pageIndex) * pageSize;
                orderField = "order by " + orderField;
                //if (orderField.Length != 0)
                //{
                //    sql += "order by ";
                //}
                StringBuilder sb = new StringBuilder();
                sb.Append("Select * From (Select ROW_NUMBER() Over (" + orderField + ")");
                sb.Append(" As rowNum, * From (" + strSql + ") As T ) As N Where rowNum > " + num + " And rowNum <= " + num1 + "");
                total = Convert.ToInt32(new DbHelper(dbConnection).ExecuteScalar(CommandType.Text, "Select Count(1) From (" + strSql + ") As t"));
                var dataQuery = dbConnection.Query<T>(sb.ToString());
                return dataQuery.ToList();
            }
        }
        public IEnumerable<T> FindList<T>(string strSql, string orderField, bool isAsc, int pageSize, int pageIndex, out int total) where T : class
        {
            return FindList<T>(strSql, null, orderField, isAsc, pageSize, pageIndex, out total);
        }
        public IEnumerable<T> FindList<T, FindT>(FindT FindEntity, string strSql, DbParameter[] dbParameter, string orderField, bool isAsc, int pageSize, int pageIndex, out int total)
            where T : class
            where FindT : class, new()
        {
            using (var dbConnection = Connection)
            {
                string sql = DatabaseCommon.QueryConditionSQL<FindT>(FindEntity).ToString();
                strSql += sql;
                return FindList<T>(strSql, orderField, isAsc, pageSize, pageIndex, out total);
            }
        }
        public IEnumerable<T> FindList<T>(string strSql, DbParameter[] dbParameter, string orderField, bool isAsc, int pageSize, int pageIndex, out int total) where T : class
        {
            using (var dbConnection = Connection)
            {
                StringBuilder sb = new StringBuilder();
                if (pageIndex == 0)
                {
                    pageIndex = 1;
                }
                int num = (pageIndex - 1) * pageSize;
                int num1 = (pageIndex) * pageSize;
                string OrderBy = "";

                if (!string.IsNullOrEmpty(orderField))
                {
                    if (orderField.ToUpper().IndexOf("ASC") + orderField.ToUpper().IndexOf("DESC") > 0)
                    {
                        OrderBy = "Order By " + orderField;
                    }
                    else
                    {
                        OrderBy = "Order By " + orderField + " " + (isAsc ? "ASC" : "DESC");
                    }
                }
                else
                {
                    OrderBy = "order by (select 0)";
                }
                sb.Append("Select * From (Select ROW_NUMBER() Over (" + OrderBy + ")");
                sb.Append(" As rowNum, * From (" + strSql + ") As T ) As N Where rowNum > " + num + " And rowNum <= " + num1 + "");
                total = Convert.ToInt32(new DbHelper(dbConnection).ExecuteScalar(CommandType.Text, "Select Count(1) From (" + strSql + ") As t", dbParameter));
                var IDataReader = new DbHelper(dbConnection).ExecuteReader(CommandType.Text, sb.ToString(), dbParameter);
                return ConvertExtension.IDataReaderToList<T>(IDataReader);
            }
        }
        #endregion

        #region 数据源查询
        public DataTable FindTable(string strSql)
        {
            return FindTable(strSql, null);
        }
        public DataTable FindTable<T>(string strSql, T FindEntity) where T : class, new()
        {
            return FindTable(strSql + DatabaseCommon.QueryConditionSQL<T>(FindEntity).ToString());
        }
        public DataTable FindTable(string strSql, DbParameter[] dbParameter)
        {
            using (var dbConnection = Connection)
            {
                var IDataReader = new DbHelper(dbConnection).ExecuteReader(CommandType.Text, strSql, dbParameter);
                return ConvertExtension.IDataReaderToDataTable(IDataReader);
            }
        }
        public DataTable FindTable(string strSql, string orderField, bool isAsc, int pageSize, int pageIndex, out int total)
        {
            return FindTable(strSql, null, orderField, isAsc, pageSize, pageIndex, out total);
        }
        public DataTable FindTable(string strSql, DbParameter[] dbParameter, string orderField, bool isAsc, int pageSize, int pageIndex, out int total)
        {
            using (var dbConnection = Connection)
            {
                StringBuilder sb = new StringBuilder();
                if (pageIndex == 0)
                {
                    pageIndex = 1;
                }
                int num = (pageIndex - 1) * pageSize;
                int num1 = (pageIndex) * pageSize;
                string OrderBy = "";

                if (!string.IsNullOrEmpty(orderField))
                {
                    if (orderField.ToUpper().IndexOf("ASC") + orderField.ToUpper().IndexOf("DESC") > 0)
                    {
                        OrderBy = "Order By " + orderField;
                    }
                    else
                    {
                        OrderBy = "Order By " + orderField + " " + (isAsc ? "ASC" : "DESC");
                    }
                }
                else
                {
                    OrderBy = "order by (select 0)";
                }
                sb.Append("Select * From (Select ROW_NUMBER() Over (" + OrderBy + ")");
                sb.Append(" As rowNum, * From (" + strSql + ") As T ) As N Where rowNum > " + num + " And rowNum <= " + num1 + "");
                total = Convert.ToInt32(new DbHelper(dbConnection).ExecuteScalar(CommandType.Text, "Select Count(1) From (" + strSql + ") As t", dbParameter));
                var IDataReader = new DbHelper(dbConnection).ExecuteReader(CommandType.Text, sb.ToString(), dbParameter);
                DataTable resultTable = ConvertExtension.IDataReaderToDataTable(IDataReader);
                resultTable.Columns.Remove("rowNum");
                return resultTable;
            }
        }
        public object FindObject(string strSql)
        {
            return FindObject(strSql, null);
        }
        public object FindObject(string strSql, DbParameter[] dbParameter)
        {
            using (var dbConnection = Connection)
            {
                return new DbHelper(dbConnection).ExecuteScalar(CommandType.Text, strSql, dbParameter);
            }
        }

        public DataTable FindTable<T, FindT>(FindT FindEntity, string orderField, bool isAsc, int pageSize, int pageIndex, out int total, string appendSql = "")
            where FindT : class, new()
        {
            string sql = DatabaseCommon.QueryWhereSQL<FindT>(FindEntity).ToString();
            Type type = FindEntity.GetType();
            string viewName = "";
            var viewAttribute = type.GetCustomAttributes(true).OfType<ViewAttribute>();
            var descriptionAttributes = viewAttribute as ViewAttribute[] ?? viewAttribute.ToArray();
            if (descriptionAttributes.Any())
                viewName = descriptionAttributes.ToList()[0].viewName;
            string tableName = string.IsNullOrWhiteSpace(viewName) ? EntityAttribute.GetEntityTable<T>() : viewName;
            var translateInfo = DatabaseCommon.GetTranslateValue<T>();
            if (translateInfo.Count(x => x.Length > 0) > 0)
            {
                sql = DatabaseCommon.PottingSql<T>(translateInfo, sql, tableName, string.IsNullOrWhiteSpace(viewName) ? false : true);
            }
            if (!string.IsNullOrEmpty(appendSql))
            {
                sql += appendSql;
            }
            return FindTable(sql, orderField, isAsc, pageSize, pageIndex, out total);
        }

        public DataTable FindTable<T, FindT>(FindT FindEntity)
            where FindT : class, new()
        {
            string sql = DatabaseCommon.QueryWhereSQL<FindT>(FindEntity).ToString();
            Type type = FindEntity.GetType();
            string viewName = "";
            var viewAttribute = type.GetCustomAttributes(true).OfType<ViewAttribute>();
            var descriptionAttributes = viewAttribute as ViewAttribute[] ?? viewAttribute.ToArray();
            if (descriptionAttributes.Any())
                viewName = descriptionAttributes.ToList()[0].viewName;
            string tableName = string.IsNullOrWhiteSpace(viewName) ? EntityAttribute.GetEntityTable<T>() : viewName;
            var translateInfo = DatabaseCommon.GetTranslateValue<T>();
            if (translateInfo.Count(x => x.Length > 0) > 0)
            {
                sql = DatabaseCommon.PottingSql<T>(translateInfo, sql, tableName, string.IsNullOrWhiteSpace(viewName) ? false : true);
            }
            return FindTable(sql);
        }

        public DataTable FindTable<T>(T entity) where T : class, new()
        {

            return FindTable(DatabaseCommon.QueryWhereSQL<T>(entity).ToString());
        }

        public DataTable FindTable<T>(T entity, string orderField, bool isAsc, int pageSize, int pageIndex, out int total) where T : class, new()
        {
            return FindTable(DatabaseCommon.QueryWhereSQL<T>(entity).ToString(), orderField, isAsc, pageSize, pageIndex, out total);
        }
        public DataTable FindView<T>(T entity, string appendsql)
        {
            return FindTable(DatabaseCommon.QueryWhereSQL<T>(entity).ToString() + appendsql);
        }
        public DataTable FindView<T>(T entity, string appendsql, string orderField, bool isAsc, int pageSize, int pageIndex, out int total)
        {
            return FindTable(DatabaseCommon.QueryWhereSQL<T>(entity).ToString() + appendsql, orderField, isAsc, pageSize, pageIndex, out total);
        }
        public DataTable FindView<T>(T entity, string querySql, string tablename)
        {
            StringBuilder sb = new StringBuilder(querySql);
            sb.Append(" WHERE 1=1 ");
            return FindTable(DatabaseCommon.GetQueryWhereString(entity, tablename, entity.GetType(), sb).ToString());
        }
        public DataTable ExecProcTable(string procName, string appendSql = "")
        {
            StringBuilder sb = new StringBuilder("EXEC ");
            sb.Append(procName);
            sb.Append(string.Format("  @APPENDSQL='{0}'", appendSql));
            return FindTable(sb.ToString());
        }
        public DataTable ExecProcTable(string procName, string orderField, bool isAsc, int pageSize, int pageIndex, out int total, string appendSql = "")
        {
            using (var dbConnection = Connection)
            {
                StringBuilder sb = new StringBuilder("EXEC ");
                sb.Append(procName);
                sb.Append(string.Format("  @APPENDSQL='{0}'", appendSql));
                if (!string.IsNullOrEmpty(orderField))
                {
                    sb.Append(string.Format(" , @ORDERBY='{0}'", orderField));
                }
                if (!isAsc)
                {
                    sb.Append(string.Format(" , @ISASC='{0}'", "desc"));
                }
                int num = (pageIndex - 1) * pageSize;
                int num1 = (pageIndex) * pageSize;
                sb.Append(string.Format("  ,@MIN_NUM='{0}'", num.ToString()));
                sb.Append(string.Format("  ,@MAX_NUM='{0}'", num1.ToString()));
                var sbtotal = string.Format("{0} {1}", sb.ToString(), ",@FLAGE ='0'");
                total = Convert.ToInt32(new DbHelper(dbConnection).ExecuteScalar(CommandType.Text, sbtotal.ToString()));
                return FindTable(sb.ToString());
            }
        }

        public DataTable FindFormatView<T>(T entity, string queryFormatSql, string tablename, string orderField, bool isAsc, int pageSize, int pageIndex, out int total)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(DatabaseCommon.GetQueryWhereString(entity, tablename, entity.GetType(), sb).ToString());
            return FindTable(string.Format(queryFormatSql, sb.ToString()), orderField, isAsc, pageSize, pageIndex, out total);
        }

        public DataTable FindFormatView<T>(T entity, string queryFormatSql, string tablename)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(DatabaseCommon.GetQueryWhereString(entity, tablename, entity.GetType(), sb).ToString());
            return FindTable(string.Format(queryFormatSql, sb.ToString()));
        }
        public DataTable FindView<T>(T entity, string querySql, string tablename, string orderField, bool isAsc, int pageSize, int pageIndex, out int total)
        {
            StringBuilder sb = new StringBuilder(querySql);
            sb.Append(" WHERE 1=1 ");
            return FindTable(DatabaseCommon.GetQueryWhereString(entity, tablename, entity.GetType(), sb).ToString(), orderField, isAsc, pageSize, pageIndex, out total);
        }


        #endregion

        #region DataSet添加数据
        public void Insert(DataSet Dst)
        {

            string connStr = "";
            if (ConfigurationManager.ConnectionStrings[connectionString] == null)
            {
                connStr = connectionString;
            }
            else
            {
                connStr = ConfigurationManager.ConnectionStrings[connectionString].ConnectionString;
            }
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                using (SqlTransaction tran = con.BeginTransaction())
                {
                    using (SqlBulkCopy copy = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                    {
                        try
                        {
                            foreach (DataTable Dt in Dst.Tables)
                            {
                                copy.ColumnMappings.Clear();
                                copy.DestinationTableName = Dt.TableName;
                                foreach (DataColumn item in Dt.Columns)
                                {
                                    copy.ColumnMappings.Add(item.ColumnName, item.ColumnName);
                                }
                                copy.WriteToServer(Dt);
                            }
                            tran.Commit();
                        }
                        catch (Exception)
                        {
                            tran.Rollback();
                            throw;
                        }
                    }
                }
            }

        }


        public void Insert(DataTable Dt)
        {
            string connStr = "";
            if (ConfigurationManager.ConnectionStrings[connectionString] == null)
            {
                connStr = connectionString;
            }
            else
            {
                connStr = ConfigurationManager.ConnectionStrings[connectionString].ConnectionString;
            }
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                using (SqlTransaction tran = con.BeginTransaction())
                {
                    using (SqlBulkCopy copy = new SqlBulkCopy(con, SqlBulkCopyOptions.CheckConstraints, tran))
                    {
                        try
                        {
                            copy.DestinationTableName = Dt.TableName;
                            foreach (DataColumn item in Dt.Columns)
                            {
                                copy.ColumnMappings.Add(item.ColumnName, item.ColumnName);
                            }
                            copy.WriteToServer(Dt);
                            tran.Commit();
                        }
                        catch (Exception)
                        {
                            tran.Rollback();
                            throw;
                        }
                    }
                }
            }
        }

        
        #endregion



    }
}
