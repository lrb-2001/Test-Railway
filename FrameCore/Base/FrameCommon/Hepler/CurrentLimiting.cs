using FrameModel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace FrameCommon.Hepler;

public class CurrentLimiting
{
    private readonly IMemoryCache memoryCache;
    private readonly RequestDelegate requestDelegate;
    public int Limit = Convert.ToInt32(AppSettings.app(new string[] { "Limiting", "limit" }));//一分钟内可以的请求次数

    public CurrentLimiting(RequestDelegate requestDelegate, IMemoryCache memoryCache)
    {
        this.requestDelegate = requestDelegate;
        this.memoryCache = memoryCache;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var requestKey = $"{context.Request.Method}-{context.Request.Path}";

        int requestCount = 0;

        var limitTime = double.Parse(AppSettings.app(new string[] { "Limiting", "limitTime" }));

        var cacheOptions = new MemoryCacheEntryOptions()
        {
            AbsoluteExpiration = DateTime.Now.AddSeconds(limitTime)//设置多长时间内
        };

        if (memoryCache.TryGetValue(requestKey, out requestCount))
        {
            if (requestCount < Limit)
            {
                await ProcessRequest(context, requestKey, requestCount, cacheOptions);
            }
            else
            {
                context.Response.Headers["X-RateLimit-Retry-After"] = cacheOptions.AbsoluteExpiration?.ToString();//超出限制后，再次正常访问时间
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsync(JsonConvert.SerializeObject((new ApiResponse(StatusCode.CODE429)).MessageModel));
            }
        }
        else
        {
            await ProcessRequest(context, requestKey, requestCount, cacheOptions);
        }
    }


    private async Task ProcessRequest(HttpContext context, string requestKey, int requestCount, MemoryCacheEntryOptions memoryCacheEntryOptions)
    {
        requestCount++;

        memoryCache.Set(requestKey, requestCount, memoryCacheEntryOptions);

        context.Response.Headers["X-RateLimit-LimitTime"] = AppSettings.app(new string[] { "Limiting", "limitTime" });//某一时间段

        context.Response.Headers["X-RateLimit-Limit"] = Limit.ToString();//同一时间段内的最大访问个数

        context.Response.Headers["X-RateLimit-Remaining"] = (Limit - requestCount).ToString();//同一时间段剩余请求次数

        await requestDelegate(context);

    }
}
