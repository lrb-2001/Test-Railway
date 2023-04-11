using Microsoft.AspNetCore.Http;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace FrameCommon;

/// <summary>
/// Http中间件
/// </summary>
public class HttpMiddleware
{
    private readonly RequestDelegate _next;
    private Stopwatch _stopwatch;
    public HttpMiddleware(RequestDelegate next)
    {
        _next = next;
        _stopwatch = new Stopwatch();
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        _stopwatch.Restart();
        var request = httpContext.Request.Body;
        var response = httpContext.Response.Body;

        #region 获取 请求 和 响应
        var req = string.Empty;
        var res = string.Empty;

        try
        {
            using (var newRequest = new MemoryStream())
            {
                //替换request流
                httpContext.Request.Body = newRequest;

                using (var newResponse = new MemoryStream())
                {
                    //替换response流
                    httpContext.Response.Body = newResponse;

                    using (var reader = new StreamReader(request))
                    {
                        //读取原始请求流的内容
                        req = await reader.ReadToEndAsync();

                        //示例加密字符串，使用 AES-ECB-PKCS7 方式加密，密钥为：0123456789abcdef
                        //api.Body = SecurityHelper.AESDecrypt(api.Body, securitykey);
                    }
                    using (var writer = new StreamWriter(newRequest))
                    {
                        await writer.WriteAsync(req);
                        await writer.FlushAsync();
                        newRequest.Position = 0;
                        httpContext.Request.Body = newRequest;
                        await _next.Invoke(httpContext);
                    }

                    using (var reader = new StreamReader(newResponse))
                    {
                        newResponse.Position = 0;
                        res = await reader.ReadToEndAsync();
                        if (!string.IsNullOrWhiteSpace(res))
                        {
                            //api.ResponseBody = SecurityHelper.AESEncrypt(api.ResponseBody, securitykey);
                        }
                    }
                    using (var writer = new StreamWriter(response))
                    {
                        await writer.WriteAsync(res);
                        await writer.FlushAsync();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            LogLock.Error($" http中间件发生错误: " + ex.ToString(), httpContext.Request.Path);
        }
        finally
        {
            httpContext.Request.Body = request;
            httpContext.Response.Body = response;
        }
        _stopwatch.Stop();
        //LogLock.Debug(req, httpContext.Request.Path, res);
        #endregion

    }
}
