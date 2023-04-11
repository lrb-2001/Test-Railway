using FrameCommon;
using FrameIRepository;
using FrameModel;
using SqlSugar;
using SqlSugar.Extensions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FrameRepository;

public class BaseRepository: IBaseRepository
{
    private static SqlSugarClient sqlSugarRead;//读
    private static SqlSugarClient sqlSugarWrite;//写

    public BaseRepository()
    {
        //var result = SqlSugarHelper.GetInstance();//读写分离
        //var result = SqlSugarHelper.GetInstance(GlobalConfig.frameCoreAgileConfig.connectionConfig.frameCon);
        //sqlSugarRead = result["read"];
        //sqlSugarWrite = result["write"];
    }

    /// <summary>
    /// 连接数据库
    /// </summary>
    /// <param name="connString">连接字符串</param>
    public void connServer(string connString)
    {
        var result = SqlSugarHelper.GetInstance(connString);
        sqlSugarRead = result["read"];
        sqlSugarWrite = result["write"];
    }

    #region Read

    /// <summary>
    /// 功能描述:查询所有数据
    /// </summary>
    /// <returns>数据列表</returns>
    public async Task<List<T>> QueryAll<T>() where T : class, new()
    {
        List<T> TList = await sqlSugarRead.Queryable<T>().ToListAsync();
        if (TList == null)
            return new List<T>();
        return TList;
    }

    /// <summary>
    /// 功能描述:根据条件表达式查询数据列表
    /// </summary>
    /// <param name="whereExpression">条件表达式</param>
    /// <returns>数据列表</returns>
    public async Task<List<T>> Query<T>(Expression<Func<T, bool>> whereExpression)
    {
        List<T> tEntityList = await sqlSugarRead.Queryable<T>().WhereIF(whereExpression != null, whereExpression).ToListAsync();
        if (tEntityList == null)
            return new List<T>();
        return tEntityList;
    }

    /// <summary>
    /// 功能描述:按ID查询数据
    /// </summary>
    /// <returns>数据列表</returns>
    public async Task<T> Query<T>(int id) where T : class, new()
    {
        T t = await sqlSugarRead.Queryable<T>().ToListAsync() == null ? new T() : await sqlSugarRead.Queryable<T>().In(id).SingleAsync();
        if (t == null)
            return new T();
        return t;
    }

    /// <summary>
    /// 根据sql语句查询
    /// </summary>
    /// <param name="strSql">完整的sql语句</param>
    /// <returns></returns>
    public async Task<List<object>> QueryBySql(string strSql)
    {
        List<object> dataTable = await sqlSugarRead.SqlQueryable<object>(strSql).ToListAsync();
        if (dataTable == null)
            return new List<object>();
        return dataTable;
    }

    /// <summary>
    /// 分页查询[使用版本，其他分页未测试]
    /// </summary>
    /// <param name="whereExpression">条件表达式</param>
    /// <param name="intPageIndex">页码（下标0）</param>
    /// <param name="intPageSize">页大小</param>
    /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
    /// <returns></returns>
    public async Task<PageInfoModel<T>> QueryPage<T>(int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null)
    {
        RefAsync<int> totalCount = 0;
        List<T> tEntityList = await sqlSugarRead.Queryable<T>()
         .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
         .ToPageListAsync(intPageIndex, intPageSize, totalCount);
        if (tEntityList == null)
            tEntityList = new List<T>();
        int pageCount = (Math.Ceiling(totalCount.ObjToDecimal() / intPageSize.ObjToDecimal())).ObjToInt();
        return new PageInfoModel<T>() { DataCount = totalCount, PageCount = pageCount, Page = intPageIndex, PageSize = intPageSize, Data = tEntityList };
    }
    

    /// <summary>
    /// 自定义查询
    /// </summary>
    /// <returns></returns>
    public ISugarQueryable<T> Query<T>()
    {
        return sqlSugarRead.Queryable<T>();
    }

    #endregion

    #region Crate
    /// <summary>
    /// 写入实体数据
    /// </summary>
    /// <param name="entity">实体类</param>
    /// <returns></returns>
    public async Task<int> Add<T>(T entity) where T : class, new()
    {
        var insert = sqlSugarWrite.Insertable<T>(entity);
        return await insert.ExecuteReturnIdentityAsync();
    }
    #endregion

    #region Update
    /// <summary>
    /// 更新实体数据
    /// </summary>
    /// <param name="entity">实体类</param>
    /// <returns></returns>
    public async Task<int> Update<T>(T entity) where T : class, new()
    {
        return await sqlSugarWrite.Updateable(entity).ExecuteCommandAsync();
    }
    #endregion

    #region Delete
    /// <summary>
    /// 根据实体删除一条数据
    /// </summary>
    /// <param name="entity">博文实体类</param>
    /// <returns></returns>
    public async Task<int> Delete<T>(T entity) where T : class, new()
    {
        return await sqlSugarWrite.Deleteable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除指定ID的数据
    /// </summary>
    /// <param name="id">主键ID</param>
    /// <returns></returns>
    public async Task<int> Delete<T>(object id) where T : class, new()
    {
        return await sqlSugarWrite.Deleteable<T>(id).ExecuteCommandAsync();
    }

    /// <summary>
    /// 按表达式删除数据
    /// </summary>
    /// <param name="whereExpression">表达式</param>
    /// <returns></returns>
    public async Task<int> Delete<T>(Expression<Func<T, bool>> whereExpression) where T : class, new()
    {
        return await sqlSugarWrite.Deleteable<T>(whereExpression).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除指定ID集合的数据(批量删除)
    /// </summary>
    /// <param name="ids">主键ID集合</param>
    /// <returns></returns>
    public async Task<int> Delete<T>(object[] ids) where T : class, new()
    {
        return await sqlSugarWrite.Deleteable<T>().In(ids).ExecuteCommandAsync();
    }

    #endregion


}
