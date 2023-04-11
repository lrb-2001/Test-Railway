using FrameModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace FrameCommon;

/// <summary>
/// 过滤器    
/// IAsyncActionFilter - 方法过滤器 , IAsyncExceptionFilter - 异常过滤器 , 
/// IAsyncAuthorizationFilter - 授权过滤器 , IAsyncResourceFilter - 资源过滤器
/// IAsyncResultFilter - 结果格式过滤器 , 
/// </summary>
public class BaseFilter : IAsyncExceptionFilter
{

    /// <summary>
    /// 异常捕获 过滤
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task OnExceptionAsync(ExceptionContext context)
    {

        var result = new ResultModel<string>()
        {
            Status = 500,
            Success = false,
            Msg = $"服务异常！Exception-->> {context.HttpContext.Request.Path} ",
            Data = context.Exception.Message
        };

        LogLock.Error(context.Exception.Message, context.HttpContext.Request.Path);

        context.Result = new ObjectResult(result);
        await Task.CompletedTask;
    }

}
