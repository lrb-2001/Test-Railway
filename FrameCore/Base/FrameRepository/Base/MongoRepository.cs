using FrameCommon;
using FrameIRepository;
using FrameModel;
using MongoDB.Driver;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FrameRepository;

public class MongoRepository: IMongoRepository
{
    private MongoClient client;
    private IMongoDatabase db;
    private string tableName;

    public MongoRepository()
    {
        string connection = GlobalConfig.frameCoreAgileConfig.connectionConfig.MongoDBCon;
        string dataBaseName = AppSettings.app(new string[] { "MongoDB", "DataBaseName" });
        //与Mongodb建立连接
        client = new MongoClient(connection);
        //获得数据库,没有则自动创建
        db = client.GetDatabase(dataBaseName);
        tableName = "LogDB";
    }
    
    /// <summary>
    /// 插入数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entity"></param>
    /// <returns></returns>
    public async Task Insert<T>( T entity)
    {
        IMongoCollection<T> Collection = db.GetCollection<T>(tableName);
        await Collection.InsertOneAsync(entity);
    }

    /// <summary>
    /// 删除数据
    /// </summary>
    /// <param name="whereExpression">条件表达式</param>
    /// <returns></returns>
    public async Task<long> Delete<T>(Expression<Func<T, bool>> whereExpression)
    {
        IMongoCollection<T> Collection = db.GetCollection<T>(tableName);
        var filter = Builders<T>.Filter;
        var project = Builders<T>.Projection;
        DeleteResult result = await Collection.DeleteManyAsync(filter.Where(whereExpression));
        return result.DeletedCount;
    }

}
