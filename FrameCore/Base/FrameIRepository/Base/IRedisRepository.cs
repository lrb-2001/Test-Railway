using System;

namespace FrameIRepository;

public interface IRedisRepository
{
    /// <summary>
    /// 增加/修改
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool SetValue(string key, string value, TimeSpan? expiry = null);

    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public string GetValue(string key);

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool DeleteKey(string key);
}