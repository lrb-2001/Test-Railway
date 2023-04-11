namespace FrameModel.SqlSugar;

public class MutiDBOperate
{
    /// <summary>
    /// 连接启用开关
    /// </summary>
    public bool Enabled { get; set; }
    /// <summary>
    /// 连接ID
    /// </summary>
    public string ConnId { get; set; }
    /// <summary>
    /// 权重比，越大越执行的次数越多
    /// </summary>
    public int HitRate { get; set; }
    /// <summary>
    /// 连接字符串
    /// </summary>
    public string Connection { get; set; }
    /// <summary>
    /// 自动关闭连接
    /// </summary>
    public bool IsAutoCloseConnection { get; set; }
    /// <summary>
    /// 数据库类型
    /// </summary>
    public DataBaseType DbType { get; set; }
}
