using FrameModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameIService
{
    public interface ITimingTaskService
    {
        /// <summary>
        /// 清理日志
        /// </summary>
        /// <returns></returns>
        public Task<ResultModel<long>> CleanLog();
    }
}
