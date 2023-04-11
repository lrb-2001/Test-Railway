using FrameCore.Controllers;
using FrameIService;
using FrameModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FrameMiscellaneous.Controllers;

/// <summary>
/// Home
/// </summary>
public class HomeController : BaseController
{
    private readonly IHomeService _homeService;
    private readonly IConfiguration _configuration;
    public HomeController(IHomeService homeService, IConfiguration configuration)
    {
        _homeService = homeService;
        _configuration = configuration;
    }

    /// <summary>
    /// Redis添加数据
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="val">值</param>
    /// <param name="expiry">过期时间</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ResultModel<string>> TestAdd(string key, string val, double? expiry)
    {
        expiry = expiry == null ? 0 : expiry;
        return await _homeService.TestAdd(key, val, TimeSpan.FromSeconds((double)expiry));
    }

    /// <summary>
    /// Redis删除数据
    /// </summary>
    /// <param name="key">键</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ResultModel<string>> TestDel(string key)
    {
        return await _homeService.TestDel(key);
    }

    /// <summary>
    /// 获取Token
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    public async Task<ResultModel<TokenInfoModel>> GetToken(TokenRequest request)
    {
        return await _homeService.GetToken(request);
    }

    /// <summary>
    /// 计算验证 -- 获取算式
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    //[AllowAnonymous]
    public async Task<ResultModel<string>> GetComputingValidation()
    {
        return await _homeService.GetComputingValidation();
    }

    /// <summary>
    /// 判断验证码是否正确
    /// </summary>
    /// <param name="val">验证码</param>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    public async Task<ResultModel<bool>> IsComputingValidation(string val)
    {
        return await _homeService.IsComputingValidation(val);
    }

    /// <summary>
    /// 测试异常
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    public ResultModel<TokenInfoModel> ActionTestException(string id = "")
    {
        if (id == "1")
        {
            throw new Exception();
        }

        return new ResultModel<TokenInfoModel>()
        {
            Msg = AttributeEnum.Test2.GetTypeName()
        };
    }

    /// <summary>
    /// 测试方法
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    public async Task<ResultModel<UserInfo>> ActionTest()
    {
        return await _homeService.ActionTest();
    }

}
