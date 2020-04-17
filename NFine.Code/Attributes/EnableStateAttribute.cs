using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFine.Code
{
   [AttributeUsage(AttributeTargets.Property | AttributeTargets.Interface)]
    public class EnableStateAttribute : Attribute
    {
            #region  私有变量
        //列名称
        private string _columnName;

        #endregion

        #region  构造方法
        public EnableStateAttribute(string columnName = "")
        {
            _columnName = columnName;
        }
        #endregion

        #region  变量封装
        public virtual string columnName 
        {
            get { return _columnName; }
            set { _columnName = value; } 
        }
        #endregion

    }
}
