using FrameModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FrameCommon;

public class LogAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        context.Result = new ObjectResult(new ResultModel<string>()
        {
            Status = (int)StatusCode.CODE500,
            Msg = $"LogAttribute: {context.Exception.Message}"
        });
    }

}
