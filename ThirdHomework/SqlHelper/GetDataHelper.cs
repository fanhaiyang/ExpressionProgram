using ThirdHomework.Interface;
using ThirdHomework.Model;
using ThirdHomework.MyAttribute;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace SqlHelper
{
    public class GetDataHelper : IGetDataHelper
    {
        private readonly string connectionstr = ConfigurationManager.ConnectionStrings["sqlconnection"].ConnectionString;

        #region 1.非委托封装

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
                throw ex;
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
                throw ex;
            }
            return list;
        }

        #endregion

        #region 2.委托封装

        /// <summary>
        /// 委托封装
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="func"></param>
        /// <param name="sqlPara"></param>
        /// <returns></returns>
        public T ExcuteSqlDelegate<T>(string sql, Func<SqlCommand, T> func, params SqlParameter[] sqlPara)
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
                        return func.Invoke(cmd);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取DataReader数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        public List<T> GetDataReader<T>(SqlDataReader dataReader)
        {
            Type modelType = typeof(T);
            var list = new List<T>();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    var model1 = Activator.CreateInstance(modelType);
                    foreach (var item in modelType.GetProperties())
                    {
                        if (dataReader[item.GetDBName()] != DBNull.Value)
                        {
                            item.SetValue(model1, dataReader[item.GetDBName()]);
                        }
                    }
                    list.Add((T)model1);
                }
            }
            return list;
        }

        #endregion

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
                //// 1，非委托调用
                //List<T> modelList = ExecuteSql<T>(sqlString, parameters);
                //if (modelList.Count <= 0)
                //{
                //    return (T)Activator.CreateInstance(modelType);
                //}
                //return modelList[0];

                // 2，委托调用
                T model = ExcuteSqlDelegate<T>(sqlString, cmd =>
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    var getData = GetDataReader<T>(reader);
                    if (getData != null)
                        return getData[0];
                    return null;
                }, parameters);
                return model;
            }
            catch (Exception ex)
            {
                throw ex;
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
                //// 1.非委托调用
                //List<T> modelList = ExecuteSql<T>(sqlString, null);
                //return modelList;

                // 2.委托调用
                return ExcuteSqlDelegate<List<T>>(sqlString, cmd =>
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    var getData = GetDataReader<T>(reader);
                    return getData;
                }, null);
            }
            catch (Exception ex)
            {
                throw ex;
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

                // 1.非委托调用
                //return ExecuteNonQuery(sqlString, parameters.ToArray());

                // 2.委托调用
                return ExcuteSqlDelegate<int>(sqlString, cmd =>
                {
                    return cmd.ExecuteNonQuery();
                }, parameters.ToArray());
            }
            catch (Exception ex)
            {
                throw ex;
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
                throw ex;
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
                throw ex;
            }
        }
    }
}
