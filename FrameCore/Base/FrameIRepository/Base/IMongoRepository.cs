using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FrameIRepository;

public interface IMongoRepository
{
    /// <summary>
    /// 插入数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entity"></param>
    /// <returns></returns>
    public Task Insert<T>(T entity);

    /// <summary>
    /// 删除数据
    /// </summary>
    /// <param name="whereExpression">条件表达式</param>
    /// <returns></returns>
    public Task<long> Delete<T>(Expression<Func<T, bool>> whereExpression);
}