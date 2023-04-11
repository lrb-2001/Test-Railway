using System.Collections.Generic;

namespace FrameModel;

/// <summary>
/// 高德地图API返回的地址类
/// </summary>
public class GDAddress
{
    /// <summary>
    /// 返回结果状态值  返回值为 0 或 1，0 表示请求失败；1 表示请求成功。
    /// </summary>
    public string status { get; set; }

    /// <summary>
    /// 逆地理编码列表
    /// </summary>
    public Regeocode regeocode { get; set; }

    /// <summary>
    /// 返回状态说明  当 status 为 0 时，info 会返回具体错误原因，否则返回“OK”。
    /// </summary>
    public string info { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string infoCode { get; set; }
}

public class Regeocode
{
    /// <summary>
    /// 地址元素列表
    /// </summary>
    public AddressComponent addressComponent { get; set; }

    /// <summary>
    /// 详细地址
    /// </summary>
    public string formatted_address { get; set; }

}

public class AddressComponent
{
    /// <summary>
    /// 坐标点所在城市名称
    /// </summary>
    public string city { get; set; }

    /// <summary>
    /// 坐标点所在省名称
    /// </summary>
    public string province { get; set; }

    /// <summary>
    /// 行政区编码
    /// </summary>
    public string adcode { get; set; }

    /// <summary>
    /// 坐标点所在区
    /// </summary>
    public string district { get; set; }

    /// <summary>
    /// 乡镇街道编码
    /// </summary>
    public string towncode { get; set; }

    /// <summary>
    /// 门牌信息列表
    /// </summary>
    public StreetNumber streetNumber { get; set; }

    /// <summary>
    /// 国家
    /// </summary>
    public string country { get; set; }

    /// <summary>
    /// 坐标点所在乡镇/街道（此街道为社区街道，不是道路信息）
    /// </summary>
    public string township { get; set; }

    /// <summary>
    /// 经纬度所属商圈列表
    /// </summary>
    public List<BusinessAreasItem> businessAreas { get; set; }

    /// <summary>
    /// 楼信息列表
    /// </summary>
    public Building building { get; set; }

    /// <summary>
    /// 社区信息列表
    /// </summary>
    public Neighborhood neighborhood { get; set; }

    /// <summary>
    /// 城市编码
    /// </summary>
    public string citycode { get; set; }

}

public class Neighborhood
{
    /// <summary>
    /// 社区名称
    /// </summary>
    public List<string> name { get; set; }

    /// <summary>
    /// POI类型
    /// </summary>
    public List<string> type { get; set; }

}

public class Building
{
    /// <summary>
    /// 建筑名称
    /// </summary>
    public List<string> name { get; set; }

    /// <summary>
    /// 类型
    /// </summary>
    public List<string> type { get; set; }

}

public class BusinessAreasItem
{
    /// <summary>
    /// 商圈中心点经纬度
    /// </summary>
    public string location { get; set; }

    /// <summary>
    /// 商圈名称  例如：颐和园 
    /// </summary>
    public string name { get; set; }

    /// <summary>
    ///  商圈所在区域的adcode 例如：朝阳区/海淀区 
    /// </summary>
    public string id { get; set; }

}

public class StreetNumber
{
    /// <summary>
    /// 门牌号
    /// </summary>
    public string number { get; set; }

    /// <summary>
    /// 坐标点
    /// </summary>
    public string location { get; set; }

    /// <summary>
    /// 方向
    /// </summary>
    public string direction { get; set; }

    /// <summary>
    /// 门牌地址到请求坐标的距离
    /// </summary>
    public string distance { get; set; }

    /// <summary>
    /// 街道名称
    /// </summary>
    public string street { get; set; }

}
