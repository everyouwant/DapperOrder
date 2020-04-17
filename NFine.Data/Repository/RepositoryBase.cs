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
    /// 描 述：定义仓储模型中的数据标准操作
    /// </summary>
    public class RepositoryBase : IRepositoryBase, IDisposable
    {

        #region 构造
        public IDatabase db = DbFactory.Base();
        //public RepositoryBase(IDatabase idatabase)
        //{
        //    this.db = idatabase;
        //}
        #endregion
        public void Dispose()
        {
            db.Dispose();
        }
        #region 事物提交
        public IRepositoryBase BeginTrans()
        {
            db.BeginTrans();
            return this;
        }
        public void Commit()
        {
            db.Commit();
        }
        public void Rollback()
        {
            db.Rollback();
        }
        #endregion

        #region 执行 SQL 语句
        public int ExecuteBySql(string strSql)
        {
            return db.ExecuteBySql(strSql);
        }
        public int ExecuteBySql(List<string> SQL) {
            return db.ExecuteBySql(SQL);
        }
        public int ExecuteBySql(string strSql, params DbParameter[] dbParameter)
        {
            return db.ExecuteBySql(strSql, dbParameter);
        }
        public int ExecuteByProc(string procName)
        {
            return db.ExecuteByProc(procName);
        }
        public int ExecuteByProc(string procName, params DbParameter[] dbParameter)
        {
            return db.ExecuteByProc(procName, dbParameter);
        }
        public string ExecuteByProcRes(string procName, params DbParameter[] dbParameter)
        {
            return db.ExecuteByProcRes(procName, dbParameter);
        }
        public int ExecuteByProcReturn(string procName, params DbParameter[] dbParameter)
        {
            return db.ExecuteByProcReturn(procName, dbParameter);
        }
        public IEnumerable<T> ExecuteByProcTable<T>(string procName, params DbParameter[] dbParameter)
        {
            return db.ExecuteByProcTable<T>(procName, dbParameter);
        }
        public DataTable ExecuteByProcReportTable(string procName, params DbParameter[] dbParameter)
        {
            return db.ExecuteByProcReportTable(procName, dbParameter);
        }

        public string FirstColRowValue(string SQL) {
            return db.FirstColRowValue(SQL);
        }
        #endregion

        #region 对象实体 添加、修改、删除
        public int Insert<T>(T entity, string NotUpdateColName="") where T : class
        {
            return db.Insert<T>(entity, NotUpdateColName);
        }
        public int Insert<T>(List<T> entity) where T : class
        {
            return db.Insert<T>(entity);
        }
        public int Delete<T>() where T : class
        {
            return db.Delete<T>();
        }
        public int Delete<T>(T entity) where T : class
        {
            return db.Delete<T>(entity);
        }
        public int Delete<T>(List<T> entity) where T : class
        {
            return db.Delete<T>(entity);
        }
        public int Delete<T>(Expression<Func<T, bool>> condition) where T : class,new()
        {
            return db.Delete<T>(condition);
        }
        public int Delete<T>(object keyValue) where T : class
        {
            return db.Delete<T>(keyValue);
        }
        public int Delete<T>(object[] keyValue) where T : class
        {
            return db.Delete<T>(keyValue);
        }
        public int Delete<T>(object propertyValue, string propertyName) where T : class
        {
            return db.Delete<T>(propertyValue, propertyName);
        }
        public int Update<T>(T entity) where T : class
        {
            return db.Update<T>(entity);
        }
        public int UpdateColNull<T>(T entity) where T : class {
            return db.UpdateColNull<T>(entity);
        }
        public int Update<T>(List<T> entity) where T : class
        {
            return db.Update<T>(entity);
        }
        public int Update<T>(Expression<Func<T, bool>> condition) where T : class,new()
        {
            return db.Update<T>(condition);
        }
        #endregion

        #region 对象实体 查询
        public T FindEntity<T>(object keyValue) where T : class
        {
            return db.FindEntity<T>(keyValue);
        }
        public T FindEntity<T>(Expression<Func<T, bool>> condition) where T : class,new()
        {
            return db.FindEntity<T>(condition);
        }
        public IQueryable<T> IQueryable<T>() where T : class,new()
        {
            return db.IQueryable<T>();
        }
        public IQueryable<T> IQueryable<T>(Expression<Func<T, bool>> condition) where T : class,new()
        {
            return db.IQueryable<T>(condition);
        }
        public IEnumerable<T> FindList<T>() where T : class,new()
        {
            return db.FindList<T>();
        }
        public IEnumerable<T> FindList<T>(Expression<Func<T, bool>> condition) where T : class,new()
        {
            return db.FindList<T>(condition);
        }
        public IEnumerable<T> FindList<T>(string strSql) where T : class
        {
            return db.FindList<T>(strSql);
        }
        public IEnumerable<T> FindList<T>(string strSql, DbParameter[] dbParameter) where T : class
        {
            return db.FindList<T>(strSql, dbParameter);
        }
        public IEnumerable<T> FindList<T>(Pagination pagination) where T : class,new()
        {
            int total = pagination.records;
            var data = db.FindList<T>(pagination.sidx, pagination.sord.ToLower() == "asc" ? true : false, pagination.rows, pagination.page, out total);
            pagination.records = total;
            return data;
        }
        public IEnumerable<T> FindList<T>(Expression<Func<T, bool>> condition, Pagination pagination) where T : class,new()
        {
            int total = pagination.records;
            var data = db.FindList<T>(condition, pagination.sidx, pagination.sord.ToLower() == "asc" ? true : false, pagination.rows, pagination.page, out total);
            pagination.records = total;
            return data;
        }
        public IEnumerable<T> FindList<T>(string strSql, Pagination pagination) where T : class
        {
            int total = pagination.records;
            var data = db.FindList<T>(strSql, pagination.sidx, pagination.sord.ToLower() == "asc" ? true : false, pagination.rows, pagination.page, out total);
            pagination.records = total;
            return data;
        }
        public IEnumerable<T> FindList<T>(string strSql, DbParameter[] dbParameter, Pagination pagination) where T : class
        {
            int total = pagination.records;
            var data = db.FindList<T>(strSql, dbParameter, pagination.sidx, pagination.sord.ToLower() == "asc" ? true : false, pagination.rows, pagination.page, out total);
            pagination.records = total;
            return data;
        }
        public IEnumerable<T1> FindListForPage<T1>(string strSql, DbParameter[] dbParameter, Pagination pagination) where T1 : class
        {
            int total = pagination.records;
            var data = db.FindList<T1>(strSql, dbParameter, pagination.sidx, pagination.sord.ToLower() == "asc" ? true : false, pagination.rows, pagination.page, out total);
            pagination.records = total;
            return data;
        }
        #endregion

        #region 数据源 查询
        public DataTable FindTable(string strSql)
        {
            return db.FindTable(strSql);
        }
        public DataTable FindTable(string strSql, DbParameter[] dbParameter)
        {
            return db.FindTable(strSql, dbParameter);
        }
        public DataTable FindTable(string strSql, Pagination pagination)
        {
            int total = pagination.records;
            var data = db.FindTable(strSql, pagination.sidx, pagination.sord.ToLower() == "asc" ? true : false, pagination.rows, pagination.page, out total);
            pagination.records = total;
            return data;
        }
        public DataTable FindTable(string strSql, DbParameter[] dbParameter, Pagination pagination)
        {
            int total = pagination.records;
            var data = db.FindTable(strSql, dbParameter, pagination.sidx, pagination.sord.ToLower() == "asc" ? true : false, pagination.rows, pagination.page, out total);
            pagination.records = total;
            return data;
        }
        public object FindObject(string strSql)
        {
            return db.FindObject(strSql);
        }
        public object FindObject(string strSql, DbParameter[] dbParameter)
        {
            return db.FindObject(strSql, dbParameter);
        }

        public DataTable FindTable<T>(T entity)
        {
            return db.FindTable(DatabaseCommon.QueryWhereSQL<T>(entity).ToString());
        }
        public DataTable FindTable<T>(string strSql, T FindEntity) where T : class, new()
        {
            return db.FindTable(strSql, FindEntity);
        }
        public DataTable FindTable<T>(T entity, Pagination pagination)
        {
            int total = pagination.records;
            if (pagination.sord==null)
            {
                pagination.sord = "asc";
            }
            var data = db.FindTable(DatabaseCommon.QueryWhereSQL<T>(entity).ToString(), pagination.sidx, pagination.sord.ToLower() == "asc" ? true : false, pagination.rows, pagination.page, out total);
            pagination.records = total;
            return data;
        }
        public DataTable FindTable<T, FindT>(FindT FindEntity)where T : class, new()where FindT : class, new()
        {
            return FindTable<T, FindT>(FindEntity);
        }

        public DataTable FindTable<T, FindT>(FindT FindEntity, Pagination pagination, string appendSql = "") where T : class, new() where FindT : class, new()
        {
            int total = pagination.records;
            pagination.records = total;
            var data = db.FindTable(FindEntity, pagination.sidx, pagination.sord.ToLower() == "asc" ? true : false, pagination.rows, pagination.page, out total);
            return data;
        }
        public DataTable FindView<T>(T entity, Pagination pagination, string appendSql)
        {
            int total = pagination.records;
            var data = db.FindView(entity, appendSql, pagination.sidx, pagination.sord.ToLower() == "asc" ? true : false, pagination.rows, pagination.page, out total);
            pagination.records = total;
            return data;
        }
        public DataTable FindView<T>(T entity, string querySql, string tablename)
        {
            return db.FindView(entity, querySql, tablename);
        }
        public DataTable FindView<T>(T entity, Pagination pagination, string querySql, string tablename)
        {
            int total = pagination.records;
            var data = db.FindView(entity, querySql, tablename, pagination.sidx, pagination.sord.ToLower() == "asc" ? true : false, pagination.rows, pagination.page, out total);
            pagination.records = total;
            return data;
        }
        public DataTable FindFormatView<T>(T entity, string queryFormatSql, string tablename="")
        {
            var data = db.FindFormatView(entity, queryFormatSql, tablename);
            return data;
        }
        public DataTable FindFormatView<T>(T entity, Pagination pagination, string queryFormatSql, string tablename)
        {
            int total = pagination.records;
            var data = db.FindFormatView(entity, queryFormatSql, tablename, pagination.sidx, pagination.sord.ToLower() == "asc" ? true : false, pagination.rows, pagination.page, out total);
            pagination.records = total;
            return data;
        }
        public DataTable ExecProcTable<T>(T entity, string procName, string appendSql, string tablename)
        {
            Type type = entity.GetType();
            appendSql += DatabaseCommon.GetQueryWhereString<T>(entity, tablename, type, null, true).ToString();
            return db.ExecProcTable(procName, appendSql);
        }
        public DataTable ExecProcTable<T>(T entity, string procName, Pagination pagination, string appendSql )
        {
            int total = pagination.records;
            Type type = entity.GetType();
            appendSql += DatabaseCommon.GetQueryWhereString<T>(entity,"T",type,null,true).ToString();
            var data = db.ExecProcTable(procName, pagination.sidx, pagination.sord.ToLower() == "asc" ? true : false, pagination.rows, pagination.page, out total, appendSql);
            pagination.records = total;
            return data;
        }

        #endregion
        public void Insert(DataSet data)
        {
            db.Insert(data);
        }
        public void Insert(DataTable Dt) {
            db.Insert(Dt);
        }
        #region 更新可用状态
        public void UpdateState<T>(int state, string key_value)
        {
            db.UpdateState<T>(state, key_value);
        }




        #endregion




















    }
}
