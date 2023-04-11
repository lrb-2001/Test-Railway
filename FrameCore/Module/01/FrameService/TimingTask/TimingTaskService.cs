using FrameIRepository;
using FrameIService;
using FrameModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameService
{
    public class TimingTaskService: ITimingTaskService
    {
        readonly IBaseRepository repository;
        readonly IMongoRepository mongo;

        public TimingTaskService(IBaseRepository _repository, IMongoRepository _mongo)
        {
            repository = _repository;
            repository.connServer(GlobalConfig.frameCoreAgileConfig.connectionConfig.frameCon);
            mongo = _mongo;
        }

        /// <summary>
        /// 清理日志
        /// </summary>
        /// <returns></returns>
        public async Task<ResultModel<long>> CleanLog()
        {
            //var res= await mongo.Delete<LogModel>(n => n.CreateTime < DateTime.Now.AddDays(-2));
            var count = await repository.Delete<LogModel>(n=>n.CreateTime < DateTime.Now.AddDays(-3));//删除三天前的日志
            return new ResultModel<long>() { 
                Success=true,
                Msg="清理成功！",
                Data= count
            };
        }
    }
}
