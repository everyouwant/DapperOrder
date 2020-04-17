using NFine.Code;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;

namespace NFine.Data
{
    /// <summary>
    /// 版 本 6.1
    /// Copyright (c) 2017 重庆万紫科技有限公司
    /// 创建人：YongTony
    /// 日 期：2015.10.10
    /// 描 述：定义仓储模型中的数据标准操作接口
    /// </summary>
    public interface IRepositoryBase:IDisposable
    {
        IRepositoryBase BeginTrans();
        void Commit();
        void Rollback();

        int ExecuteBySql(string strSql);
        int ExecuteBySql(List<string> strSql);
        int ExecuteBySql(string strSql, params DbParameter[] dbParameter);
        int ExecuteByProc(string procName);
        int ExecuteByProc(string procName, params DbParameter[] dbParameter);
        string ExecuteByProcRes(string procName, params DbParameter[] dbParameter);
        int ExecuteByProcReturn(string procName, params DbParameter[] dbParameter);
        string FirstColRowValue(string SQL);
        IEnumerable<T> ExecuteByProcTable<T>(string procName, params DbParameter[] dbParameter);
        DataTable ExecuteByProcReportTable(string procName, params DbParameter[] dbParameter);
        int Insert<T>(T entity, string NotUpdateColName="") where T : class;
        int Insert<T>(List<T> entity) where T : class;
        int Delete<T>() where T : class;
        int Delete<T>(T entity) where T : class;
        int Delete<T>(List<T> entity) where T : class;
        int Delete<T>(Expression<Func<T, bool>> condition) where T : class,new();
        int Delete<T>(object keyValue) where T : class;
        int Delete<T>(object[] keyValue) where T : class;
        int Delete<T>(object propertyValue, string propertyName) where T : class;
        int Update<T>(T entity) where T : class;
        int UpdateColNull<T>(T entity) where T : class;
        int Update<T>(List<T> entity) where T : class;
        int Update<T>(Expression<Func<T, bool>> condition) where T : class,new();

        T FindEntity<T>(object keyValue) where T : class;
        T FindEntity<T>(Expression<Func<T, bool>> condition) where T : class,new();
        IQueryable<T> IQueryable<T>() where T : class,new();
        IQueryable<T> IQueryable<T>(Expression<Func<T, bool>> condition) where T : class,new();
     
        IEnumerable<T> FindList<T>(string strSql) where T : class;
        IEnumerable<T> FindList<T>(string strSql, DbParameter[] dbParameter) where T : class;
        IEnumerable<T> FindList<T>(Pagination pagination) where T : class,new();
        IEnumerable<T> FindList<T>(Expression<Func<T, bool>> condition, Pagination pagination) where T : class,new();
        IEnumerable<T> FindList<T>(Expression<Func<T, bool>> condition) where T : class,new();
        IEnumerable<T> FindList<T>(string strSql, Pagination pagination) where T : class;
        IEnumerable<T> FindList<T>(string strSql, DbParameter[] dbParameter, Pagination pagination) where T : class;
        IEnumerable<T1> FindListForPage<T1>(string strSql, DbParameter[] dbParameter, Pagination pagination) where T1 : class;
        DataTable FindTable(string strSql);
        DataTable FindTable<T>(string strSql, T FindEntity) where T : class, new();
        DataTable FindTable(string strSql, DbParameter[] dbParameter);
        DataTable FindTable(string strSql, Pagination pagination);
        DataTable FindTable(string strSql, DbParameter[] dbParameter, Pagination pagination);
        object FindObject(string strSql);
        object FindObject(string strSql, DbParameter[] dbParameter);
        DataTable FindTable<T, FindT>(FindT FindEntity)
            where T : class, new()
            where FindT : class, new();
        DataTable FindTable<T, FindT>(FindT FindEntity, Pagination pagination, string appendSql = "")
            where T : class, new()
            where FindT : class, new();
        DataTable FindTable<T>(T entity);
        DataTable FindTable<T>(T entity, Pagination pagination);
        DataTable FindView<T>(T entity, Pagination pagination,string appendSql);
        DataTable FindView<T>(T entity, string querySql, string tablename);
        DataTable FindView<T>(T entity,Pagination pagination, string querySql, string tablename);
        DataTable FindFormatView<T>(T entity, string queryFormatSql, string tablename = "");
        DataTable FindFormatView<T>(T entity, Pagination pagination, string queryFormatSql, string tablename);
        DataTable ExecProcTable<T>(T entity, string procName, string appendSql, string tablename="");
        DataTable ExecProcTable<T>(T entity,string procName, Pagination pagination, string appendSql );
        void UpdateState<T>(int state, string key_value);
        void Insert(DataSet data);
        void Insert(DataTable Dt);
    }
}
