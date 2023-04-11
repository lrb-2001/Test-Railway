using FrameModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FrameCommon;

public class MpHelper
{
    /// <summary>
    /// 
    /// </summary>
    public static string APPID = GlobalConfig.frameCoreAgileConfig.weiXinMPConfig.AppId;
    /// <summary>
    /// 
    /// </summary>
    public static string SECRET = GlobalConfig.frameCoreAgileConfig.weiXinMPConfig.AppSecret;
    /// <summary>
    ///expire_seconds：有效时间   action_name：{QR_SCENE（临时）、QR_LIMIT_SCENE（永久）}
    /// </summary>
    public const string WeiXin_QrCodeTicket_Create_JsonString = "{\"expire_seconds\":604800,\"action_name\":\"QR_STR_SCENE\",\"action_info\":{\"scene\":{\"scene_str\":\"$*$\"}}}";


    /// <summary>
    /// 发送通知
    /// </summary>
    /// <param name="OpenId"></param>
    /// <param name="TemplateId"></param>
    /// <param name="Urls"></param>
    /// <param name="Msgbody"></param>
    /// <param name="Pagepath"></param>
    /// <returns></returns>
    public static async Task<string> SendTemplate(string OpenId, string TemplateId, string Urls, Dictionary<string, msgBody> Msgbody, string Pagepath = "")
    {
        var msgData = new
        {
            touser = OpenId,
            template_id = TemplateId,
            url = Urls,
            data = Msgbody
        };
        string token = GetAccessToken(APPID, SECRET);
        var url = GlobalConfig.frameCoreAgileConfig.weiXinMPConfig.SendTemplateUrl + "?access_token=" + token;

        var res = await HttpHelper.Post(url, JsonConvert.SerializeObject(msgData));
        JObject _jObject = JsonConvert.DeserializeObject<JObject>(res);
        string errcode = _jObject["errcode"].ToString();    //取值
        return errcode;
    }


    /// <summary>
    /// 关注公众号回调方法
    /// </summary>
    /// <returns></returns>
    //public static async Task<string> WxSubscribe()
    //{
    //    _repository.Add<UserThirdInfo>();
    //}

    #region 私有方法
    /// <summary>
    /// 获取Token
    /// </summary>
    /// <param name="appid"></param>
    /// <param name="secret"></param>
    /// <returns></returns>
    public static string GetAccessToken(string appid, string secret)
    {
        var url = GlobalConfig.frameCoreAgileConfig.weiXinMPConfig.AccessTokenUrl + "?grant_type=client_credential&appid=" + appid + "&secret=" + secret;
        var res = HttpHelper.Get(url).Result;
        WXAccessToken token = JsonConvert.DeserializeObject<WXAccessToken>(res);
        return token.access_token;
    }


    /// <summary>
    /// 获取Ticket
    /// </summary>
    /// <returns></returns>
    public static async Task<string> CreateTicket(string scene_id)
    {
        var Access_Token = GetAccessToken(APPID, SECRET);//access_token

        string url = GlobalConfig.frameCoreAgileConfig.weiXinMPConfig.CreateTicketUrl + "?access_token=" + Access_Token;

        string postData = WeiXin_QrCodeTicket_Create_JsonString.Replace("$*$", scene_id);

        var result = await HttpHelper.Post(url, postData);

        TicketDTO ticket = JsonConvert.DeserializeObject<TicketDTO>(result); //HttpClientHelper.PostResponse<Ticket>(url, postData);

        if (ticket == null || string.IsNullOrEmpty(ticket.ticket))
        {
            return "";
        }
        return ticket.ticket;
    }

    /// <summary>
    /// 验证
    /// </summary>
    /// <returns></returns>
    public static bool CheckSignature(string signature, string timestamp, string nonce, string token)
    {
        SortedDictionary<string, string> sort = new SortedDictionary<string, string>();
        sort.Add("token", token);
        sort.Add("timestamp", timestamp);
        sort.Add("nonce", nonce);
        var str = "";
        foreach (var item in sort.OrderBy(c => c.Value))
        {
            str += item.Value;
        }
        SHA1 sHA1 = SHA1.Create();
        var hash = sHA1.ComputeHash(Encoding.UTF8.GetBytes(str));
        string shaStr = BitConverter.ToString(hash);
        shaStr = shaStr.Replace("-", "");

        if (signature == shaStr.ToLower())

        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 解析XML
    /// </summary>
    /// <returns></returns>
    public static WXResultBase GetRequestEntity(XmlDocument doc)
    {
        WXResultBase xmlDate = new WXResultBase();

        XmlNode rootNode = doc.SelectSingleNode("xml");
        XmlNodeList firstLevelNodeList = rootNode.ChildNodes;
        foreach (XmlNode node in firstLevelNodeList)
        {
            Type types = typeof(WXResultBase);

            foreach (PropertyInfo item in types.GetProperties())
            {
                if (item.Name == node.LocalName)
                {
                    types.GetProperty(node.LocalName).SetValue(xmlDate, node.InnerText);
                    //获取属性名称与属性值
                    Console.WriteLine("{0} = {1}", node.LocalName, node.InnerText);
                    break;
                }
            }
        }


        return xmlDate;

    }

    /// <summary>
    /// 解析XML
    /// </summary>
    /// <typeparam name="T">返回类型</typeparam>
    /// <param name="doc">XML内容</param>
    /// <param name="model">返回模型</param>
    public static void GetRequestEntity<T>(XmlDocument doc, ref T model) where T : new()
    {
        T xmlDate = new T();

        XmlNode rootNode = doc.SelectSingleNode("xml");
        XmlNodeList firstLevelNodeList = rootNode.ChildNodes;
        foreach (XmlNode node in firstLevelNodeList)
        {
            Type types = typeof(T);

            foreach (PropertyInfo item in types.GetProperties())
            {
                if (item.Name == node.LocalName)
                {
                    types.GetProperty(node.LocalName).SetValue(xmlDate, node.InnerText);
                    //获取属性名称与属性值
                    Console.WriteLine("{0} = {1}", node.LocalName, node.InnerText);
                    break;
                }
            }
        }
        model = xmlDate;
    }

    #endregion

}
