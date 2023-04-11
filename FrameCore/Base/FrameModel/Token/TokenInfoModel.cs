namespace FrameModel;

public class TokenInfoModel
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }
    /// <summary>
    /// Token
    /// </summary>
    public string Token { get; set; }
    /// <summary>
    /// 有效时间(S)
    /// </summary>
    public double ExpiresIn { get; set; }
    /// <summary>
    /// Token类型
    /// </summary>
    public string TokenType { get; set; }
}
