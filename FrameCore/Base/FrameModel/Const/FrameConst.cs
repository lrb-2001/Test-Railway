using Microsoft.AspNetCore.Authentication;

namespace FrameModel;

public class FrameConst : AuthenticationSchemeOptions
{

    /// <summary>
    /// 默认登录
    /// </summary>
    public const string HttpParaKey_Token = "oauth2";
    /// <summary>
    /// 调试登录
    /// </summary>
    public const string Developer = "Developer";

}
