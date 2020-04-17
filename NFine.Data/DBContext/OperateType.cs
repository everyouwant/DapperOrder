using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFine.Data
{
    /// <summary>
    /// 版 本 6.1
    /// Copyright (c) 2017 重庆万紫科技有限公司
    /// 创建人：zpp
    /// 日 期：2017-11-13 09:13:23
    /// 描 述：数据库类型枚举
    /// </summary>
    public enum OperateType
    {
        /// <summary>
        /// 数据操作类型：Select
        /// </summary>
        Select,

        /// <summary>
        /// 数据操作类型：Insert
        /// </summary>
        Insert,

        /// <summary>
        /// 数据操作类型：Update
        /// </summary>
        Update,

        /// <summary>
        /// 数据操作类型：Delete
        /// </summary>
        Delete
    }
}
