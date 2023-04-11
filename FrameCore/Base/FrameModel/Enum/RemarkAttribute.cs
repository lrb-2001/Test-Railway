using System;

namespace FrameModel;

public class RemarkAttribute: Attribute
{
    public RemarkAttribute(string _remarks)
    {
        remarks = _remarks;
    }

    /// <summary>
    /// 备注
    /// </summary>
    public string remarks { get; set; }


    /// <summary>
    /// 返回备注
    /// </summary>
    /// <returns></returns>
    public string GetRemarks()
    {
        return this.remarks;
    }
}
