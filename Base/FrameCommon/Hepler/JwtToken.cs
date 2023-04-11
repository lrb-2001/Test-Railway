using FrameModel;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FrameCommon.Hepler;

public class JwtToken
{
    /// <summary>
    /// 获取基于JWT的Token
    /// </summary>
    /// <param name="claims">需要在登陆的时候配置</param>
    /// <param name="permissionRequirement">在startup中定义的参数</param>
    /// <returns></returns>
    public static TokenInfoModel BuildJwtToken(Claim[] claims)
    {
        //生成对称秘钥
        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppSettings.app(new string[] { "JWT", "Secret" })));
        //初始化签名凭证
        SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        double expire = Convert.ToDouble(AppSettings.app(new string[] { "JWT", "Expired" }));//Token过期时间
                                                                                             // 实例化JwtSecurityToken
        var token = new JwtSecurityToken(
            issuer: AppSettings.app(new string[] { "JWT", "Issuer" }),//设置签发者
            audience: AppSettings.app(new string[] { "JWT", "Audience" }),//设置接收者
            claims: claims,//设置payload
            expires: DateTime.Now.AddMinutes(expire),//5分钟有效期
            signingCredentials: creds);//初始化安全令牌参数
                                       // 生成 Token
        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(token);
        //打包返回前台
        var responseJson = new TokenInfoModel
        {
            Success = true,
            Token = encodedJwt,
            ExpiresIn = expire
        };
        return responseJson;
    }

}
