using FrameModel;

namespace FrameCommon.Hepler;

public class ApiResponse
{
    public int Status { get; set; } = 404;
    public string Value { get; set; } = "No Found";
    public ResultModel<string> MessageModel = new ResultModel<string>() { };

    public ApiResponse(StatusCode apiCode, string msg = null)
    {
        switch (apiCode)
        {
            case StatusCode.CODE401:
                {
                    Status = 401;
                    Value = "您无权访问该接口，验证失败!";
                }
                break;
            case StatusCode.CODE403:
                {
                    Status = 403;
                    Value = "您的访问权限等级不够，联系管理员!";
                }
                break;
            case StatusCode.CODE429:
                {
                    Status = 429;
                    Value = "请勿频繁请求，请稍后再试！";
                }
                break;
            case StatusCode.CODE500:
                {
                    Status = 500;
                    Value = msg;
                }
                break;
        }

        MessageModel = new ResultModel<string>()
        {
            Status = Status,
            Msg = Value,
            Success = false
        };
    }
}