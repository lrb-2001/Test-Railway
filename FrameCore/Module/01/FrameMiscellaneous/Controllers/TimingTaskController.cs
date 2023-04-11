using FrameCore.Controllers;
using FrameIService;
using FrameModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FrameMiscellaneous.Controllers;

/// <summary>
/// 定时任务控制器
/// </summary>
public class TimingTaskController : BaseController
{
    private readonly ITimingTaskService timingTaskService;

    public TimingTaskController(ITimingTaskService _timingTaskService)
    {
        timingTaskService = _timingTaskService;
    }

    /// <summary>
    /// 清理日志
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    public async Task<ResultModel<long>> CleanLog()
    {
        return await timingTaskService.CleanLog();
    }
}