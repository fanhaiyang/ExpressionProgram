using RMFirstHomework.Interface;
using RMFirstHomework.Model;
using RMFirstHomework.MyAttribute;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlHelper
{
    public class GetDataHelper : IGetDataHelper
    {
        private readonly string connectionstr = ConfigurationManager.ConnectionStrings["sqlconnection"].ConnectionString;

        /// <summary>
        /// 对连接执行 Transact-SQL 语句并返回受影响的行数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="sqlPara"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string sql, params SqlParameter[] sqlPara)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionstr))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        if (sqlPara != null)
                        {
                            cmd.Parameters.AddRange(sqlPara);
                        }
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("对连接执行 Transact-SQL 语句出错：", ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// 获取 SqlDataReader 对象集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="sqlPara"></param>
        /// <returns></returns>
        public List<T> ExecuteSql<T>(string sql, params SqlParameter[] sqlPara) where T : BaseModel
        {
            Type modelType = typeof(T);
            var list = new List<T>();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionstr))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        if (sqlPara != null)
                        {
                            cmd.Parameters.AddRange(sqlPara);
                        }
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                var model = Activator.CreateInstance(modelType);
                                foreach (var item in modelType.GetProperties())
                                {
                                    if (reader[item.GetDBName()] != DBNull.Value)
                                    {
                                        item.SetValue(model, reader[item.GetDBName()]);
                                    }
                                }
                                list.Add((T)model);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("获取SqlDataReader对象出错：", ex.Message);
            }
            return list;
        }

        /// <summary>
        /// 通过Id获取对象实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public T GetModelById<T>(int id) where T : BaseModel
        {
            Type modelType = typeof(T);
            string sqlString = $"select * from [{modelType.Name}] where Id=@Id";
            SqlParameter[] parameters = {
                    new SqlParameter("@Id",id)
            };
            try
            {
                List<T> modelList = ExecuteSql<T>(sqlString, parameters);
                if (modelList.Count <= 0)
                {
                    return (T)Activator.CreateInstance(modelType);
                }
                return modelList[0];
            }
            catch (Exception ex)
            {
                Console.WriteLine("通过Id获取对象实体出错：", ex.Message);
                return (T)Activator.CreateInstance(modelType);
            }
        }

        /// <summary>
        /// 获取所有对象实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> GetModelList<T>() where T : BaseModel
        {
            Type modelType = typeof(T);
            string sqlString = $"select * from [{modelType.Name}]";
            try
            {
                List<T> modelList = ExecuteSql<T>(sqlString, null);
                return modelList;
            }
            catch (Exception ex)
            {
                Console.WriteLine("获取所有对象实体出错:", ex.Message);
                return new List<T>();
            }
        }

        /// <summary>
        /// 添加对象实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public int AddModel<T>(T model) where T : BaseModel
        {
            Type modelType = typeof(T);
            var values = new List<string>();
            var parameters = new List<SqlParameter>();
            try
            {
                foreach (var item in modelType.GetProperties())
                {
                    if (item.Name.Equals("Id"))
                    {
                        continue;
                    }
                    values.Add("@" + item.GetDBName());
                    parameters.Add(new SqlParameter("@" + item.GetDBName(), item.GetValue(model) ?? DBNull.Value));
                }
                string valueStr = string.Join(",", values);

                string sqlString = $"insert into [{modelType.Name}] values({valueStr})";
                return ExecuteNonQuery(sqlString, parameters.ToArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine("添加对象实体出错：", ex.Message);
                return 0;
            }

        }

        /// <summary>
        /// 更新对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public int UpdateModel<T>(T model) where T : BaseModel
        {
            Type modelType = typeof(T);
            var values = new List<string>();
            var parameters = new List<SqlParameter>();
            try
            {
                foreach (var item in modelType.GetProperties())
                {
                    if (!item.Name.Equals("Id"))
                    {
                        values.Add(string.Format("{0}=@{1}", item.GetDBName(), item.GetDBName()));
                    }
                    parameters.Add(new SqlParameter("@" + item.GetDBName(), item.GetValue(model) ?? DBNull.Value));
                }
                string valueStr = string.Join(",", values);

                string sqlString = $"update [{modelType.Name}] set {valueStr} where Id=@Id";
                return ExecuteNonQuery(sqlString, parameters.ToArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine("更新对象实体出错：", ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DeleteModel<T>(int id) where T : BaseModel
        {
            Type modelType = typeof(T);
            try
            {
                string sqlString = $"delete from [{modelType.Name}] where Id=@Id";
                SqlParameter[] paramters = { new SqlParameter("@Id", id) };
                return ExecuteNonQuery(sqlString, paramters);
            }
            catch (Exception ex)
            {
                Console.WriteLine("删除数据出错：", ex.Message);
                return 0;
            }
        }
    }
}
