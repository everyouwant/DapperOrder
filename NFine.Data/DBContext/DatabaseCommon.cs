using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using Dapper;
using System.Linq.Expressions;
using NFine.Code;

namespace NFine.Data
{
    public class DatabaseCommon
    {

        #region 对象参数转换DbParameter
        /// <summary>
        /// Dapper对象参数转换DynamicParameters
        /// </summary>
        /// <returns></returns>
        public static DynamicParameters GetDynamicParameter<T>(T entity, string NotUpdateColName="")
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            Type type = entity.GetType();
            PropertyInfo[] props = type.GetProperties();
            foreach (PropertyInfo pi in props)
            {
                if (pi.GetValue(entity, null) != null)
                {
                    DbParameters.AddDynamicParameter(dynamicParameters, DbParameters.CreateDbParmCharacter() + pi.Name, pi.GetValue(entity, null));
                }
            }
            return dynamicParameters;
        }

        /// <summary>
        ///DbParameter参数转换SqlDynamicParameters
        /// </summary>
        /// <returns></returns>
        public static DynamicParameters GetDynamicParameter(DbParameter[] dbParameter)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            foreach (DbParameter dp in dbParameter)
            {
                dynamicParameters.Add(dp.ParameterName, dp.Value, dp.DbType);
            }
            return dynamicParameters;
        }
        /// <summary>
        /// 对象参数转换DbParameter
        /// </summary>
        /// <returns></returns>
        public static DbParameter[] GetParameter<T>(T entity)
        {
            IList<DbParameter> parameter = new List<DbParameter>();
            DbType dbtype = new DbType();
            Type type = entity.GetType();
            PropertyInfo[] props = type.GetProperties();
            foreach (PropertyInfo pi in props)
            {
                if (pi.GetValue(entity, null) != null)
                {
                    switch (pi.PropertyType.ToString())
                    {
                        case "System.Nullable`1[System.Int32]":
                            dbtype = DbType.Int32;
                            break;
                        case "System.Nullable`1[System.Decimal]":
                            dbtype = DbType.Decimal;
                            break;
                        case "System.Nullable`1[System.DateTime]":
                            dbtype = DbType.DateTime;
                            break;
                        default:
                            dbtype = DbType.String;
                            break;
                    }
                    parameter.Add(DbParameters.CreateDbParameter(DbParameters.CreateDbParmCharacter() + pi.Name, pi.GetValue(entity, null), dbtype));
                }
            }
            return parameter.ToArray();
        }
        public static DbParameter[] GetColNullParameter<T>(T entity)
        {
            IList<DbParameter> parameter = new List<DbParameter>();
            DbType dbtype = new DbType();
            Type type = entity.GetType();
            PropertyInfo[] props = type.GetProperties();
            foreach (PropertyInfo pi in props)
            {
                
                    switch (pi.PropertyType.ToString())
                    {
                        case "System.Nullable`1[System.Int32]":
                            dbtype = DbType.Int32;
                            break;
                        case "System.Nullable`1[System.Decimal]":
                            dbtype = DbType.Decimal;
                            break;
                        case "System.Nullable`1[System.DateTime]":
                            dbtype = DbType.DateTime;
                            break;
                        default:
                            dbtype = DbType.String;
                            break;
                    }
                    parameter.Add(DbParameters.CreateDbParameter(DbParameters.CreateDbParmCharacter() + pi.Name, pi.GetValue(entity, null), dbtype));
            }
            return parameter.ToArray();
        }
        /// <summary>
        /// 对象参数转换DbParameter
        /// </summary>
        /// <returns></returns>
        public static DbParameter[] GetParameter(Hashtable ht)
        {
            IList<DbParameter> parameter = new List<DbParameter>();
            DbType dbtype = new DbType();
            foreach (string key in ht.Keys)
            {
                if (ht[key] is DateTime)
                    dbtype = DbType.DateTime;
                else
                    dbtype = DbType.String;
                parameter.Add(DbParameters.CreateDbParameter(DbParameters.CreateDbParmCharacter() + key, ht[key], dbtype));
            }
            return parameter.ToArray();
        }
        #endregion

        #region 获取实体类自定义信息

        // <summary>
        /// 获取实体集的可用标识字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static string GetEnableColumn<T>()
        {
            Type objTye = typeof(T);
            string custmerEnableColumnInfo = string.Empty;
            string enumEnableColumnInfo = string.Empty;
            bool _break=false;
            objTye.GetProperties().ToList().ForEach(x =>
            {
                if (_break)
                {
                    return;
                }
                EnableStateAttribute attr = x.GetCustomAttributes(typeof(EnableStateAttribute), true).FirstOrDefault() as EnableStateAttribute;
                if (attr != null)
                {
                     custmerEnableColumnInfo = x.Name;
                    _break = true;
                }
                if (x.Name.ToUpper().Contains("ENABLE"))
                {
                    enumEnableColumnInfo = x.Name;
                }
            });
            return string.IsNullOrEmpty(custmerEnableColumnInfo) ? enumEnableColumnInfo : custmerEnableColumnInfo;
        }

        /// <summary>
        /// 获取实体集需要翻译字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static List<string[]> GetTranslateValue<T>()
        {
            Type objTye = typeof(T);
            List<string[]> TranslateInfo = new List<string[]>();
            objTye.GetProperties().ToList().ForEach(x =>
                {
                    TranslateAttribute attr = x.GetCustomAttributes(typeof(TranslateAttribute), true).FirstOrDefault() as TranslateAttribute;
                    if (attr != null)
                    {
                        TranslateInfo.Add(new string[] { x.Name, attr.mainTable, attr.originalField, attr.newField });
                    }
                });
            return TranslateInfo;
        }
        

        /// <summary>
        /// 获取实体类主键字段
        /// </summary>
        /// <returns></returns>
        public static object GetKeyField<T>()
        {
            Type objTye = typeof(T);
            string _KeyField = "";
            PrimaryKeyAttribute KeyField;
            var name = objTye.Name;
            foreach (Attribute attr in objTye.GetCustomAttributes(true))
            {
                KeyField = attr as PrimaryKeyAttribute;
                if (KeyField != null)
                    _KeyField = KeyField.Name;
            }
            return _KeyField;
        }
        /// <summary>
        /// 获取实体类主键字段
        /// </summary>
        /// <returns></returns>
        public static string [] GetEditColArrName<T>()
        {
            Type objTye = typeof(T);
            string arrColName = "";
            EditNullAttribute KeyField;
            var name = objTye.Name;
            foreach (Attribute attr in objTye.GetCustomAttributes(true))
            {
                KeyField = attr as EditNullAttribute;
                if (KeyField != null)
                    arrColName = KeyField.EditColNullName;
            }
            return arrColName.Split(',');
        }
        /// <summary>
        /// 获取实体类主键字段
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns></returns>
        public static object GetKeyFieldValue<T>(T entity)
        {
            Type objTye = typeof(T);
            string _KeyField = "";
            PrimaryKeyAttribute KeyField;
            var name = objTye.Name;
            foreach (Attribute attr in objTye.GetCustomAttributes(true))
            {
                KeyField = attr as PrimaryKeyAttribute;
                if (KeyField != null)
                    _KeyField = KeyField.Name;
            }
            PropertyInfo property = objTye.GetProperty(_KeyField);
            return property.GetValue(entity, null).ToString();
        }
        /// <summary>
        /// 获取实体类 字段中文名称
        /// </summary>
        /// <param name="pi">字段属性信息</param>
        /// <returns></returns>
        public static string GetFieldText(PropertyInfo pi)
        {
            DisplayNameAttribute descAttr;
            string txt = "";
            var descAttrs = pi.GetCustomAttributes(typeof(DisplayNameAttribute), true);
            if (descAttrs.Any())
            {
                descAttr = descAttrs[0] as DisplayNameAttribute;
                txt = descAttr.DisplayName;
            }
            else
            {
                txt = pi.Name;
            }
            return txt;
        }
        /// <summary>
        /// 获取实体类中文名称
        /// </summary>
        /// <returns></returns>
        public static string GetClassName<T>()
        {
            Type objTye = typeof(T);
            string entityName = "";
            var busingessNames = objTye.GetCustomAttributes(true).OfType<DisplayNameAttribute>();
            var descriptionAttributes = busingessNames as DisplayNameAttribute[] ?? busingessNames.ToArray();
            if (descriptionAttributes.Any())
                entityName = descriptionAttributes.ToList()[0].DisplayName;
            else
            {
                entityName = objTye.Name;
            }
            return entityName;
        }
        #endregion

        #region 按 操作类型 拼接SQL语句

        /// <summary>
        /// 执行表达式
        /// </summary>
        /// <param name="exp">表达式</param>
        /// <param name="operateType">操作类型</param>
        /// <param name="columnName">列名</param>
        /// <param name="newValue">值</param>
        /// <returns>int</returns>
        public static StringBuilder DealExp<T>(Expression exp, OperateType operateType, List<string> columnName = null, List<string> newValue = null)
        {
            StringBuilder sb = new StringBuilder();
            string TableName = EntityAttribute.GetEntityTable<T>();
            switch (operateType)
            {
                case OperateType.Select:
                    sb.Append("Select ");
                    sb.Append(GetConditionString(columnName));
                    sb.Append(" From ");
                    sb.Append(TableName);
                    sb.Append(" Where ");
                    break;
                case OperateType.Delete:
                    sb.Append("Delete ");
                    sb.Append(TableName);
                    sb.Append(" Where ");
                    break;
                case OperateType.Update:
                    sb.Append("Update ");
                    sb.Append(TableName);
                    sb.Append("  Set(");
                    if (columnName != null && newValue != null && columnName.Count == newValue.Count && columnName.Count > 0)
                    {
                        for (int i = 0; i < columnName.Count; i++)
                        {
                            sb.Append(columnName[i]);
                            sb.Append("=");
                            sb.Append(newValue[i]);
                        }
                    }
                    sb.Append(") Where ");
                    break;
                case OperateType.Insert:
                    sb.Append("Insert Into ");
                    sb.Append(TableName);
                    sb.Append(GetConditionString(columnName).Length > 0 ? string.Format("({0})", GetConditionString(columnName)) : string.Empty);
                    sb.Append(" Values( ");
                    sb.Append(GetConditionString(newValue));
                    sb.Append(") Where ");
                    break;
            }
            if (ExpToSqlHelper.DealExpress(exp).Substring(0, 4).ToUpper().Equals("TRUE"))
            {
                sb.Append(" 1=1 ");
                sb.Append(ExpToSqlHelper.DealExpress(exp).Remove(0, 4));
            }
            else
            {
                sb.Append(ExpToSqlHelper.DealExpress(exp));
            }
            return sb;
        }

        #endregion

        #region 拼接 Insert SQL语句

        /// <summary>
        /// 哈希表生成Insert语句
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="ht">Hashtable</param>
        /// <returns>int</returns>
        public static StringBuilder InsertSql(string tableName, Hashtable ht)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" Insert Into ");
            sb.Append(tableName);
            sb.Append("(");
            StringBuilder sp = new StringBuilder();
            StringBuilder sb_prame = new StringBuilder();
            foreach (string key in ht.Keys)
            {
                if (ht[key] != null)
                {
                    sb_prame.Append("," + key);
                    sp.Append("," + DbParameters.CreateDbParmCharacter() + "" + key);
                }
            }
            sb.Append(sb_prame.ToString().Substring(1, sb_prame.ToString().Length - 1) + ") Values (");
            sb.Append(sp.ToString().Substring(1, sp.ToString().Length - 1) + ")");
            return sb;
        }

        /// <summary>
        /// 泛型方法，反射生成InsertSql语句
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns>int</returns>
        public static StringBuilder InsertSql<T>(T entity, string NotUpdateColName="")
        {
            Type type = entity.GetType();
            StringBuilder sb = new StringBuilder();
            sb.Append(" Insert Into ");
            sb.Append(EntityAttribute.GetEntityTable<T>());
            sb.Append("(");
            StringBuilder sp = new StringBuilder();
            StringBuilder sb_prame = new StringBuilder();
            PropertyInfo[] props = type.GetProperties();
            foreach (PropertyInfo prop in props)
            {
                if (prop.GetValue(entity, null) != null && !NotUpdateColName.Split(',').Contains(prop.Name))
                {
                    sb_prame.Append("," + (prop.Name));
                    sp.Append("," + DbParameters.CreateDbParmCharacter() + "" + (prop.Name));
                }
            }
            sb.Append(sb_prame.ToString().Substring(1, sb_prame.ToString().Length - 1) + ") Values (");
            sb.Append(sp.ToString().Substring(1, sp.ToString().Length - 1) + ")");
            return sb;
        }

        #endregion

        #region 拼接 Update SQL语句
        /// <summary>
        /// 哈希表生成UpdateSql语句
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="ht">Hashtable</param>
        /// <param name="pkName">主键</param>
        /// <returns></returns>
        public static StringBuilder UpdateSql(string tableName, Hashtable ht, string pkName)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" Update ");
            sb.Append(tableName);
            sb.Append(" Set ");
            bool isFirstValue = true;
            foreach (string key in ht.Keys)
            {
                if (ht[key] != null && pkName != key)
                {
                    if (isFirstValue)
                    {
                        isFirstValue = false;
                        sb.Append(key);
                        sb.Append("=");
                        sb.Append(DbParameters.CreateDbParmCharacter() + key);
                    }
                    else
                    {
                        sb.Append("," + key);
                        sb.Append("=");
                        sb.Append(DbParameters.CreateDbParmCharacter() + key);
                    }
                }
            }
            sb.Append(" Where ").Append(pkName).Append("=").Append(DbParameters.CreateDbParmCharacter() + pkName);
            return sb;
        }
        /// <summary>
        /// 泛型方法，反射生成UpdateSql语句
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <param name="pkName">主键</param>
        /// <returns>int</returns>
        public static StringBuilder UpdateSql<T>(T entity, string pkName)
        {
            Type type = entity.GetType();
            PropertyInfo[] props = type.GetProperties();
            StringBuilder sb = new StringBuilder();
            sb.Append(" Update ");
            sb.Append(EntityAttribute.GetEntityTable<T>());
            sb.Append(" Set ");
            bool isFirstValue = true;
            foreach (PropertyInfo prop in props)
            {
                if (prop.GetValue(entity, null) != null && GetKeyField<T>().ToString() != prop.Name)
                {
                    if (isFirstValue)
                    {
                        isFirstValue = false;
                        sb.Append(prop.Name);
                        sb.Append("=");
                        sb.Append(DbParameters.CreateDbParmCharacter() + prop.Name);
                    }
                    else
                    {
                        sb.Append("," + prop.Name);
                        sb.Append("=");
                        sb.Append(DbParameters.CreateDbParmCharacter() + prop.Name);
                    }
                }
            }
            sb.Append(" Where ").Append(pkName).Append("=").Append(DbParameters.CreateDbParmCharacter() + pkName);
            return sb;
        }
        /// <summary>
        /// 泛型方法，反射生成UpdateSql语句
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns>int</returns>
        public static StringBuilder UpdateSql<T>(T entity)
        {
            string pkName = GetKeyField<T>().ToString();
            Type type = entity.GetType();
            PropertyInfo[] props = type.GetProperties();
            StringBuilder sb = new StringBuilder();
            sb.Append("Update ");
            sb.Append(EntityAttribute.GetEntityTable<T>());
            sb.Append(" Set ");
            bool isFirstValue = true;
            foreach (PropertyInfo prop in props)
            {
                if (prop.GetValue(entity, null) != null && pkName != prop.Name)
                {
                    if (isFirstValue)
                    {
                        isFirstValue = false;
                        sb.Append(prop.Name);
                        sb.Append("=");
                        sb.Append(DbParameters.CreateDbParmCharacter() + prop.Name);
                    }
                    else
                    {
                        sb.Append("," + prop.Name);
                        sb.Append("=");
                        sb.Append(DbParameters.CreateDbParmCharacter() + prop.Name);
                    }
                }
            }
            sb.Append(" Where ").Append(pkName).Append("=").Append(DbParameters.CreateDbParmCharacter() + pkName);
            return sb;
        }


        /// <summary>
        /// 泛型方法，反射生成UpdateSql语句
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns>int</returns>
        public static StringBuilder UpdateNullSql<T>(T entity)
        {
            string pkName = GetKeyField<T>().ToString();
            string[] EditColName = GetEditColArrName<T>();
            Type type = entity.GetType();
            PropertyInfo[] props = type.GetProperties();
            StringBuilder sb = new StringBuilder();
            sb.Append("Update ");
            sb.Append(EntityAttribute.GetEntityTable<T>());
            sb.Append(" Set ");
            bool isFirstValue = true;
            foreach (PropertyInfo prop in props)
            {
                if ((prop.GetValue(entity, null) != null && pkName != prop.Name) || EditColName.Contains(prop.Name))
                {
                    string ColValue = Convert.ToString(type.GetProperty(prop.Name).GetValue(entity, null));
                    if (isFirstValue)
                    {
                        isFirstValue = false;
                       
                    }
                    else
                    {
                        sb.Append(",");
                      
                    }
                    if (string.IsNullOrWhiteSpace(ColValue) && EditColName.Contains(prop.Name))
                    {
                        sb.Append(prop.Name);
                        sb.Append("=null");
                    }
                    else {
                        sb.Append(prop.Name);
                        sb.Append("=");
                        sb.Append("'"+ColValue+"'");
                    }
                    
                }
                
            }
            sb.Append(" Where ").Append(pkName).Append("=").Append("'"+Convert.ToString(type.GetProperty(pkName).GetValue(entity, null))+"'");
            return sb;
        }
        #endregion

        #region 拼接 Delete SQL语句
        /// <summary>
        /// 拼接删除SQL语句
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="pkName">字段主键</param>
        /// <returns></returns>
        public static StringBuilder DeleteSql(string tableName)
        {
            return new StringBuilder("Delete From " + tableName);
        }
        /// <summary>
        /// 拼接删除SQL语句
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="pkName">字段主键</param>
        /// <returns></returns>
        public static StringBuilder DeleteSql(string tableName, string pkName)
        {
            return new StringBuilder("Delete From " + tableName + " Where " + pkName + " = " + DbParameters.CreateDbParmCharacter() + pkName + "");
        }
        /// <summary>
        /// 拼接删除SQL语句
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="ht">多参数</param>
        /// <returns></returns>
        public static StringBuilder DeleteSql(string tableName, Hashtable ht)
        {
            StringBuilder sb = new StringBuilder("Delete From " + tableName + " Where 1=1");
            foreach (string key in ht.Keys)
            {
                sb.Append(" AND " + key + " = " + DbParameters.CreateDbParmCharacter() + "" + key + "");
            }
            return sb;
        }
        /// <summary>
        /// 拼接删除SQL语句
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns></returns>
        public static StringBuilder DeleteSql<T>(T entity)
        {
            Type type = entity.GetType();
            PropertyInfo[] props = type.GetProperties();
            StringBuilder sb = new StringBuilder("Delete From " + EntityAttribute.GetEntityTable<T>() + " Where 1=1");
            foreach (PropertyInfo prop in props)
            {
                if (prop.GetValue(entity, null) != null)
                {
                    sb.Append(" AND " + prop.Name + " = " + DbParameters.CreateDbParmCharacter() + "" + prop.Name + "");
                }
            }
            return sb;
        }
        #endregion

        #region 拼接 Select SQL语句
        /// <summary>
        /// 拼接 查询 SQL语句
        /// </summary>
        /// <returns></returns>
        public static StringBuilder SelectSql<T>() where T : new()
        {
            string tableName = typeof(T).Name;
            PropertyInfo[] props = GetProperties(new T().GetType());
            StringBuilder sbColumns = new StringBuilder();
            foreach (PropertyInfo prop in props)
            {
                string propertytype = prop.PropertyType.ToString();
                sbColumns.Append(prop.Name + ",");
            }
            if (sbColumns.Length > 0) sbColumns.Remove(sbColumns.ToString().Length - 1, 1);
            string strSql = "SELECT {0} FROM {1} WHERE 1=1 ";
            strSql = string.Format(strSql, sbColumns.ToString(), tableName + " ");
            return new StringBuilder(strSql);
        }

        public static StringBuilder GetQueryWhereString<T>(T entity, string tableName, Type type, StringBuilder sb,bool execsql=false)
        {
            if (sb==null)
            {
                sb = new StringBuilder();
            }
            if (!string.IsNullOrWhiteSpace(tableName)) {
                tableName = tableName + ".";
            }
            foreach (PropertyInfo item in GetProperties(type))
            {
                var ItemValue = item.GetValue(entity, null);
                if (ItemValue != null)
                {
                    if (item.PropertyType == typeof(string))
                    {
                        //判断是否间段查询
                        if (item.Name.IndexOf("Smt_End_") == 0 || item.Name.IndexOf("Smt_Start_") == 0)
                        {
                            string execsqlReplace=execsql?"''":"'";
                            if (item.Name.IndexOf("Smt_End_") == 0)
                            {
                                sb.Append(string.Format(" and {0}{1}<='{2}'", tableName, item.Name.Replace("Smt_End_", ""), ItemValue).Replace("'", execsqlReplace));
                            }
                            if (item.Name.IndexOf("Smt_Start_") == 0)
                            {
                                sb.Append(string.Format("  and {0}{1}>='{2}' ", tableName, item.Name.Replace("Smt_Start_", ""), ItemValue).Replace("'", execsqlReplace));
                            }
                           
                        }
                        else
                        {
                            string ColName = "Chk_" + item.Name;
                            //找到这个是否完全匹配的判断对应的值
                            bool IsLike;
                            if (type.GetProperty(ColName) == null)
                            {
                                IsLike = true;
                            }
                            else
                            {
                                IsLike = Convert.ToBoolean(type.GetProperty(ColName).GetValue(entity, null));
                            }
                            //完全匹配的bool值
                            if (!IsLike)
                            {
                                if (execsql)
                                {
                                    sb.Append(string.Format(" and {0}{1} like ''%{2}%'' ", tableName, item.Name, ItemValue));
                                }
                                else
                                {
                                    sb.Append(string.Format(" and {0}{1} like '%{2}%' ", tableName, item.Name, ItemValue));
                                }

                            }
                            else
                            {
                                if (execsql)
                                {
                                    sb.Append(string.Format(" and {0}{1}=''{2}''", tableName, item.Name, ItemValue));
                                }
                                else
                                {
                                    sb.Append(string.Format(" and {0}{1}='{2}' ", tableName, item.Name, ItemValue));
                                }

                            }
                        }
                    }
                    //如果是时间类型的判断
                    else if (item.PropertyType == typeof(DateTime?))
                    {
                        string ColName = item.Name;
                        //取到列名的类型 起始值，结束值
                        string StartName = ColName.Substring(0, 4);
                        if (!string.IsNullOrWhiteSpace(Convert.ToString(item.GetValue(entity, null))))
                        {
                            if (StartName == "End_")
                            {
                                if (execsql)
                                {
                                    sb.Append(string.Format(" and {2}{0} <=''{1}''", ColName.Substring(4, ColName.Length - 4), Convert.ToDateTime(item.GetValue(entity, null)).ToString("yyyy-MM-dd") + " 23:59:59",tableName));
                                }
                                else
                                {
                                    sb.Append(string.Format(" and {0} <='{1}'", ColName.Substring(4, ColName.Length - 4), Convert.ToDateTime(item.GetValue(entity, null)).ToString("yyyy-MM-dd") + " 23:59:59"));
                                }
                            }
                            //判断是否查询年月日时分秒
                            else if (item.Name.IndexOf("T_Start_") == 0 || item.Name.IndexOf("T_End_") == 0)
                            {
                                string execsqlReplace = execsql ? "''" : "'";
                                if (item.Name.IndexOf("T_End_") == 0)
                                {
                                    sb.Append(string.Format(" and {0}{1}<='{2}'", tableName, item.Name.Replace("T_End_", ""), ItemValue).Replace("'", execsqlReplace));
                                }
                                if (item.Name.IndexOf("T_Start_") == 0)
                                {
                                    sb.Append(string.Format("  and {0}{1}>='{2}' ", tableName, item.Name.Replace("T_Start_", ""), ItemValue).Replace("'", execsqlReplace));
                                }

                            }
                            else
                            {
                                if (execsql)
                                {
                                    sb.Append(string.Format(" and {2}{0} >=''{1}''", ColName, Convert.ToDateTime(item.GetValue(entity, null)).ToString("yyyy-MM-dd") + " 00:00:00", tableName));
                                }
                                else
                                {
                                    sb.Append(string.Format(" and {0} >='{1}'", ColName, Convert.ToDateTime(item.GetValue(entity, null)).ToString("yyyy-MM-dd") + " 00:00:00"));
                                }
                             }
                        }
                    }
                    //如果是其它类型
                    else
                    {
                        if (item.Name.Contains("Chk_"))
                        {
                            continue;
                        }
                        if (item.Name.Contains("End_"))
                        {
                            sb.Append(string.Format(" and {0} <={1}", item.Name.Remove(0, 4), ItemValue));
                        }
                        else if (item.Name.Contains("Start_"))
                        {
                            sb.Append(string.Format(" and {0} >={1}", item.Name.Remove(0, 6), ItemValue));
                        }
                        else
                        {
                            sb.Append(string.Format(" and {0} ={1}", item.Name, ItemValue));
                        }

                    }
                }
            }
            return sb;
        }
        public static StringBuilder QueryWhereSQL<T>(T entity)
        {

            StringBuilder sb = new StringBuilder();
            Type type = entity.GetType();
            string viewName = "";
            var viewAttribute = type.GetCustomAttributes(true).OfType<ViewAttribute>();
            var descriptionAttributes = viewAttribute as ViewAttribute[] ?? viewAttribute.ToArray();
            if (descriptionAttributes.Any())
                viewName = descriptionAttributes.ToList()[0].viewName;

            string tableName = string.IsNullOrWhiteSpace(viewName) ? EntityAttribute.GetEntityTable<T>() : viewName;
            sb.Append(string.Format("select {0}.* from ", tableName));
            sb.Append(tableName);
            sb.Append(" where 1=1 ");
            return GetQueryWhereString<T>(entity, tableName, type, sb);

        }
        /// <summary>
        /// 拼接条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static StringBuilder QueryConditionSQL<T>(T entity) where T : new()
        {
            StringBuilder sb = new StringBuilder();
            Type type = entity.GetType();
            string tableName = EntityAttribute.GetEntityTable<T>();
            if (!string.IsNullOrWhiteSpace(tableName))
            {
                tableName = tableName + ".";
            }
            foreach (PropertyInfo item in GetProperties(type))
            {
                var ItemValue = item.GetValue(entity, null);
                if (ItemValue != null)
                {
                    if (item.PropertyType == typeof(string))
                    {
                        string ColName = "Chk_" + item.Name;
                        //找到这个是否完全匹配的判断对应的值
                        bool IsLike;
                        if (type.GetProperty(ColName) == null)
                        {
                            IsLike = true;
                        }
                        else
                        {
                            IsLike = Convert.ToBoolean(type.GetProperty(ColName).GetValue(entity, null));
                        }
                        //完全匹配的bool值
                        if (!IsLike)
                        {
                                sb.Append(string.Format(" and {0} like '%{1}%' ", item.Name, ItemValue));
                        }
                        else
                        {
                                sb.Append(string.Format(" and {0}='{1}' ", item.Name, ItemValue));
                        }
                    }
                    //如果是时间类型的判断
                    else if (item.PropertyType == typeof(DateTime?))
                    {
                        string ColName = item.Name;
                        //取到列名的类型 起始值，结束值
                        string StartName = ColName.Substring(0, 4);
                        if (!string.IsNullOrWhiteSpace(Convert.ToString(item.GetValue(entity, null))))
                        {
                            if (StartName == "End_")
                            {
                                sb.Append(string.Format(" and {0} <='{1}'", ColName.Substring(4, ColName.Length - 4), Convert.ToDateTime(item.GetValue(entity, null)).ToString("yyyy-MM-dd") + " 23:59:59"));
                            }
                            else
                            {
                                sb.Append(string.Format(" and {0} >='{1}'", ColName, item.GetValue(entity, null)));
                            }
                        }
                    }
                    //如果是其它类型
                    else
                    {
                        if (item.Name.Contains("Chk_"))
                        {
                            continue;
                        }
                        if (item.Name.Substring(0, 4) == "End_")
                        {
                            sb.Append(string.Format(" and {0} <={1}", item.Name.Remove(0,4), ItemValue));
                        }
                        else if (item.Name.Substring(0, 6) == "Start_")
                        {
                            sb.Append(string.Format(" and {0} >={1}", item.Name.Remove(0,6), ItemValue));
                        }
                        else
                        {
                            sb.Append(string.Format(" and {0} ={1}", item.Name, ItemValue));
                        }

                    }
                }
            }
            return sb;

        }
        /// <summary>
        /// 拼接 查询 SQL语句
        /// </summary>
        /// <param name="Top">显示条数</param>
        /// <returns></returns>
        public static StringBuilder SelectSql<T>(int Top) where T : new()
        {

            string tableName = typeof(T).Name;
            PropertyInfo[] props = GetProperties(new T().GetType());
            StringBuilder sbColumns = new StringBuilder();
            foreach (PropertyInfo prop in props)
            {
                sbColumns.Append(prop.Name + ",");
            }
            if (sbColumns.Length > 0) sbColumns.Remove(sbColumns.ToString().Length - 1, 1);
            string strSql = "SELECT top {0} {1} FROM {2} WHERE 1=1 ";
            strSql = string.Format(strSql, Top, sbColumns.ToString(), tableName + " ");
            return new StringBuilder(strSql);
        }
        /// <summary>
        /// 拼接 查询 SQL语句
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public static StringBuilder SelectSql(string tableName)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM " + tableName + " WHERE 1=1 ");
            return strSql;
        }
        /// <summary>
        /// 拼接 查询 SQL语句
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="Top">显示条数</param>
        /// <returns></returns>
        public static StringBuilder SelectSql(string tableName, int Top)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT top " + Top + " * FROM " + tableName + " WHERE 1=1 ");
            return strSql;
        }
        /// <summary>
        /// 拼接 查询条数 SQL语句
        /// </summary>
        /// <returns></returns>
        public static StringBuilder SelectCountSql<T>() where T : new()
        {
            string tableName = typeof(T).Name;//获取表名
            return new StringBuilder("SELECT Count(1) FROM " + tableName + " WHERE 1=1 ");
        }
        /// <summary>
        /// 拼接 查询最大数 SQL语句
        /// </summary>
        /// <param name="propertyName">属性字段</param>
        /// <returns></returns>
        public static StringBuilder SelectMaxSql<T>(string propertyName) where T : new()
        {
            string tableName = typeof(T).Name;//获取表名
            return new StringBuilder("SELECT MAX(" + propertyName + ") FROM " + tableName + "  WHERE 1=1 ");
        }


        #endregion

        #region  封装查询SQL
        /// <summary>
        /// 根据翻译能力前置进行SQL更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="translateInfo"></param>
        /// <param name="sqlString"></param>
        /// <param name="tablename"></param>
        /// <returns></returns>
        public static string PottingSql<T>(List<string[]> translateInfo, string sqlString, string tablename, bool View = false)
        {
            // sqlString = sqlString.ToUpper();
            if (translateInfo.Count() > 0)
            {
                if (!View)
                {
                    if (sqlString.Contains("*"))
                    {
                        typeof(T).GetProperties().ToList().ForEach(x =>
                        {
                            sqlString = sqlString.Insert(sqlString.IndexOf("from"), string.Format(",{0}.{1} ", tablename, x.Name));
                        }
                            );
                        sqlString = sqlString.Replace( tablename+".* ,", string.Empty);
                    }
                }
                string selectString = sqlString.Substring(0, sqlString.IndexOf("where"));
                string whereString = sqlString.Substring(sqlString.IndexOf("where"), sqlString.Length - sqlString.IndexOf("where"));
                List<string> tableName = new List<string>();
                tableName.Add(tablename);
                translateInfo.ForEach(y =>
                 {
                     int changedIndex = 1;
                     string name = tableName.Where(x => x == y[1].Trim()).FirstOrDefault();
                     bool changed = false;
                     if (!string.IsNullOrEmpty(name))
                     {
                         changed = true;
                         y[1] = string.Format("{0}{1}", y[1], changedIndex.ToString());
                     }
                     if (selectString.Contains(string.Format("{0}.{1}", tablename, y[0])))
                     {
                         selectString = selectString.Replace(string.Format("{0}.{1}", tablename, y[0]), string.Format("{0}.{1} as {2}", y[1], y[3], y[0]));
                     }
                     else if (View)
                     {  
                         string colName = selectString.Substring(0, selectString.IndexOf("from"));
                         string soursName = selectString.Substring(selectString.IndexOf("from"), selectString.Length - selectString.IndexOf("from"));
                         colName +=","+string.Format("{0}.{1} as T_{2}", y[1], y[3], y[0]);
                         selectString =string.Format("{0} {1}",colName , soursName);
                     }
                     selectString = selectString + string.Format(" left join {0} on {1}.{2}={3}.{4}", changed ? string.Format("{0} as {1}", name, y[1]) : y[1], tablename, y[0], y[1], y[2]);
                     changedIndex++;
                     tableName.Add(y[1].Trim());
                 });
                selectString += " " + whereString;
                sqlString = selectString;
            }
            return sqlString;
        }
        #endregion

        #region 扩展
        /// <summary>
        /// 获取访问元素
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static PropertyInfo[] GetProperties(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }

        /// <summary>
        /// 获取列名
        /// </summary>
        /// <param name="columnName">列名</param>
        /// <returns>int</returns>
        public static string GetConditionString(List<string> columnName)
        {
            string queryColumnName = string.Empty;
            if (columnName != null && columnName.Count > 0)
            {
                columnName.ForEach(x =>
                {
                    queryColumnName += (string.Format(x + "{0}", columnName.IndexOf(x) != columnName.Count - 1 ? "," : string.Empty));
                });
            }
            else
            {
                queryColumnName = "*";
            }
            return queryColumnName;
        }

        #endregion
    }
}
