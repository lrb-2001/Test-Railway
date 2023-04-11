using FrameModel.SqlSugar;
using SqlSugar;
using SqlSugar.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FrameCommon;

public class SqlSugarHelper
{
    private static SqlSugarClient sqlSugarWrite;//写
    private static SqlSugarClient sqlSugarRead;//读
    private ISqlSugarClient SqlSugarWrite { get { return sqlSugarWrite; } }
    internal ISqlSugarClient Db { get { return SqlSugarWrite; } }
    private ISqlSugarClient SqlSugarRead { get { return sqlSugarRead; } }
    internal ISqlSugarClient DbRead { get { return SqlSugarRead; } }
    /// <summary>
    /// 数据库连接字符串
    /// </summary>
    public static string ConnectionString { get; set; }

    private static int INum = 0;

    /// <summary>
    /// 创建读写库  取Json文件
    /// </summary>
    /// <returns></returns>
    public static Dictionary<string, SqlSugarClient> GetInstance()
    {
        var result = new Dictionary<string, SqlSugarClient>();
        //主库连接
        sqlSugarWrite = new SqlSugarClient(new ConnectionConfig()
        {
            DbType = (DbType)Convert.ToInt32(AppSettings.app(new string[] { "MainLibrary", "DBType" })),
            IsAutoCloseConnection = Convert.ToBoolean(AppSettings.app(new string[] { "MainLibrary", "IsAutoCloseConnection" })),
            ConnectionString = AppSettings.app(new string[] { "MainLibrary", "Connection" })
        });
        /* 如果要开启多库支持，
       * 1、在appsettings.json 中开启MutiDBEnabled节点为true，必填
       * 2、设置一个主连接的数据库ID，节点MainDB，对应的连接字符串的Enabled也必须true，必填
       */
        if (AppSettings.app(new string[] { "MutiDBEnabled" }).ObjToBool())
        {
            //获取从库列表
            List<MutiDBOperate> listdatabase = AppSettings.app<MutiDBOperate>("FromLibrary").Where(i => i.Enabled).ToList();

            if (!AppSettings.app(new string[] { "PollOrWeight" }).ObjToBool())
            {
                int index = INum++ % listdatabase.Count;//从库下标 -----轮询策略
                sqlSugarWrite = new SqlSugarClient(new ConnectionConfig()
                {
                    DbType = (DbType)listdatabase[index].DbType,
                    IsAutoCloseConnection = listdatabase[index].IsAutoCloseConnection,
                    ConnectionString = listdatabase[index].Connection
                });
            }
            else
            {
                //权重从库列表
                List<MutiDBOperate> ListDataWeight = new List<MutiDBOperate>();
                foreach (var item in listdatabase)
                {
                    for (int i = 0; i < item.HitRate; i++)
                    {
                        ListDataWeight.Add(item);
                    }
                }
                int index = new Random(INum++).Next(0, ListDataWeight.Count);//从库下标 -----权重策略
                sqlSugarWrite = new SqlSugarClient(new ConnectionConfig()
                {
                    DbType = (DbType)ListDataWeight[index].DbType,
                    IsAutoCloseConnection = ListDataWeight[index].IsAutoCloseConnection,
                    ConnectionString = ListDataWeight[index].Connection
                });
            }
        }
        else
        {
            sqlSugarRead = sqlSugarWrite;
        }
        result.Add("write", sqlSugarWrite);
        result.Add("read", sqlSugarRead);

        return result;
    }

    /// <summary>
    /// 创建
    /// </summary>
    /// <param name="connString">连接字符串</param>
    /// <returns>SqlSugarClient实例对象</returns>
    public static Dictionary<string, SqlSugarClient> GetInstance(string connString)
    {
        var result = new Dictionary<string, SqlSugarClient>();
        //数据库连接
        SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
        {
            DbType = DbType.SqlServer,//数据库类型
            IsAutoCloseConnection = true,//自动释放和关闭数据库连接，如果有事务事务结束时关闭，否则每次操作后关闭
            ConnectionString = connString,//连接字符串
            InitKeyType = InitKeyType.Attribute//从特性读取主键自增信息
        });

        result.Add("write", db);
        result.Add("read", db);
        return result;
    }

}
