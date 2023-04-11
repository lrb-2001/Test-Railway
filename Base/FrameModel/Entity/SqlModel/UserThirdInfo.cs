using SqlSugar;

namespace FrameModel;

/// <summary>
/// 第三方信息表
/// </summary>
public class UserThirdInfo
{
    /// <summary>
    /// 
    /// </summary>
    public UserThirdInfo()
    {
    }

    private System.Int32 _ID;
    /// <summary>
    /// ID
    /// </summary>
    [SugarColumn(IsPrimaryKey =true,IsIdentity =true)]
    public System.Int32 ID { get { return this._ID; } set { this._ID = value; } }

    private System.Int32? _UserID;
    /// <summary>
    /// 用户ID
    /// </summary>
    public System.Int32? UserID { get { return this._UserID; } set { this._UserID = value; } }

    private System.Int32? _ThirdValue;
    /// <summary>
    /// 第三方系统
    /// </summary>
    public System.Int32? ThirdValue { get { return this._ThirdValue; } set { this._ThirdValue = value; } }

    private System.String _Identity1;
    /// <summary>
    /// 标志1
    /// </summary>
    public System.String Identity1 { get { return this._Identity1; } set { this._Identity1 = value; } }

    private System.String _Identity2;
    /// <summary>
    /// 标志2
    /// </summary>
    public System.String Identity2 { get { return this._Identity2; } set { this._Identity2 = value; } }

    private System.String _Identity3;
    /// <summary>
    /// 标志3
    /// </summary>
    public System.String Identity3 { get { return this._Identity3; } set { this._Identity3 = value; } }

    private System.String _Identity4;
    /// <summary>
    /// 标志4
    /// </summary>
    public System.String Identity4 { get { return this._Identity4; } set { this._Identity4 = value; } }

    private System.String _Identity5;
    /// <summary>
    /// 标志5
    /// </summary>
    public System.String Identity5 { get { return this._Identity5; } set { this._Identity5 = value; } }

    private System.DateTime? _CreateTime;
    /// <summary>
    /// 创建时间
    /// </summary>
    public System.DateTime? CreateTime { get { return this._CreateTime; } set { this._CreateTime = value; } }

    private System.DateTime? _UpdateTime;
    /// <summary>
    /// 更新时间
    /// </summary>
    public System.DateTime? UpdateTime { get { return this._UpdateTime; } set { this._UpdateTime = value; } }

    private System.String _OpenID;
    /// <summary>
    /// OpenID
    /// </summary>
    public System.String OpenID { get { return this._OpenID; } set { this._OpenID = value; } }
}
