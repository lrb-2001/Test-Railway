using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FrameCommon.Hepler;

public class Assemblys
{
    /// <summary> 
    /// 自动注册服务——获取程序集中的实现类对应的多个接口
    /// </summary>
    /// <param name="services">服务集合</param> 
    /// <param name="assemblyName">程序集名称</param>
    public static void AddAssembly(IServiceCollection services, string assemblyName)
    {
        if (!String.IsNullOrEmpty(assemblyName))
        {
            Assembly assembly = Assembly.Load(assemblyName);
            //MemberTypes.TypeInfo 指定成员是类型
            List<Type> ts = assembly.GetTypes().Where(u => u.MemberType == MemberTypes.TypeInfo).ToList();

            foreach (var item in ts)
            {
                var interfaceType = item.GetInterfaces();
                if (interfaceType.Length == 1)
                {
                    services.AddTransient(interfaceType[0], item);
                }
                if (interfaceType.Length > 1)
                {
                    services.AddTransient(interfaceType[1], item);
                }
            }
        }
    }
}