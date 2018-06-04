using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlHelper.Code
{
    /// <summary>
    /// 表达式目录树封装类
    /// </summary>
    public class ConditionBuilderVisitor : ExpressionVisitor
    {
        /// <summary>
        /// 栈，先进后出
        /// </summary>
        private Stack<string> StringStack = new Stack<string>();

        /// <summary>
        /// 表达式目录树转换成字符串
        /// </summary>
        /// <returns></returns>
        public string ToExpressionString()
        {
            string str = string.Concat(StringStack.ToArray());
            StringStack.Clear();
            return str;
        }

        /// <summary>
        /// 二元表达式转换
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (node == null) throw new ArgumentNullException("BinaryExpression");

            StringStack.Push(")");
            base.Visit(node.Right);
            StringStack.Push(" " + node.NodeType.ToSqlOperator() + " ");
            base.Visit(node.Left);
            StringStack.Push("(");

            return node;
        }

        /// <summary>
        /// 字段表达式转换
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitMember(MemberExpression node)
        {
            if (node == null) throw new ArgumentNullException("MemberExpression");

            StringStack.Push(" [" + node.Member.Name + "] ");
            return node;
        }

        /// <summary>
        /// 常量表达式
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (node == null) throw new ArgumentNullException("ConstantExpression");

            StringStack.Push(" '" + node.Value + "' ");
            return node;
        }

        /// <summary>
        /// 方法表达式
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m == null) throw new ArgumentNullException("MethodCallExpression");

            string format;
            switch (m.Method.Name)
            {
                case "StartsWith":
                    format = "({0} LIKE {1}+'%')";
                    break;

                case "Contains":
                    format = "({0} LIKE '%'+{1}+'%')";
                    break;

                case "EndsWith":
                    format = "({0} LIKE '%'+{1})";
                    break;

                default:
                    throw new NotSupportedException(m.NodeType + " is not supported!");
            }
            base.Visit(m.Object);
            base.Visit(m.Arguments[0]);
            string right = StringStack.Pop();
            string left = StringStack.Pop();
            StringStack.Push(String.Format(format, left, right));

            return m;
        }
    }
}
