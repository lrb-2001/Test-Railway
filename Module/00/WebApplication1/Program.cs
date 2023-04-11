using FrameCommon;
using FrameExtensions;
using Microsoft.OpenApi.Models;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System;
using FrameModel;
using Swashbuckle.AspNetCore.SwaggerUI;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using GZY.Quartz.MUI.Extensions;

var builder = WebApplication.CreateBuilder(args);

//添加配置文件服务
builder.Services.AddSingleton(new AppSettings(builder.Configuration));

// Add services to the container.


builder.Services.AddQuartzUI();//任务调度
builder.Services.AddQuartzClassJobs();//添加本地调度任务访问
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
var ApiName = AppSettings.app(new string[] { "Startup", "ApiName" });
var version = AppSettings.app(new string[] { "Startup", "Version" });
var controllerXml = AppSettings.app(new string[] { "Startup", "ControllerXml" });
var modelXml = AppSettings.app(new string[] { "Startup", "ModelXml" });
var url = AppSettings.app(new string[] { "Startup", "Url" });
var email = AppSettings.app(new string[] { "Startup", "Email" });
builder.Services.AddSwaggerGen(c => {
    //Swagger说明
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = $"{ApiName} 接口文档 - {RuntimeInformation.FrameworkDescription}",
        Description = $"{ApiName} HTTP API " + version,
        Contact = new OpenApiContact { Name = ApiName, Email = email, Url = new Uri(url) },
        License = new OpenApiLicense { Name = ApiName + " 官方文档", Url = new Uri(url) }
    });
    // Jwt Bearer 认证，必须是 oauth2
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "JWT授权(数据将在请求头中进行传输) 直接在下框中输入Bearer {token}（注意两者之间是一个空格）\"",
        Name = "Authorization",//jwt默认的参数名称
        In = ParameterLocation.Header,//jwt默认存放Authorization信息的位置(请求头中)
        Type = SecuritySchemeType.ApiKey
    });
    //后台内部调用授权
    c.AddSecurityDefinition("Developer", new OpenApiSecurityScheme
    {
        Description = "Developer授权",
        Name = FrameConst.Developer,//开发者参数名称
        In = ParameterLocation.Header,//存放Developer信息的位置(请求头中)
        Type = SecuritySchemeType.ApiKey
    });
    #region 添加一个必须的全局安全信息
    var security = new OpenApiSecurityRequirement()
    {
        //Jwt Bearer 认证，必须是 oauth2
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference()
                {
                    Id = FrameConst.HttpParaKey_Token,//"X-JWT-TOKEN",
                    Type = ReferenceType.SecurityScheme
                }
            }, Array.Empty<string>()
        },
        //后台内部调用授权
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference()
                {
                    Id = FrameConst.Developer,//"X-USER-LOGINNAME",
                    Type = ReferenceType.SecurityScheme
                }
            }, Array.Empty<string>()
        }
    };
    c.AddSecurityRequirement(security);//添加一个必须的全局安全信息，和AddSecurityDefinition方法指定的方案名称要一致，这里是Bearer。//实现自定义验证
    #endregion
});
var symmetricKeyAsBase64 = AppSettings.app(new string[] { "Jwt", "Secret" });
var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
var signingKey = new SymmetricSecurityKey(keyByteArray);
var Issuer = AppSettings.app(new string[] { "Jwt", "Issuer" });
var Audience = AppSettings.app(new string[] { "Jwt", "Audience" });
var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
// 令牌验证参数
var tokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = signingKey,
    ValidateIssuer = true,
    ValidIssuer = Issuer,//发行人
    ValidateAudience = true,
    ValidAudience = Audience,//订阅人
    ValidateLifetime = true,
    ClockSkew = TimeSpan.FromSeconds(30),
    RequireExpirationTime = true,
};

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = FrameConst.Developer;
    option.DefaultChallengeScheme = FrameConst.Developer;
})
.AddAuthentication(options => { })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = tokenValidationParameters;
    });
//builder.Services.AddSwaggerSetup();//Swagger
// 授权+认证 (JWT)
//builder.Services.AddAuthenticationJWTSetup();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}
app.UseSwagger();
app.UseSwaggerUI();

app.UseErrorHandling();//请求错误返回中间件
app.UseQuartz();//任务调度

app.UseHttpsRedirection();
app.UseAuthentication();//认证
app.UseAuthorization();

app.MapControllers();

app.Run();
