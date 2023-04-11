using FrameModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FrameCore.Controllers;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("api/[controller]/[action]")]
[Authorize(AuthenticationSchemes = "Bearer,Developer")]//Bearer -- JWT默认验证，Developer -- 自定义开发者验证
public class BaseController : ControllerBase
{
    /// <summary>
    /// 测试
    /// </summary>
    [HttpPost]
    private ResultModel<string> Test()
    {
        return new ResultModel<string>();
    }
}