using FrameModel;
using System;
using System.Threading.Tasks;

namespace FrameIService
{
    public interface IHomeService
    {
        /// <summary>
        /// Redis添加数据
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="val">值</param>
        /// <param name="timeSpan">过期时间</param>
        public Task<ResultModel<string>> TestAdd(string key, string val,TimeSpan? timeSpan);

        /// <summary>
        /// Redis删除数据
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public Task<ResultModel<string>> TestDel(string key);

        /// <summary>
        /// 获取Token
        /// </summary>
        /// <param name="name">账号</param>
        /// <param name="pwd">密码</param>
        /// <returns></returns>
        public Task<ResultModel<TokenInfoModel>> GetToken(TokenRequest request);

        /// <summary>
        /// 计算验证 -- 获取算式
        /// </summary>
        /// <returns></returns>
        public Task<ResultModel<string>> GetComputingValidation();

        /// <summary>
        /// 判断验证码是否正确
        /// </summary>
        /// <param name="val">验证码</param>
        /// <returns></returns>
        public Task<ResultModel<bool>> IsComputingValidation(string val);

        /// <summary>
        /// 测试方法
        /// </summary>
        /// <returns></returns>
        public Task<ResultModel<UserInfo>> ActionTest();
    }
}
