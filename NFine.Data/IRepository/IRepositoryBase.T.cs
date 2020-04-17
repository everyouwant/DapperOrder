using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using NFine.Code;

namespace NFine.Data
{
    /// <summary>
    /// 版 本 6.1
    /// Copyright (c) 2017 重庆万紫科技有限公司
    /// 创建人：YongTony
    /// 日 期：2015.10.10
    /// 描 述：定义仓储模型中的数据标准操作接口
    /// </summary>
    /// <typeparam name="T">动态实体类型</typeparam>
    public interface IRepositoryBase<T> where T : class, new()
    {
        IRepositoryBase<T> BeginTrans();
        void Commit();
        void Rollback();

        int ExecuteBySql(string strSql);
        int ExecuteBySql(List<string> strSql);
        int ExecuteBySql(string strSql, params DbParameter[] dbParameter);
        int ExecuteByProc(string procName);
        int ExecuteByProc(string procName, params DbParameter[] dbParameter);
        string ExecuteByProcRes(string procName, params DbParameter[] dbParameter);
        int ExecuteByProcReturn(string procName, params DbParameter[] dbParameter);
        IEnumerable<T> ExecuteByProcTable<T>(string procName, params DbParameter[] dbParameter);
        DataTable ExecuteByProcReportTable(string procName, params DbParameter[] dbParameter);
        string FirstColRowValue(string SQL);
        int Insert(T entity);
        int Insert(List<T> entity);

        void Insert(DataSet dataset);
        void Insert(DataTable datatable);
        int Delete();
        int Delete(T entity);
        int Delete(List<T> entity);
        int Delete(Expression<Func<T, bool>> condition);
        int Delete(object keyValue);
        int Delete(object[] keyValue);
        int Delete(object propertyValue, string propertyName);
        int Update(T entity);
        int Update(List<T> entity);
        int Update(Expression<Func<T, bool>> condition);
        DataTable FindView<T>(T entity, string querySql, string tablename);
        DataTable FindView<T>(T entity, Pagination pagination, string appendSql);
        DataTable FindFormatView<T>(T entity, Pagination pagination, string queryFormatSql, string tablename);

        T FindEntity(object keyValue);
        T FindEntity(Expression<Func<T, bool>> condition);
        IQueryable<T> IQueryable();
        IQueryable<T> IQueryable(Expression<Func<T, bool>> condition);
        IEnumerable<T> FindList(string strSql);
        IEnumerable<T> FindList(string strSql, DbParameter[] dbParameter);
        IEnumerable<T> FindList(Pagination pagination);
        IEnumerable<T> FindList(Expression<Func<T, bool>> condition, Pagination pagination);
        IEnumerable<T> FindList(string strSql, Pagination pagination);
        IEnumerable<T> FindList(string strSql, DbParameter[] dbParameter, Pagination pagination);
        IEnumerable<T1> FindList<FindT, T1>(FindT FindEntity, string strSql, DbParameter[] dbParameter, Pagination pagination)
            where FindT : class, new()
            where T1 : class, new();

        DataTable FindTable(string strSql);
        DataTable FindTable<T>(string strSql, T FindEntity) where T : class, new();
        DataTable FindTable(string strSql, DbParameter[] dbParameter);
        DataTable FindTable(string strSql, Pagination pagination);
        DataTable FindTable(string strSql, DbParameter[] dbParameter, Pagination pagination);
        object FindObject(string strSql);
        object FindObject(string strSql, DbParameter[] dbParameter);
        DataTable FindTable<T>(T entity);
        DataTable FindTable<T>(T entity, Pagination pagination);
        DataTable FindTable<T, FindT>(FindT FindEntity)
            where T : class, new()
            where FindT : class, new();
        DataTable FindTable<T, FindT>(FindT FindEntity, Pagination pagination,string appendSql="")
            where T : class, new()
            where FindT : class, new();
        void UpdateState(int state, string key_value);
        DataTable ExecProcTable<T>(T entity,string procName, string appendSql );
        DataTable ExecProcTable<T>(T entity,string procName, Pagination pagination, string appendSql  );
    }
}
