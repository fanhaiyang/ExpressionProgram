using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ThirdHomework.MyAttribute
{
    /// <summary>
    /// 设置属性名称
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SetNameAttribute : Attribute
    {
        public string DBName { get; set; }

        public SetNameAttribute(string dbName)
        {
            this.DBName = dbName;
        }
    }

    /// <summary>
    /// 标记属性名称
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class RemarkAttribute : Attribute
    {
        public string RemarkName { get; set; }

        public RemarkAttribute(string remarkName)
        {
            this.RemarkName = remarkName;
        }

    }

    ///---------------------------------扩展方法--------------------------------------///
    /// <summary>
    /// DBNameAttribute 扩展方法
    /// </summary>
    public static class MyAttributeExtend
    {
        public static string GetDBName<T>(this T value) where T : PropertyInfo
        {
            if (value.IsDefined(typeof(SetNameAttribute), true))
            {
                SetNameAttribute attribute = (SetNameAttribute)value.GetCustomAttribute(typeof(SetNameAttribute), true);
                return attribute.DBName;
            }
            return value.Name;
        }
    }

    /// <summary>
    /// RemarkAttribute 扩展方法
    /// </summary>
    public static class RemarkAttributeExtend
    {
        public static string GetRemarkName(this PropertyInfo value)
        {
            if (value.IsDefined(typeof(RemarkAttribute), true))
            {
                RemarkAttribute attribute = value.GetCustomAttribute<RemarkAttribute>(); //(RemarkAttribute)value.GetCustomAttribute(typeof(RemarkAttribute), true);
                return attribute.RemarkName;
            }
            return value.Name;
        }
    }
}
