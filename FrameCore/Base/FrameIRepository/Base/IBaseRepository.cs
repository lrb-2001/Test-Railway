using FrameModel;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FrameIRepository;

public interface IBaseRepository
{
    /// <summary>
    /// 连接数据库
    /// </summary>
    /// <param name="connString">连接字符串</param>
    public void connServer(string connString);
    #region Read

    /// <summary>
    /// 功能描述:查询所有数据
    /// </summary>
    /// <returns>数据列表</returns>
    public Task<List<T>> QueryAll<T>() where T : class, new();

    /// <summary>
    /// 功能描述:根据条件表达式查询数据列表
    /// </summary>
    /// <param name="whereExpression">条件表达式</param>
    /// <returns>数据列表</returns>
    public Task<List<T>> Query<T>(Expression<Func<T, bool>> whereExpression);

    /// <summary>
    /// 功能描述:按ID查询数据
    /// </summary>
    /// <returns>数据列表</returns>
    public Task<T> Query<T>(int id) where T : class, new();

    /// <summary>
    /// 根据sql语句查询
    /// </summary>
    /// <param name="strSql">完整的sql语句</param>
    /// <returns></returns>
    public Task<List<object>> QueryBySql(string strSql);

    /// <summary>
    /// 分页查询[使用版本，其他分页未测试]
    /// </summary>
    /// <param name="whereExpression">条件表达式</param>
    /// <param name="intPageIndex">页码（下标0）</param>
    /// <param name="intPageSize">页大小</param>
    /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
    /// <returns></returns>
    public Task<PageInfoModel<T>> QueryPage<T>(int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null);

    /// <summary>
    /// 自定义查询
    /// </summary>
    /// <returns></returns>
    public ISugarQueryable<T> Query<T>();

    #endregion

    #region Crate
    /// <summary>
    /// 写入实体数据
    /// </summary>
    /// <param name="entity">实体类</param>
    /// <returns></returns>
    public Task<int> Add<T>(T entity) where T : class, new();
    #endregion

    #region Update
    /// <summary>
    /// 更新实体数据
    /// </summary>
    /// <param name="entity">实体类</param>
    /// <returns></returns>
    public Task<int> Update<T>(T entity) where T : class, new();
    #endregion

    #region Delete
    /// <summary>
    /// 根据实体删除一条数据
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public Task<int> Delete<T>(T entity) where T : class, new();

    /// <summary>
    /// 删除指定ID的数据
    /// </summary>
    /// <param name="id">主键ID</param>
    /// <returns></returns>
    public Task<int> Delete<T>(object id) where T : class, new();

    /// <summary>
    /// 按表达式删除数据
    /// </summary>
    /// <param name="whereExpression">表达式</param>
    /// <returns></returns>
    public Task<int> Delete<T>(Expression<Func<T, bool>> whereExpression) where T : class, new();

    /// <summary>
    /// 删除指定ID集合的数据(批量删除)
    /// </summary>
    /// <param name="ids">主键ID集合</param>
    /// <returns></returns>
    public Task<int> Delete<T>(object[] ids) where T : class, new();
    #endregion

}
