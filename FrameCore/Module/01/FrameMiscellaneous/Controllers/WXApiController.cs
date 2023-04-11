using FrameCommon;
using FrameCore.Controllers;
using FrameIService;
using FrameModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Web;

namespace FrameMiscellaneous.Controllers;

/// <summary>
/// 微信接口
/// </summary>
public class WXApiController : BaseController
{
    private readonly IWXApiService _service;
    private IMemoryCache _cache;//缓存
    public WXApiController(IWXApiService service, IMemoryCache cache)
    {
        _service = service;
        _cache = cache;
    }
    #region 公众号

    /// <summary>
    /// 模板通知
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    public async Task<ResultModel<bool>> Inform(string openid)
    {
        var urls = GlobalConfig.frameCoreAgileConfig.externalUrl;
        ResultModel<bool> res = new ResultModel<bool>();
        Dictionary<string, msgBody> msgBody = new Dictionary<string, msgBody>();
        //1.1 实体添加
        msgBody.Add("time", new msgBody()
        {
            value = DateTime.Now.ToString("yyyy-MM-dd") + "   " + BaseHelper.DayToWeek(DateTime.Now.DayOfWeek),
            color = "#0f95b0"
        });
        var morningRes = await HttpHelper.Get(urls.OneWord);
        JObject _jObject = JsonConvert.DeserializeObject<JObject>(morningRes);
        var morning = _jObject["data"]["content"].ToString();
        msgBody.Add("morning", new msgBody()
        {
            value = morning,
            color = "#5698c3"
        });
        msgBody.Add("mood", new msgBody()
        {
            value = "又是美好的一天！！！",
            color = "#00BFFF"
        });

        UserInfo user = _service.QueryUserByOpenID(openid);
        msgBody.Add("city", new msgBody()
        {
            value = user.City,
            color = "#66c18c"
        });

        var weatherRes = await HttpHelper.Get(urls.WeekWeather + $"?msg={user.City}&type=1");
        _jObject = JsonConvert.DeserializeObject<JObject>(weatherRes);
        var weather = _jObject["data"][1]["tianqi"].ToString() + "  " + _jObject["data"][1]["wendu"].ToString() + "℃" + "  " +
             _jObject["data"][1]["fengdu"].ToString() + "   PM:" + _jObject["data"][1]["pm"].ToString();
        msgBody.Add("weather", new msgBody()
        {
            value = weather,
            color = "#8A2BE2"
        });

        var inWordRes = await HttpHelper.Get(urls.OneVerse);
        _jObject = JsonConvert.DeserializeObject<JObject>(inWordRes);
        string author = _jObject["author"].ToString();
        string inWord = _jObject["content"].ToString() + $"    ---《{_jObject["origin"].ToString()}》——{author}";
        msgBody.Add("inWord", new msgBody()
        {
            value = inWord,
            color = "#66c18c"
        });


        //1.2 发送
        string ret = await MpHelper.SendTemplate(openid, GlobalConfig.frameCoreAgileConfig.weiXinMPConfig.TimedNotifKey, "", msgBody);
        if (ret == "0")
        {
            res.Msg = "公众号发送成功!";
            res.Success = true;
            res.Data = true;
        }
        else
        {
            res.Msg = "公众号发送失败!";
        }
        return res;
    }

    /// <summary>
    /// 获取临时场景二维码带场景值
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    public async Task<ResultModel<SceneCodeRes>> MPQRcodeSceneValue()
    {
        SceneCodeRes res = new SceneCodeRes();
        string Scene_Id = Guid.NewGuid().ToString().Replace("-", "");
        string Ticket = await MpHelper.CreateTicket(Scene_Id);

        if (Ticket == null)
            throw new ArgumentNullException("Ticket");

        res = new SceneCodeRes()
        {
            SceneValue = Scene_Id,
            Code = GlobalConfig.frameCoreAgileConfig.weiXinMPConfig.SceneCodeUrl + "?ticket=" + HttpUtility.UrlEncode(Ticket) + "&"
        };

        return new ResultModel<SceneCodeRes>()
        {
            Msg = "二维码生成成功！",
            Success = true,
            Data = res
        };
    }

    #endregion

    /// <summary>
    /// 微信回调
    /// </summary>
    /// <returns></returns>
    [HttpPost, HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> WXApiService()
    {
        try
        {
            ActionResult actionResult = Content("");
            var signature = HttpContext.Request.Query["signature"];
            var timestamp = HttpContext.Request.Query["timestamp"];
            var nonce = HttpContext.Request.Query["nonce"];
            var echostr = HttpContext.Request.Query["echostr"];

            var token = "zheng5ling5ren5shang5xina5";//微信公众平台配置url的token

            //验证程序
            var methodType = HttpContext.Request.Method;
            if (methodType.Contains("GET"))
            {
                if (MpHelper.CheckSignature(signature, timestamp, nonce, token))
                {
                    actionResult = Content(echostr);
                }
            }
            else
            {
                if (!MpHelper.CheckSignature(signature, timestamp, nonce, token))
                {
                    LogLock.Info("验证失败！", "WXApiService");
                }
                //1.1 获取流信息
                var buffer = new byte[Convert.ToInt32(Request.ContentLength)];
                await Request.Body.ReadAsync(buffer, 0, buffer.Length);
                var body = Encoding.UTF8.GetString(buffer);
                if (!body.Contains("TEMPLATESENDJOBFINISH"))
                {
                    LogLock.Info("数据内容：" + body, "WXApiService");
                }
                //1.2 解析XML
                System.Xml.XmlDocument requestDoc = new System.Xml.XmlDocument();//新建对象
                requestDoc.LoadXml(body);
                WXResultBase responseBaseDate = MpHelper.GetRequestEntity(requestDoc);
                //1.3 判断消息类型
                switch (responseBaseDate.MsgType.ToUpper())
                {
                    case "EVENT":
                        await _service.WXCallback(requestDoc, responseBaseDate.Event);
                        break;
                    case "TEXT":; break;
                }
            }

            return actionResult;
        }
        catch (Exception e)
        {
            LogLock.Error("结束报错：" + e.Message, "WXApiService");
            return Content("");
        }
    }

    /// <summary>
    /// 微信获取用户信息的回调
    /// </summary>
    /// <param name="code">code作为换取access_token的票据</param>
    /// <param name="state">重定向后会带上 state 参数</param>
    /// <returns></returns>
    [HttpPost, HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> WXApiOauthService(string code = "", string state = "")
    {
        try
        {
            ActionResult actionResult = Content("");
            LogLock.Info($"code:{code},state:{state}", "WXApiService");

            UserInfo userInfo = (await _service.WXOauthCallback(code, state)).Data;
            return actionResult;
        }
        catch (Exception ex)
        {
            return Content(ex.Message);
        }
    }

    /// <summary>
    /// 获取微信授权Url
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    public ResultModel<string> GetOauthUrl()
    {
        var model = new ResultModel<string>();
        string redirect_uri = HttpUtility.UrlEncode("http://1.15.63.248/api/WXApi/WXApiOauthService", Encoding.UTF8);
        _cache.Set("redirect_uri", redirect_uri);
        string url = $"https://open.weixin.qq.com/connect/oauth2/authorize?appid={MpHelper.APPID}&redirect_uri={redirect_uri}&response_type=code&scope=snsapi_userinfo&state=STATE#wechat_redirect";
        var cache = _cache.Get("redirect_uri");
        model.Data = url;
        return model;
    }


}
