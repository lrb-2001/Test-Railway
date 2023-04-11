using FrameCommon;
using FrameIRepository;
using FrameIService;
using FrameModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FrameService
{
    public class WXApiService: IWXApiService
    {
        private readonly IBaseRepository _repository;
        public WXApiService(IBaseRepository repository)
        {
            _repository = repository;
            repository.connServer(GlobalConfig.frameCoreAgileConfig.connectionConfig.frameCon);
        }

        /// <summary>
        /// 微信回调
        /// </summary>
        /// <returns></returns>
        public async Task<string> WXCallback(XmlDocument requestDoc, string Event)
        {
            //判断事件类型
            switch (Event)
            {
                case "LOCATION"://获取用户地理位置

                    var WXMPConfig = GlobalConfig.frameCoreAgileConfig.weiXinMPConfig;
                    WXLocation location = new WXLocation();
                    MpHelper.GetRequestEntity<WXLocation>(requestDoc, ref location);
                    GDAddress address = GetAddress(location.Longitude,location.Latitude);
                    UserInfo user = QueryUserByOpenID(location.FromUserName);
                    LogLock.Info(JsonConvert.SerializeObject(address), "WXCallback");
                    if (user!=null)
                    {
                        user.Address = address.regeocode.formatted_address;
                        user.Province = address.regeocode.addressComponent.province;
                        user.City = address.regeocode.addressComponent.city;
                        user.County = address.regeocode.addressComponent.district;
                        await _repository.Update<UserInfo>(user);
                    }
                    break;
                case "subscribe": //用户订阅事件
                    WXSubscribe subscribe = new WXSubscribe();
                    MpHelper.GetRequestEntity<WXSubscribe>(requestDoc, ref subscribe);

                    break;
                case "unsubscribe"://用户取消订阅事件

                    break;
                case "SCAN"://已关注用户扫码进入事件

                    break;
                default:
                    break;
            }
            return "";
        }

        /// <summary>
        /// 微信获取用户信息
        /// </summary>
        /// <param name="code">用于换取token的</param>
        /// <param name="state">重定向的参数</param>
        /// <returns></returns>
        public async Task<ResultModel<UserInfo>> WXOauthCallback(string code, string state)
        {
            var model = new ResultModel<UserInfo>();
            string url = $"https://api.weixin.qq.com/sns/oauth2/access_token?appid={MpHelper.APPID}&secret={MpHelper.SECRET}&code={code}&grant_type=authorization_code";
            var res = await HttpHelper.Get(url);
            WXOauth oauth = JsonConvert.DeserializeObject<WXOauth>(res);
            url = $"https://api.weixin.qq.com/sns/auth?access_token={oauth.access_token}&openid={oauth.openid}";
            res = await HttpHelper.Get(url);
            JObject _jObject = JsonConvert.DeserializeObject<JObject>(res);
            if (_jObject["errcode"].ToString()!="0")
            {
                url = $"https://api.weixin.qq.com/sns/oauth2/access_token?appid={MpHelper.APPID}&refresh_token={oauth.refresh_token}&grant_type=refresh_token";
                res = await HttpHelper.Get(url);
                oauth = JsonConvert.DeserializeObject<WXOauth>(res);
            }
            url = $"https://api.weixin.qq.com/sns/userinfo?access_token={oauth.access_token}&openid={oauth.openid}&lang=zh_CN";
            res = await HttpHelper.Get(url);
            WXUserInfo info = JsonConvert.DeserializeObject<WXUserInfo>(res);
            UserInfo user = QueryUserByOpenID(info.openid);
            if (user == null)
            {
                user.NickName = info.nickname;
                user.Sex = info.sex;
                user.Openid = info.openid;
                user.CreateTime = DateTime.Now;
                user.UpdateTime = DateTime.Now;
                var result = await AddUserInfo(user);
                if (result>0)
                {
                    return new ResultModel<UserInfo>() {
                        Msg="添加用户成功",
                        Data= QueryUserByOpenID(info.openid)
                    };
                }
            }
            model.Data = user;
            return model;
        }

        public UserInfo QueryUserByOpenID(string openid)
        {
            return  _repository.Query<UserInfo>(n=>n.Openid==openid).Result.FirstOrDefault();
        }

        /// <summary>
        /// 添加用户第三方信息
        /// </summary>
        /// <returns></returns>
        public async Task<int> AddUserThirdInfo(UserThirdInfo info)
        {
            return await _repository.Add<UserThirdInfo>(info);
        }
       
        /// <summary>
        /// 添加用户
        /// </summary>
        /// <returns></returns>
        public async Task<int> AddUserInfo(UserInfo info)
        {
            return await _repository.Add<UserInfo>(info);
        }


        /// <summary>
        /// 通过高德地图API 经纬度转地址
        /// </summary>
        /// <param name="lon">经度</param>
        /// <param name="loc">维度</param>
        /// <returns></returns>
        public GDAddress GetAddress(string lon,string loc)
        {
            string url = GlobalConfig.frameCoreAgileConfig.externalUrl.GdAddressByLocation+ $"?key={GlobalConfig.frameCoreAgileConfig.externalUrl.GdKey}&location={lon},{loc}";
            var res = HttpHelper.Get(url).Result;
            LogLock.Info("GetAddress:" + res , "GetAddress");
            GDAddress address = JsonConvert.DeserializeObject<GDAddress>(res);
            return address;
        }
    }
}
