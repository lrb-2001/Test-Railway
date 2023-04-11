using FrameCommon;
using FrameCommon.Hepler;
using FrameIRepository;
using FrameIService;
using FrameModel;
using log4net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FrameService
{
    public class HomeService: IHomeService
    {
        private IBaseRepository _base;
        private IRedisRepository _redis;
        public HomeService(IRedisRepository redis, IBaseRepository repository)
        {
            _redis = redis;
            _base = repository;
            _base.connServer(GlobalConfig.frameCoreAgileConfig.connectionConfig.frameCon);
        }

        /// <summary>
        /// Redis添加数据
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="val">值</param>
        /// <param name="timeSpan">过期时间</param>
        /// <returns></returns>
        public async Task<ResultModel<string>> TestAdd(string key, string val,TimeSpan? timeSpan)
        {
            _redis.SetValue(key,val, timeSpan);
            ResultModel<string> result = new ResultModel<string>();
            result.Data = "调用成功！---->>> " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            result.Success = true;
            result.Msg = "成功！";
            await Task.CompletedTask;
            return result;
        }

        /// <summary>
        /// Redis删除数据
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public async Task<ResultModel<string>> TestDel(string key)
        {
            _redis.DeleteKey(key);
            ResultModel<string> result = new ResultModel<string>();
            result.Data = "调用成功！---->>> " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            result.Success = true;
            result.Msg = "成功！";
            await Task.CompletedTask;
            return result;
        }

        /// <summary>
        /// 获取Token
        /// </summary>
        /// <param name="name">账号</param>
        /// <param name="pwd">密码</param>
        /// <returns></returns>
        public async Task<ResultModel<TokenInfoModel>> GetToken(TokenRequest request)
        {
            //如果是基于用户的授权策略，这里要添加用户;如果是基于角色的授权策略，这里要添加角色
            var claims = new List<Claim> {
                        new Claim(ClaimTypes.Name, request.Name),
                        new Claim(JwtRegisteredClaimNames.Jti, request.Name),
                        new Claim(ClaimTypes.Expiration, DateTime.Now.AddSeconds(new TimeSpan().TotalSeconds).ToString()) };
            claims.AddRange(request.Name.Split(',').Select(s => new Claim(ClaimTypes.Role, s)));

            //用户标识
            var identity = new ClaimsIdentity(JwtBearerDefaults.AuthenticationScheme);
            identity.AddClaims(claims);

            var token = JwtToken.BuildJwtToken(claims.ToArray());
            await Task.CompletedTask;
            return new ResultModel<TokenInfoModel>()
            {
                Success = true,
                Msg = "获取成功",
                Data = token,
            };
        }

        /// <summary>
        /// 计算验证 -- 获取算式
        /// </summary>
        /// <returns></returns>
        public async Task<ResultModel<string>> GetComputingValidation()
        {
            string result = string.Empty;
            int diff = 1;
            char expression = char.MinValue;
            int Number = 0, BySeveral = 0;
            int[] NumArray = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };

            char[] Operators = { '＋', '﹣', '×', '÷' };

            Random random = new Random();

            Number = random.Next(1, diff <= 1 ? 9 : diff == 2 ? 99 : 999);
            BySeveral = random.Next(1, diff <= 1 ? 9 : diff == 2 ? 99 : 999);
            expression = Operators[random.Next(Operators.Length)];
            string res = string.Empty;
            switch (expression)
            {
                case '＋':
                    res = (Number + BySeveral).ToString();
                    break;
                case '﹣':
                    if (Number < BySeveral)
                    {
                        int num = Number;
                        Number = BySeveral;
                        BySeveral = num;
                    }
                    res = (Number - BySeveral).ToString();
                    break;
                case '×':
                    --diff;
                    BySeveral = random.Next(1, diff <= 1 ? 9 : diff == 2 ? 99 : 999);
                    res = (Number * BySeveral).ToString();
                    break;
                case '÷':
                    --diff;
                    BySeveral = random.Next(1, diff <= 1 ? 9 : diff == 2 ? 99 : 999);
                    if (Number < BySeveral)
                    {
                        int num = Number;
                        Number = BySeveral;
                        BySeveral = num;
                    }
                    Number = Number - (Number % BySeveral);
                    res = (Number / BySeveral).ToString();
                    break;
                default:
                    break;
            }
            result = Number.ToString() + " " + expression + " " + BySeveral.ToString();
            //AppSettings.write("VCCode", res);
            _redis.SetValue("VCCode", res, TimeSpan.FromMinutes(5));
            await Task.CompletedTask;
            return new ResultModel<string>()
            {
                Msg = "--->>> 调用成功",
                Success = true,
                Data = result
            };
        }

        /// <summary>
        /// 判断验证码是否正确
        /// </summary>
        /// <param name="val">验证码</param>
        /// <returns></returns>
        public async Task<ResultModel<bool>> IsComputingValidation(string val)
        {
            //bool result = val == AppSettings.read("VCCode");
            bool result = val ==_redis.GetValue("VCCode");
            await Task.CompletedTask;
            return new ResultModel<bool>()
            {
                Success = true,
                Msg = "--->>> 调用成功",
                Data = result
            };
        }
        
        /// <summary>
        /// 测试方法
        /// </summary>
        /// <returns></returns>
        public async Task<ResultModel<UserInfo>> ActionTest()
        {
            var res = (await _base.Query<UserInfo>().ToListAsync()).FirstOrDefault();
            return new ResultModel<UserInfo>() { 
                Data=res
            };
        }
    }
}
