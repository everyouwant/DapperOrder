using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFine.Code
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class EditNullAttribute : Attribute
    {
        private string _EditNullName;

         public EditNullAttribute(string viewName = "")
        {
            _EditNullName = viewName;
        }

         public virtual string  EditColNullName { get { return _EditNullName; } set { value = _EditNullName; } }
    }
}
