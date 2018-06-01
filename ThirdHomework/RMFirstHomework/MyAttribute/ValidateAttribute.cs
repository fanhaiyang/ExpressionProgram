using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RMFirstHomework.MyAttribute
{
    public abstract class ValidateAttribute : Attribute
    {
        public abstract bool IsValid(object value);
    }

    /// <summary>
    /// 必填属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class RequiredAttribute : ValidateAttribute
    {
        public bool IsRequired { get; set; }
        public RequiredAttribute()
        {
            this.IsRequired = true;
        }

        public override bool IsValid(object value)
        {
            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// 手机格式
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class MobileAttribute : ValidateAttribute
    {
        public string Mobile { get; set; }
        public MobileAttribute(string mobile)
        {
            this.Mobile = mobile;
        }

        public override bool IsValid(object value)
        {
            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                if (Regex.IsMatch(value.ToString(), Mobile))
                    return true;
            }
            return false;
        }
    }

    /// <summary>
    /// 邮箱格式
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class EmailAttribute : ValidateAttribute
    {
        public string Email { get; set; }
        public EmailAttribute(string email)
        {
            this.Email = email;
        }

        public override bool IsValid(object value)
        {
            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                if (Regex.IsMatch(value.ToString(), Email))
                    return true;
            }
            return false;
        }
    }

    /// <summary>
    /// 长度限制
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class LengthAttribute : ValidateAttribute
    {
        public int MinLength { get; set; }
        public int MaxLength { get; set; }
        public LengthAttribute(int minLength, int maxLength)
        {
            this.MinLength = minLength;
            this.MaxLength = maxLength;
        }

        public override bool IsValid(object value)
        {
            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                int len = value.ToString().Length;
                if (len >= MinLength && len <= MaxLength)
                {
                    return true;
                }
            }
            return false;
        }
    }

    /// <summary>
    /// ValidateAttribute 扩展方法
    /// </summary>
    public static class ValidateAttributeExtend
    {
        public static bool Validate(this object value)
        {
            Type type = value.GetType();
            foreach (var porp in type.GetProperties())
            {
                if (porp.IsDefined(typeof(ValidateAttribute), true))
                {
                    object[] atttibuts = porp.GetCustomAttributes(typeof(ValidateAttribute), true);
                    foreach (ValidateAttribute item in atttibuts)
                    {
                        if (!item.IsValid(porp.GetValue(value)))
                            return false;
                    }
                }
            }
            return true;
        }
    }
}
