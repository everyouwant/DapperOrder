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
    /// 描 述：定义仓储模型中的数据标准操作
    /// </summary>
    /// <typeparam name="TEntity">动态实体类型</typeparam>
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class, new()
    {
        #region 构造
        public IDatabase db= DbFactory.Base();
        //public Repository(IDatabase idatabase)
        //{
        //    this.db = idatabase;
        //}
        #endregion

        #region 事物提交
        public IRepositoryBase<T> BeginTrans()
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
        public int ExecuteBySql(List<string> SQL)
        {
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

        public string FirstColRowValue(string SQL)
        {
            return db.FirstColRowValue(SQL);
        }
        #endregion

        #region 对象实体 添加、修改、删除
        public int Insert(T entity)
        {
            return db.Insert<T>(entity);
        }
        public int Insert(List<T> entity)
        {
            return db.Insert<T>(entity);
        }
        public void Insert(DataSet dataset) {
            db.Insert(dataset);
        }
        public void Insert(DataTable datatable)
        {
            db.Insert(datatable);
        }
        public int Delete()
        {
            return db.Delete<T>();
        }
        public int Delete(T entity)
        {
            return db.Delete<T>(entity);
        }
        public int Delete(List<T> entity)
        {
            return db.Delete<T>(entity);
        }
        public int Delete(Expression<Func<T, bool>> condition)
        {
            return db.Delete<T>(condition);
        }
        public int Delete(object keyValue)
        {
            return db.Delete<T>(keyValue);
        }
        public int Delete(object[] keyValue)
        {
            return db.Delete<T>(keyValue);
        }
        public int Delete(object propertyValue, string propertyName)
        {
            return db.Delete<T>(propertyValue, propertyName);
        }
        public int Update(T entity)
        {
            return db.Update<T>(entity);
        }
        public int Update(List<T> entity)
        {
            return db.Update<T>(entity);
        }
        public int Update(Expression<Func<T, bool>> condition)
        {
            return db.Update<T>(condition);
        }
        #endregion

        #region 对象实体 查询
        public T FindEntity(object keyValue)
        {
            return db.FindEntity<T>(keyValue);
        }
        public T FindEntity(Expression<Func<T, bool>> condition)
        {
            return db.FindEntity<T>(condition);
        }
        public IQueryable<T> IQueryable()
        {
            return db.IQueryable<T>();
        }
        public IQueryable<T> IQueryable(Expression<Func<T, bool>> condition)
        {
            return db.IQueryable<T>(condition);
        }
        public IEnumerable<T> FindList(string strSql)
        {
            return db.FindList<T>(strSql);
        }
        public IEnumerable<T> FindList(string strSql, DbParameter[] dbParameter)
        {
            return db.FindList<T>(strSql, dbParameter);
        }
        public IEnumerable<T> FindList(Pagination pagination)
        {
            int total = pagination.records;
            var data = db.FindList<T>(pagination.sidx, pagination.sord.ToLower() == "asc" ? true : false, pagination.rows, pagination.page, out total);
            pagination.records = total;
            return data;
        }
        public IEnumerable<T> FindList(Expression<Func<T, bool>> condition, Pagination pagination)
        {
            int total = pagination.records;
            var data = db.FindList<T>(condition, pagination.sidx, pagination.sord.ToLower() == "asc" ? true : false, pagination.rows, pagination.page, out total);
            pagination.records = total;
            return data;
        }
       
        public IEnumerable<T> FindList(string strSql, Pagination pagination)
        {
            int total = pagination.records;
            var data = db.FindList<T>(strSql, pagination.sidx, pagination.sord.ToLower() == "asc" ? true : false, pagination.rows, pagination.page, out total);
            pagination.records = total;
            return data;
        }
        public IEnumerable<T> FindList(string strSql, DbParameter[] dbParameter, Pagination pagination)
        {
            int total = pagination.records;
            var data = db.FindList<T>(strSql, dbParameter, pagination.sidx, pagination.sord.ToLower() == "asc" ? true : false, pagination.rows, pagination.page, out total);
            pagination.records = total;
            return data;
        }
        public IEnumerable<T1> FindList<FindT, T1>(FindT FindEntity, string strSql, DbParameter[] dbParameter, Pagination pagination)
            where FindT : class, new()
            where T1 : class, new()
        {
            int total = pagination.records;
            if (pagination.sord==null)
            {
                pagination.sord = "ASC";
            }
            var data = db.FindList<T1, FindT>(FindEntity, strSql, dbParameter, pagination.sidx, pagination.sord.ToLower() == "asc" ? true : false, pagination.rows, pagination.page, out total);
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
            if (entity == null)
            {
                return db.FindTable(DatabaseCommon.SelectSql(EntityAttribute.GetEntityTable<T>()));
            }
            else
            {
                return db.FindTable(DatabaseCommon.QueryWhereSQL<T>(entity).ToString());
            }
        }
        public DataTable FindTable<T>(T entity, Pagination pagination)
        {
            DataTable data;
            if (entity != null && pagination != null)
            {
                int total = pagination.records;
                if (pagination.sord==null)
                {
                    pagination.sord = "asc";
                }
                data = db.FindTable(DatabaseCommon.QueryWhereSQL<T>(entity).ToString(), pagination.sidx, pagination.sord.ToLower() == "asc" ? true : false, pagination.rows, pagination.page, out total);
                pagination.records = total;
            }
            else if (entity != null && pagination == null)
            {
                data = db.FindTable(DatabaseCommon.QueryWhereSQL<T>(entity).ToString());
            }
            else
            {
                data = db.FindTable(DatabaseCommon.SelectSql(EntityAttribute.GetEntityTable<T>()));
            }
            return data;
        }

        public DataTable FindView<T>(T entity, Pagination pagination, string appendSql)
        {
            int total = pagination.records;
            var data = db.FindView(entity, appendSql, pagination.sidx, pagination.sord.ToLower() == "asc" ? true : false, pagination.rows, pagination.page, out total);
            pagination.records = total;
            return data;
        }
        public DataTable FindTable<T, FindT>(FindT FindEntity)
            where T : class, new()
            where FindT : class, new()
        {
            return db.FindTable<T, FindT>(FindEntity);
        }

        public DataTable FindTable<T, FindT>(FindT FindEntity, Pagination pagination, string appendSql = "")
            where T : class, new()
            where FindT : class, new()
        {
            DataTable dt = new DataTable();
            int total = pagination.records;
            dt = db.FindTable<T, FindT>(FindEntity, pagination.sidx, pagination.sord.ToLower() == "asc" ? true : false, pagination.rows, pagination.page, out total, appendSql);
            pagination.records = total;
            return dt;
        }
        public DataTable FindTable<T>(string strSql, T FindEntity) where T : class, new()
        {
            return db.FindTable(strSql, FindEntity);
        }
        public DataTable FindView<T>(T entity, string querySql, string tablename)
        {
            return db.FindView(entity, querySql, tablename);
        }
        public DataTable FindView<T>(T entity, Pagination pagination, string querySql, string tablename)
        {
            DataTable dt = new DataTable();
            int total = pagination.records;
            dt =db.FindView(entity, querySql, tablename, pagination.sidx, pagination.sord.ToLower() == "asc" ? true : false, pagination.rows, pagination.page, out total);
            pagination.records = total;
            return dt;
        }
        public DataTable FindFormatView<T>(T entity, Pagination pagination, string queryFormatSql, string tablename)
        {
            int total = pagination.records;
            var data = db.FindFormatView(entity, queryFormatSql, tablename, pagination.sidx, pagination.sord.ToLower() == "asc" ? true : false, pagination.rows, pagination.page, out total);
            pagination.records = total;
            return data;
        }
        public DataTable ExecProcTable<T>(T entity, string procName, string appendSql )
        {
            Type type = entity.GetType();
            appendSql += DatabaseCommon.GetQueryWhereString<T>(entity, "T", type, null, true).ToString();
            return db.ExecProcTable(procName, appendSql);
        }
        public DataTable ExecProcTable<T>(T entity, string procName, Pagination pagination, string appendSql  )
        {
            int total = pagination.records;
            appendSql += DatabaseCommon.QueryWhereSQL<T>(entity).ToString();
            var data = db.ExecProcTable(procName, pagination.sidx, pagination.sord.ToLower() == "asc" ? true : false, pagination.rows, pagination.page, out total, appendSql);
            pagination.records = total;
            return data;
        }
        #endregion

        #region 更新可用状态 
        public void UpdateState(int state, string key_value) 
        {
             db.UpdateState<T>(state, key_value);
        }

   
        #endregion





    }
}
