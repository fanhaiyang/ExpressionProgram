using SqlHelper.Code;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ThirdHomework.Model;

namespace SqlHelper
{
    /// <summary>
    /// 表达式目录树封装sql帮助类
    /// </summary>
    public class ExpressionSqlHelper
    {
        GetDataHelper client = new GetDataHelper();

        /// <summary>
        /// 表达式目录树封装DataReader返回数据：生成硬代码返回
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        public Company ExpressDataReader(SqlDataReader dataReader)
        {
            if (dataReader.Read())
            {
                Expression<Func<SqlDataReader, Company>> expression = reader => new Company
                {
                    Id = (int)Convert.ChangeType(reader["Id"], typeof(int)),
                    Name = reader["Name"].ToString(),
                    CreateTime = Convert.ToDateTime(reader["CreateTime"]),
                    CreatorId = Convert.ToInt32(reader["CreatorId"].ToString()),
                    LastModifierId = Convert.ToInt32(reader["LastModifierId"].ToString()),
                    LastModifyTime = Convert.ToDateTime(reader["LastModifyTime"])
                };
                return expression.Compile().Invoke(dataReader);
            }
            return null;
        }

        /// <summary>
        /// 表达式树查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public List<T> ExpressQuery<T>(Expression<Func<T, bool>> predicate)
        {
            try
            {
                ConditionBuilderVisitor visitor = new ConditionBuilderVisitor();
                visitor.Visit(predicate);
                string sqlStr = visitor.ToExpressionString();

                Type modelType = typeof(T);
                string sqlString = $"select * from [{modelType.Name}] where 1=1";

                if (!string.IsNullOrEmpty(sqlStr))
                {
                    sqlString += " AND "+sqlStr;
                }

                return client.ExcuteSqlDelegate<List<T>>(sqlString, cmd =>
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    var getData = client.GetDataReader<T>(reader);
                    return getData;
                }, null);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }


    }
}
