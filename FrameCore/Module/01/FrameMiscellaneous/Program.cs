using FrameCommon;
using FrameExtensions;
using FrameMiscellaneous.Common;
using FrameModel;
using GZY.Quartz.MUI.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Swashbuckle.AspNetCore.SwaggerUI;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseAgileConfig(e => Console.WriteLine($"configs {e.Action}"));
builder.WebHost.UseUrls("https://*:8680");
GlobalConfig.frameCoreAgileConfig = GlobalConfigHelper.InitConfig(builder.Configuration);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//添加配置文件服务
builder.Services.AddSingleton(new AppSettings(builder.Configuration));
builder.Services.AddSingleton(new LogLock(builder.Environment.ContentRootPath));
builder.Services.AddControllers(opt =>// 统一设置路由前缀
{
    opt.UseCentralRoutePrefix(new RouteAttribute(AppSettings.app(new string[] { "Startup", "RoutePrefixName" })));
});
builder.Services.AddTransientSetup(AppSettings.app(new string[] { "Startup", "Transient" }));//服务注册

builder.Services.AddSwaggerSetup();//Swagger


builder.Services.AddAuthenticationJWTSetup();// 授权+认证 (JWT)


builder.Services.AddCorsSetup();//跨域


builder.Services.AddMemoryCache();//限流


builder.Services.Configure<KestrelServerOptions>(x => x.AllowSynchronousIO = true)
    .Configure<IISServerOptions>(x => x.AllowSynchronousIO = true);//同步读取IO


builder.Services.AddQuartzUI();//任务调度

builder.Services.AddQuartzClassJobs();//添加本地调度任务访问


builder.Services.AddAgileConfig();//AgileConfig配置中心


builder.Services.AddHttpClient();//注入httpclient  用于调用外部api


builder.Services.AddHealthChecks();// 应用健康状态检查


builder.Services.AddConsul(builder.Configuration);// 配置consul服务注册信息


builder.Services.AddAutoMapper(cfg =>//添加对AutoMapper的支持
{
    cfg.AddProfile<MapperConfig>();
});


builder.Services.AddMvc(options =>//过滤器
{
    options.Filters.Add<BaseFilter>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
//Swagger
var ApiName = AppSettings.app(new string[] { "Startup", "ApiName" });
var version = AppSettings.app(new string[] { "Startup", "Version" });
app.UseSwagger();
app.UseSwaggerUI(c => {
    c.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"{ApiName} {version}");
    c.DocExpansion(DocExpansion.None); //->修改界面打开时自动折叠
    c.RoutePrefix = "frame";
});
app.UseCors(AppSettings.app(new string[] { "Startup", "Cors", "PolicyName" }));// CORS跨域
app.UseHttpsRedirection();
app.AddHttp().AddCurrentLimiting(); //添加中间件
app.UseErrorHandling();//请求错误返回中间件
//app.UseQuartz();//任务调度
app.UseAuthentication();//认证
app.UseAuthorization();//授权
//app.UseStaticFiles();
app.UseHealthChecks("/health");// 启用健康状态检查中间件
app.Use((context, next) =>//启动倒带方式      
{
    context.Request.EnableBuffering();
    return next(context);
});
app.MapControllers();
app.Run();
