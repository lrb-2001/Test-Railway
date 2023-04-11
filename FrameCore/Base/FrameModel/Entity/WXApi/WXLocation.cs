namespace FrameModel;

public class WXLocation: WXResultBase
{
    /// <summary>
    /// 纬度
    /// </summary>
    public string Latitude { get; set; }

    /// <summary>
    /// 经度
    /// </summary>
    public string Longitude { get; set; }

    /// <summary>
    /// 精度
    /// </summary>
    public string Precision { get; set; }
}
