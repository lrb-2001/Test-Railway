using System;
using FrameIRepository;
using FrameModel;
using StackExchange.Redis;

namespace FrameRepository;

public class RedisRepository: IRedisRepository
{
    private ConnectionMultiplexer redis { get; set; }
    private IDatabase db { get; set; }
    public RedisRepository()
    {
        string connection = GlobalConfig.frameCoreAgileConfig.connectionConfig.RedisCon;
        redis = ConnectionMultiplexer.Connect(connection);
        db = redis.GetDatabase();
    }

    /// <summary>
    /// 增加/修改
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool SetValue(string key, string value, TimeSpan? expiry=null)
    {
        return db.StringSet(key, value, expiry);
    }

    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public string GetValue(string key)
    {
        return db.StringGet(key);
    }

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool DeleteKey(string key)
    {
        return db.KeyDelete(key);
    }
}
