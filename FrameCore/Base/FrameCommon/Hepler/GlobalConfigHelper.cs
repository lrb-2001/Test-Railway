using FrameModel;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace FrameCommon;


public class GlobalConfigHelper
{
    /// <summary>
    /// 初始化设置
    /// </summary>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static FrameCoreAgileConfig InitConfig(IConfiguration configuration)
    {
        var result = new FrameCoreAgileConfig();
        if (!string.IsNullOrEmpty(configuration.GetValue<string>(ConfigConst.BaseConfig)))
        {
            result.baseConfig = JsonConvert.DeserializeObject<BaseConfig>(configuration.GetValue<string>(ConfigConst.BaseConfig));
        }
        if (!string.IsNullOrEmpty(configuration.GetValue<string>(ConfigConst.ConnectionConfig)))
        {
            result.connectionConfig = JsonConvert.DeserializeObject<ConnectionConfig>(configuration.GetValue<string>(ConfigConst.ConnectionConfig));
        }
        if (!string.IsNullOrEmpty(configuration.GetValue<string>(ConfigConst.WeixinMPConfig)))
        {
            result.weiXinMPConfig = JsonConvert.DeserializeObject<WeiXinMPConfig>(configuration.GetValue<string>(ConfigConst.WeixinMPConfig));
        }
        if (!string.IsNullOrEmpty(configuration.GetValue<string>(ConfigConst.ExternalUrl)))
        {
            result.externalUrl = JsonConvert.DeserializeObject<ExternalUrl>(configuration.GetValue<string>(ConfigConst.ExternalUrl));
        }
        if (!string.IsNullOrEmpty(configuration.GetValue<string>(ConfigConst.OpenAIConfig)))
        {
            result.openAIConfig = JsonConvert.DeserializeObject<OpenAIConfig>(configuration.GetValue<string>(ConfigConst.OpenAIConfig));
        }
        return result;
    }
}