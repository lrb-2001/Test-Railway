using Consul;
using Consul.AspNetCore;
using FrameModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace FrameExtensions;

/// <summary>
/// Consul扩展类
/// </summary>
public static class ConsulRegisterEx
{
    /// <summary>
    /// Configuration 扩展方法ip和端口走的命令窗体控制台， Consul 配置走的是appsettings.json 文件
    /// </summary>
    /// <param name="configuration"></param>
    public static IServiceCollection AddConsul(this IServiceCollection services, IConfiguration configuration)
    {
        // 配置consul服务注册信息
        var option = configuration.GetSection("Consul").Get<ConsulOption>();
        // 通过consul提供的注入方式注册consulClient
        services.AddConsul(options => options.Address = new Uri($"http://{option.ConsulIP}:{option.ConsulPort}"));

        // 通过consul提供的注入方式进行服务注册
        var httpCheck = new AgentServiceCheck()
        {
            DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),//服务启动多久后注册
            Interval = TimeSpan.FromSeconds(10),//健康检查时间间隔，或者称为心跳间隔
            HTTP = $"http://{option.IP}:{option.Port}/health",//健康检查地址
            Timeout = TimeSpan.FromSeconds(5)
        };

        // Register service with consul
        var registration = new AgentServiceRegistration()
        {
            Checks = new[] { httpCheck },
            ID = Guid.NewGuid().ToString(),
            Name = option.ServiceName,
            Address = option.IP,
            Port = option.Port,
            Meta = new Dictionary<string, string>() { { "Weight", option.Weight.HasValue ? option.Weight.Value.ToString() : "1" } },
            Tags = new[] { $"urlprefix-/{option.ServiceName}" }//添加 urlprefix-/servicename 格式的 tag 标签，以便 Fabio 识别
        };
        services.AddConsulServiceRegistration(options =>
        {
            options.Checks = new[] { httpCheck };
            options.ID = Guid.NewGuid().ToString();
            options.Name = option.ServiceName;
            options.Address = option.IP;
            options.Port = option.Port;
            options.Meta = new Dictionary<string, string>() { { "Weight", option.Weight.HasValue ? option.Weight.Value.ToString() : "1" } };
            options.Tags = new[] { $"urlprefix-/{option.ServiceName}" }; //添加 urlprefix -/ servicename 格式的 tag 标签，以便 Fabio 识别
        });

        //services.AddSingleton<IConsulServerManager, DefaultConsulServerManager>();
        return services;
    }

}