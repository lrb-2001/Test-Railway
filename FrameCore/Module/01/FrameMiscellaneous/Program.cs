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

//��������ļ�����
builder.Services.AddSingleton(new AppSettings(builder.Configuration));
builder.Services.AddSingleton(new LogLock(builder.Environment.ContentRootPath));
builder.Services.AddControllers(opt =>// ͳһ����·��ǰ׺
{
    opt.UseCentralRoutePrefix(new RouteAttribute(AppSettings.app(new string[] { "Startup", "RoutePrefixName" })));
});
builder.Services.AddTransientSetup(AppSettings.app(new string[] { "Startup", "Transient" }));//����ע��

builder.Services.AddSwaggerSetup();//Swagger


builder.Services.AddAuthenticationJWTSetup();// ��Ȩ+��֤ (JWT)


builder.Services.AddCorsSetup();//����


builder.Services.AddMemoryCache();//����


builder.Services.Configure<KestrelServerOptions>(x => x.AllowSynchronousIO = true)
    .Configure<IISServerOptions>(x => x.AllowSynchronousIO = true);//ͬ����ȡIO


builder.Services.AddQuartzUI();//�������

builder.Services.AddQuartzClassJobs();//��ӱ��ص����������


builder.Services.AddAgileConfig();//AgileConfig��������


builder.Services.AddHttpClient();//ע��httpclient  ���ڵ����ⲿapi


builder.Services.AddHealthChecks();// Ӧ�ý���״̬���


builder.Services.AddConsul(builder.Configuration);// ����consul����ע����Ϣ


builder.Services.AddAutoMapper(cfg =>//��Ӷ�AutoMapper��֧��
{
    cfg.AddProfile<MapperConfig>();
});


builder.Services.AddMvc(options =>//������
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
    c.DocExpansion(DocExpansion.None); //->�޸Ľ����ʱ�Զ��۵�
    c.RoutePrefix = "frame";
});
app.UseCors(AppSettings.app(new string[] { "Startup", "Cors", "PolicyName" }));// CORS����
app.UseHttpsRedirection();
app.AddHttp().AddCurrentLimiting(); //����м��
app.UseErrorHandling();//������󷵻��м��
//app.UseQuartz();//�������
app.UseAuthentication();//��֤
app.UseAuthorization();//��Ȩ
//app.UseStaticFiles();
app.UseHealthChecks("/health");// ���ý���״̬����м��
app.Use((context, next) =>//����������ʽ      
{
    context.Request.EnableBuffering();
    return next(context);
});
app.MapControllers();
app.Run();
