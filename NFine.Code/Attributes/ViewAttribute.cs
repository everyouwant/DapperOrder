using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFine.Code
{
    /// <summary>
    /// 视图
    /// <author>
    ///		<name>zpp</name>
    ///		<date>2017年11月24日14:44:17</date>
    /// </author>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class ViewAttribute : Attribute
    {
        private string _viewname;

        public ViewAttribute(string viewName = "")
        {
            _viewname = viewName;
        }

        public virtual string viewName { get { return _viewname; } set { _viewname = value; } }
    }
}
