using FrameCommon.Hepler;
using FrameModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace FrameCommon;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            var statusCode = (StatusCode)context.Response.StatusCode;
            if (ex is ArgumentException)
            {
                statusCode = StatusCode.CODE500;
            }
            await HandleExceptionAsync(context, statusCode, ex.Message);
        }
        finally
        {
            if (StatusCode.CODE200 != (StatusCode)context.Response.StatusCode)
            {
                var statusCode = (StatusCode)context.Response.StatusCode;
                await HandleExceptionAsync(context, statusCode);
            }
        }

    }
    //异常错误信息捕获，将错误信息用Json方式返回

    private static Task HandleExceptionAsync(HttpContext context, StatusCode statusCode, string msg = null)
    {
        var result = JsonConvert.SerializeObject(new ApiResponse(statusCode, msg).MessageModel);
        context.Response.ContentType = "application/json;charset=utf-8";
        return context.Response.WriteAsync(result);

    }
}
//扩展方法

public static class ErrorHandlingExtensions
{
    public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ErrorHandlingMiddleware>();
    }
}