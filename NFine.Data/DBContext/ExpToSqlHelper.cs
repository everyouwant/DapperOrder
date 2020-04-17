using System;
using System.Linq.Expressions;
using System.Reflection;


namespace NFine.Data
{

    public static class ExpToSqlHelper
    {
        public static Expression ClearLeft(this Expression expression)
        {
            if (expression is BinaryExpression)
            {
                BinaryExpression exp = expression as BinaryExpression;
                if (exp.Left is ConstantExpression && (exp.Left as ConstantExpression).Value.ToString().ToUpper() == "TRUE")
                {
                    return exp.Right;
                }
            }
            return expression;
        }

        public static string DealExpress(Expression exp)
        {

            if (exp is LambdaExpression)
            {
                LambdaExpression l_exp = exp as LambdaExpression;
                return DealExpress(l_exp.Body);
            }
            if (exp is BinaryExpression)
            {
                return DealBinaryExpression(exp as BinaryExpression);
            }
            if (exp is MemberExpression)
            {
                return DealMemberExpression(exp as MemberExpression);
            }
            if (exp is ConstantExpression)
            {
                return DealConstantExpression(exp as ConstantExpression);
            }
            if (exp is UnaryExpression)
            {
                return DealUnaryExpression(exp as UnaryExpression);
            }
            if (exp is MethodCallExpression)
            {
                return DealMethodCallExpression(exp as MethodCallExpression);
            }


            return "";
        }
        private static string DealUnaryExpression(UnaryExpression exp)
        {
            return DealExpress(exp.Operand);
        }
        private static string DealConstantExpression(ConstantExpression exp)
        {
            object vaule = exp.Value;
            string v_str = string.Empty;
            if (vaule == null)
            {
                return "NULL";
            }
            if (vaule is string)
            {
                v_str = string.Format("'{0}'", vaule.ToString());
            }
            else if (vaule is DateTime)
            {
                DateTime time = (DateTime)vaule;
                v_str = string.Format("'{0}'", time.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            else
            {
                v_str = vaule.ToString();
            }
            return v_str;
        }

        /// <summary>
        /// 模糊查询    
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        private static string DealMethodCallExpression(MethodCallExpression exp)
        {
            var coloumnName = DealMemberExpression(exp.Object.Reduce() as MemberExpression);
            var coloumnValue = DealMemberExpression(exp.Arguments[0] as MemberExpression);
            return string.Format("{0} Like '%{1}%'", coloumnName, coloumnValue.Replace("'", ""));
        }
        private static string DealBinaryExpression(BinaryExpression exp)
        {

            string left = DealExpress(exp.Left);
            string oper = GetOperStr(exp.NodeType);
            string right = DealExpress(exp.Right);
            if (right == "NULL")
            {
                if (oper == "=")
                {
                    oper = " is ";
                }
                else
                {
                    oper = " is not ";
                }
            }
            return left + oper + right;
        }
        private static string DealMemberExpression(MemberExpression exp)
        {
            if (exp.Expression.NodeType.ToString() == "Constant")
            {
                Type type = (exp.Expression as ConstantExpression).Value.GetType();
                FieldInfo info = type.GetField(exp.Member.Name);

                object obj = info.GetValue((exp.Expression as ConstantExpression).Value);
                if (info.FieldType.Name == "String")
                {
                    return "'" + obj.ToString() + "'";
                }
                else if (info.FieldType.Name == "DateTime")
                {
                    DateTime time = (DateTime)obj;
                    return string.Format("'{0}'", time.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                return obj.ToString();
            }
            return exp.Member.Name;
        }
        private static string GetOperStr(ExpressionType e_type)
        {
            switch (e_type)
            {
                case ExpressionType.OrElse: return " OR ";
                case ExpressionType.Or: return "|";
                case ExpressionType.AndAlso: return " AND ";
                case ExpressionType.And: return "&";
                case ExpressionType.GreaterThan: return ">";
                case ExpressionType.GreaterThanOrEqual: return ">=";
                case ExpressionType.LessThan: return "<";
                case ExpressionType.LessThanOrEqual: return "<=";
                case ExpressionType.NotEqual: return "<>";
                case ExpressionType.Add: return "+";
                case ExpressionType.Subtract: return "-";
                case ExpressionType.Multiply: return "*";
                case ExpressionType.Divide: return "/";
                case ExpressionType.Modulo: return "%";
                case ExpressionType.Equal: return "=";
            }
            return "";
        }


    }
}
