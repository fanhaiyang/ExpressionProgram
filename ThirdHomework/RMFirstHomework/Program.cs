using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThirdHomework.Model;
using ThirdHomework.Factory;
using ThirdHomework.Interface;
using ThirdHomework.MyAttribute;
using SqlHelper;
using System.Linq.Expressions;

namespace ThirdHomework
{
    public class Program
    {
        static void Main(string[] args)
        {
            // 反射+工厂+配置文件
            IGetDataHelper client = MyFactory.CreateFactory();

            #region 查询：一般方法

            // 根据Id获取对象实体
            {
                User userModel = client.GetModelById<User>(1);
                Console.WriteLine("获取User对象：");
                Console.WriteLine(ShowData(userModel));

                Company compModel = client.GetModelById<Company>(2);
                Console.WriteLine("\n\n获取Company对象：");
                Console.WriteLine(ShowData(compModel));
            }

            // 获取所有对象
            {
                List<User> userList = client.GetModelList<User>();
                if (userList.Count > 0)
                {
                    Console.WriteLine("\n\n获取User集合：");
                    foreach (var item in userList)
                    {
                        Console.WriteLine(ShowData(item));
                    }
                }

                List<Company> compList = client.GetModelList<Company>();
                if (compList.Count > 0)
                {
                    Console.WriteLine("\n\n获取Company集合：");
                    foreach (var item in compList)
                    {
                        Console.WriteLine(ShowData(item));
                    }
                }
            }
            #endregion

            #region 查询：表达式目录树
            {
                ExpressionSqlHelper expressionSql = new ExpressionSqlHelper();
                List<User> userList = expressionSql.ExpressQuery<User>(n => n.Id > 2 && n.Name.Contains("1"));
                if (userList.Count > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\n\n--表达式目录树--获取User集合：\n");
                    foreach (var item in userList)
                    {
                        Console.WriteLine(ShowData(item));
                    }
                }
            }
            #endregion
            Console.ForegroundColor = ConsoleColor.Gray;
            // 添加数据
            {
                User userModel = new User()
                {
                    Name = "神韵凌天",
                    Account = "admin",
                    Password = "123456",
                    Email = "931385258@qq.com",
                    Mobile = "13812345678",
                    CompanyId = 1,
                    CreateTime = DateTime.Now,
                    LastLoginTime = DateTime.Now,
                    LastModifyTime = DateTime.Now,
                    CreatorId = 1,
                    Status = 1,
                    UserType = 2
                };
                if (userModel.Validate())
                {
                    int row = client.AddModel(userModel);
                    if (row > 0)
                    {
                        Console.WriteLine("\n\n添加User对象：");
                        Console.WriteLine(ShowData(userModel));
                    }
                    else
                    {
                        Console.WriteLine("\n\n添加User对象失败！！！");
                    }
                }
                else
                {
                    Console.WriteLine("\n\n添加User对象失败！！！属性格式错误");
                }
            }

            // 更新User对象数据
            {
                try
                {
                    User userModel = client.GetModelList<User>()[1];
                    if (userModel.Id != 0)
                    {
                        userModel.Name = "TestUpdate";
                        userModel.Mobile = "15726683424";
                        if (userModel.Validate())
                        {
                            int row = client.UpdateModel<User>(userModel);
                            if (row > 0)
                            {
                                Console.WriteLine("\n\n更新User对象：");
                                Console.WriteLine(ShowData(userModel));
                            }
                            else
                            {
                                Console.WriteLine("\n\n更新User对象失败！！！");
                            }
                        }
                        else
                        {
                            Console.WriteLine("\n\n更新User对象失败！！！属性格式错误");
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("\n\n更新数据出错：", ex.Message);
                }
            }

            //// 删除数据
            //{
            //    User userModel = client.GetModelList<User>()[1];
            //    if (userModel.Id != 0)
            //    {
            //        int row = client.DeleteModel<User>(userModel.Id);
            //        if (row > 0)
            //        {
            //            Console.WriteLine("\n\n删除User对象：");
            //            Console.WriteLine(ShowData(userModel));
            //        }
            //        else
            //        {
            //            Console.WriteLine("\n\n删除User对象失败！！！");
            //        }
            //    }
            //}

            Console.ReadKey();
        }

        public static string ShowData<T>(T model) where T : BaseModel
        {
            Type type = typeof(T);
            string showMsg = "";
            foreach (var item in type.GetProperties())
            {
                showMsg += $"{item.GetRemarkName()}={item.GetValue(model)}  ";
            }
            return showMsg;
        }
    }
}
