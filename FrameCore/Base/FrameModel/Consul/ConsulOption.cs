namespace FrameModel;

public class ConsulOption
{
    /// <summary>
    /// 当前应用IP
    /// </summary>
    public string IP { get; set; }

    /// <summary>
    /// 当前应用端口
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// 当前服务名称
    /// </summary>
    public string ServiceName { get; set; }

    /// <summary>
    /// Consul集群IP
    /// </summary>
    public string ConsulIP { get; set; }

    /// <summary>
    /// Consul集群端口
    /// </summary>
    public int ConsulPort { get; set; }

    /// <summary>
    /// 负载均衡策略
    /// </summary>
    public string LBStrategy { get; set; }

    /// <summary>
    /// 权重
    /// </summary>
    public int? Weight { get; set; }
}
