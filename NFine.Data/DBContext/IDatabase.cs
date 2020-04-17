using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;

namespace NFine.Data
{
    /// <summary>
    /// 操作数据库接口
    /// </summary>
    public interface IDatabase : IDisposable
    {
        new void Dispose();
        IDatabase BeginTrans();
        int Commit();
        void Rollback();
        void Close();
        int ExecuteBySql(string strSql);
        int ExecuteBySql(List<string> SQL);
        int ExecuteBySql(string strSql, params DbParameter[] dbParameter);
        int ExecuteByProc(string procName);
        int ExecuteByProc(string procName, DbParameter[] dbParameter);
        string ExecuteByProcRes(string procName, DbParameter[] dbParameter);
        int ExecuteByProcReturn(string procName, DbParameter[] dbParameter);
        string FirstColRowValue(string SQL);
        
        IEnumerable<T> ExecuteByProcTable<T>(string procName, DbParameter[] dbParameter);
        DataTable ExecuteByProcReportTable(string procName, params DbParameter[] dbParameter);
        int Insert<T>(T entity, string NotUpdateColName="") where T : class;
        int Insert<T>(IEnumerable<T> entities) where T : class;
        int Delete<T>() where T : class;
        int Delete<T>(T entity) where T : class;
        int Delete<T>(IEnumerable<T> entities) where T : class;
        int Delete<T>(Expression<Func<T, bool>> condition) where T : class,new();
        int Delete<T>(object KeyValue) where T : class;
        int Delete<T>(object[] KeyValue) where T : class;
        int Delete<T>(object propertyValue, string propertyName) where T : class;
        int Update<T>(T entity) where T : class;
        int UpdateColNull<T>(T entity) where T : class;
        int Update<T>(IEnumerable<T> entities) where T : class;
        int Update<T>(Expression<Func<T, bool>> condition) where T : class,new();

        object FindObject(string strSql);
        object FindObject(string strSql, DbParameter[] dbParameter);
        T FindEntity<T>(object KeyValue) where T : class;
        T FindEntity<T>(Expression<Func<T, bool>> condition) where T : class,new();
        IQueryable<T> IQueryable<T>() where T : class,new();
        IQueryable<T> IQueryable<T>(string sql) where T : class,new();
        IQueryable<T> IQueryable<T>(Expression<Func<T, bool>> condition) where T : class,new();
        IEnumerable<T> FindList<T>() where T : class,new();
        IEnumerable<T> FindList<T>(Func<T, object> orderby) where T : class,new();
        IEnumerable<T> FindList<T>(Expression<Func<T, bool>> condition) where T : class,new();
        IEnumerable<T> FindList<T>(string strSql) where T : class;
        IEnumerable<T> FindList<T>(string strSql, DbParameter[] dbParameter) where T : class;
        IEnumerable<T> FindList<T>(string orderField, bool isAsc, int pageSize, int pageIndex, out int total) where T : class,new();
        IEnumerable<T> FindList<T>(Expression<Func<T, bool>> condition, string orderField, bool isAsc, int pageSize, int pageIndex, out int total) where T : class,new();
        IEnumerable<T> FindList<T>(string strSql, string orderField, bool isAsc, int pageSize, int pageIndex, out int total) where T : class;
        IEnumerable<T> FindList<T>(string strSql, DbParameter[] dbParameter, string orderField, bool isAsc, int pageSize, int pageIndex, out int total) where T : class;
        IEnumerable<T> FindList<T, FindT>(FindT FindEntity, string strSql, DbParameter[] dbParameter, string orderField, bool isAsc, int pageSize, int pageIndex, out int total)
            where T : class
            where FindT : class, new();
        DataTable FindTable(string strSql);
        DataTable FindTable(string strSql, DbParameter[] dbParameter);
        DataTable FindTable<T>(string strSql, T FindEntity) where T : class, new();

        DataTable FindTable(string strSql, string orderField, bool isAsc, int pageSize, int pageIndex, out int total);
        DataTable FindTable(string strSql, DbParameter[] dbParameter, string orderField, bool isAsc, int pageSize, int pageIndex, out int total);
        DataTable FindTable<T>(T entity) where T : class,new();
        DataTable FindTable<T>(T entity, string orderField, bool isAsc, int pageSize, int pageIndex, out int total) where T : class,new();
        DataTable FindTable<T, FindT>(FindT FindEntity)
            where FindT : class,new();
        DataTable FindTable<T, FindT>(FindT FindEntity, string orderField, bool isAsc, int pageSize, int pageIndex, out int total,string appendSql = "") where FindT : class,new();
        DataTable FindView<T>(T entity, string appendsql);
        DataTable FindView<T>(T entity, string appendsql,string orderField, bool isAsc, int pageSize, int pageIndex, out int total);
        DataTable FindView<T>(T entity, string querySql, string tablename);
        DataTable FindView<T>(T entity, string querySql, string tablename, string orderField, bool isAsc, int pageSize, int pageIndex, out int total);
        DataTable FindFormatView<T>(T entity, string querySql, string tablename);
        DataTable FindFormatView<T>(T entity, string querySql, string tablename, string orderField, bool isAsc, int pageSize, int pageIndex, out int total);
        void UpdateState<T>(int state, string key_value);
        DataTable ExecProcTable(string procName, string appendSql = "");
        DataTable ExecProcTable(string procName, string orderField, bool isAsc, int pageSize, int pageIndex, out int total , string appendSql = "");

        void Insert(DataSet data);
        void Insert(DataTable Dt);
    }
}
