using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FrameCommon;

public static class HttpHelper
{
    #region 获取外网接口内容n result.Result;

    /// <summary>
    /// Get 请求
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public static async Task<string> Get(string url)
    {
        var httpClientHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true
        };
        var result = string.Empty;
        using (HttpClient client = new HttpClient(httpClientHandler))
        {
            var res = await client.GetAsync(url);
            if (res.IsSuccessStatusCode)
            {
                result = await res.Content.ReadAsStringAsync();
            }
        }
        return result;
    }
    /// <summary>
    /// Post 请求
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public static async Task<string> Post(string url, string body)
    {
        var httpClientHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true
        };
        var result = string.Empty;
        using (HttpClient client = new HttpClient(httpClientHandler))
        {
            var content = new StringContent(body == null ? "" : body);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await client.PostAsync(url, content);
            if (response.IsSuccessStatusCode)
            {
                result = await response.Content.ReadAsStringAsync();
            }
        }
        return result;
    }
    #endregion

}
