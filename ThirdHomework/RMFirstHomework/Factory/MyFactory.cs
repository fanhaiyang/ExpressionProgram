using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using ThirdHomework.Interface;

namespace ThirdHomework.Factory
{
    public class MyFactory
    {
        private static string dllName = ConfigurationManager.AppSettings["DllName"].ToString();
        private static string typeName = ConfigurationManager.AppSettings["TypeName"].ToString();

        /// <summary>
        /// 创建工厂实例
        /// </summary>
        /// <returns></returns>
        public static IGetDataHelper CreateFactory()
        {
            Assembly assembly = Assembly.Load(dllName);
            Type modelType = assembly.GetType(typeName);
            IGetDataHelper factory = (IGetDataHelper)Activator.CreateInstance(modelType);
            return factory;
        }
    }
}
