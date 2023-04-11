using FrameModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameIService
{
    public interface IWXApiService
    {
        /// <summary>
        /// 微信回调
        /// </summary>
        /// <returns></returns>
        public Task<string> WXCallback(System.Xml.XmlDocument requestDoc, string Event);

        /// <summary>
        /// 微信获取用户信息
        /// </summary>
        /// <param name="code">用于换取token的</param>
        /// <param name="state">重定向的参数</param>
        /// <returns></returns>
        public Task<ResultModel<UserInfo>> WXOauthCallback(string code, string state);

        public UserInfo QueryUserByOpenID(string openid);
    }
}
