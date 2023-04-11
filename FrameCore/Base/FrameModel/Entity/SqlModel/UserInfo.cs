using SqlSugar;

namespace FrameModel;

/// <summary>
/// 用户表
/// </summary>
public class UserInfo
{
    /// <summary>
    /// 用户表
    /// </summary>
    public UserInfo()
    {
    }

    private System.Int64 _ID;
    /// <summary>
    /// 用户ID
    /// </summary>
    [SugarColumn(IsPrimaryKey =true,IsIdentity =true)]
    public System.Int64 ID { get { return this._ID; } set { this._ID = value; } }

    private System.String _NickName;
    /// <summary>
    /// 昵称
    /// </summary>
    public System.String NickName { get { return this._NickName; } set { this._NickName = value; } }

    private System.String _Password;
    /// <summary>
    /// 密码
    /// </summary>
    public System.String Password { get { return this._Password; } set { this._Password = value; } }

    private System.String _Photo;
    /// <summary>
    /// 头像
    /// </summary>
    public System.String Photo { get { return this._Photo; } set { this._Photo = value; } }

    private System.String _UserName;
    /// <summary>
    /// 名称
    /// </summary>
    public System.String UserName { get { return this._UserName; } set { this._UserName = value; } }

    private System.Int32? _Sex;
    /// <summary>
    /// 性别
    /// </summary>
    public System.Int32? Sex { get { return this._Sex; } set { this._Sex = value; } }

    private System.String _Email;
    /// <summary>
    /// 邮箱
    /// </summary>
    public System.String Email { get { return this._Email; } set { this._Email = value; } }

    private System.String _Province;
    /// <summary>
    /// 省
    /// </summary>
    public System.String Province { get { return this._Province; } set { this._Province = value; } }

    private System.String _City;
    /// <summary>
    /// 市
    /// </summary>
    public System.String City { get { return this._City; } set { this._City = value; } }

    private System.String _County;
    /// <summary>
    /// 县
    /// </summary>
    public System.String County { get { return this._County; } set { this._County = value; } }

    private System.String _Address;
    /// <summary>
    /// 详细地址
    /// </summary>
    public System.String Address { get { return this._Address; } set { this._Address = value; } }

    private System.String _Phone;
    /// <summary>
    /// 电话
    /// </summary>
    public System.String Phone { get { return this._Phone; } set { this._Phone = value; } }

    private System.Int32? _RoleType;
    /// <summary>
    /// 角色类型
    /// </summary>
    public System.Int32? RoleType { get { return this._RoleType; } set { this._RoleType = value; } }

    private System.String _RoleName;
    /// <summary>
    /// 角色名称
    /// </summary>
    public System.String RoleName { get { return this._RoleName; } set { this._RoleName = value; } }

    private System.Boolean? _IsOnline;
    /// <summary>
    /// 是否在线
    /// </summary>
    public System.Boolean? IsOnline { get { return this._IsOnline; } set { this._IsOnline = value; } }

    private System.Boolean? _Disabled;
    /// <summary>
    /// 是否禁用
    /// </summary>
    public System.Boolean? Disabled { get { return this._Disabled; } set { this._Disabled = value; } }

    private System.DateTime? _CreateTime;
    /// <summary>
    /// 创建时间
    /// </summary>
    public System.DateTime? CreateTime { get { return this._CreateTime; } set { this._CreateTime = value; } }

    private System.DateTime? _UpdateTime;
    /// <summary>
    /// 修改时间
    /// </summary>
    public System.DateTime? UpdateTime { get { return this._UpdateTime; } set { this._UpdateTime = value; } }

    private System.String _Openid;
    /// <summary>
    /// Openid
    /// </summary>
    public System.String Openid { get { return this._Openid; } set { this._Openid = value; } }
}
