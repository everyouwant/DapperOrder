using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFine.Code
{
    /// <summary>
    /// 翻译类(列如有Table_A(ID,B_ID,Money)和Table_B(ID,Name),如要翻译成(A.ID,B.Name,A.Money)需要设置[Translate/、、("Table_B", "ID", "Name")]属性
    /// </summary>
   [AttributeUsage(AttributeTargets.Property | AttributeTargets.Interface)]
   public  class TranslateAttribute : Attribute
    {
       
        #region  私有变量
        //主表名称
        private string _mainTable;

        //翻译原字段
        private string _originalField;

        //翻译新字段
        private string _newField;
        private string _newKey;
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mainTable">新表名称</param>
        /// <param name="originalField">当前关联键值</param>
        /// <param name="newField">关联表对应显示字段</param>
        /// <param name="newKey">关联表的主键字段</param>
        #region  构造方法
        public TranslateAttribute(string mainTable="", string originalField="", string newField="",string newKey = "")
        {
            _mainTable = mainTable;
            _originalField = originalField;
            _newField = newField;
            _newKey = newKey;
        }
        #endregion

        #region  变量封装
        public virtual string mainTable 
        { 
            get { return _mainTable; } set { _mainTable = value; } 
        }
        public virtual string originalField
        {
            get { return _originalField; }
            set { _originalField = value; }
        }
        public virtual string newField
        {
            get { return _newField; }
            set { _newField = value; }
        }
        public virtual string newKey
        {
            get { return _newKey; }
            set { _newKey = value; }
        }
        #endregion

    }
}
