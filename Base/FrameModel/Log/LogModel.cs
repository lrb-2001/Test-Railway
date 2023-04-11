using SqlSugar;
using System;

namespace FrameModel;

[SugarTable("Logs")]
public class LogModel
{
    /// <summary>
    /// ID
    /// </summary>
    //[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public string ID { get; set; }

    /// <summary>
    /// 日志内容
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// 日志类型
    /// </summary>
    public string LogType { get; set; }

    /// <summary>
    /// 日志名称
    /// </summary>
    public string LoggerName { get; set; }

    /// <summary>
    /// 类名
    /// </summary>
    public string ClassName { get; set; }

    /// <summary>
    /// 位置
    /// </summary>
    public string Domain { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }


}
