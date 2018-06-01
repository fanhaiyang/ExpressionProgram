using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RMFirstHomework.Model;
using System.Data.SqlClient;

namespace RMFirstHomework.Interface
{
    public interface IGetDataHelper
    {
        int ExecuteNonQuery(string sql, params SqlParameter[] sqlPara);

        List<T> ExecuteSql<T>(string sql, params SqlParameter[] sqlPara) where T : BaseModel;

        /// <summary>
        /// 通过Id获取对象实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        T GetModelById<T>(int id) where T : BaseModel;

        /// <summary>
        /// 获取所有对象实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        List<T> GetModelList<T>() where T : BaseModel;

        /// <summary>
        /// 增加对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        int AddModel<T>(T model) where T : BaseModel;

        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        int UpdateModel<T>(T model) where T : BaseModel;

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        int DeleteModel<T>(int id) where T : BaseModel;
    }
}
