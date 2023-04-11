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

//��������ļ�����
builder.Services.AddSingleton(new AppSettings(builder.Configuration));

// Add services to the container.


builder.Services.AddQuartzUI();//�������
builder.Services.AddQuartzClassJobs();//��ӱ��ص����������
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
    //Swagger˵��
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = $"{ApiName} �ӿ��ĵ� - {RuntimeInformation.FrameworkDescription}",
        Description = $"{ApiName} HTTP API " + version,
        Contact = new OpenApiContact { Name = ApiName, Email = email, Url = new Uri(url) },
        License = new OpenApiLicense { Name = ApiName + " �ٷ��ĵ�", Url = new Uri(url) }
    });
    // Jwt Bearer ��֤�������� oauth2
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "JWT��Ȩ(���ݽ�������ͷ�н��д���) ֱ�����¿�������Bearer {token}��ע������֮����һ���ո�\"",
        Name = "Authorization",//jwtĬ�ϵĲ�������
        In = ParameterLocation.Header,//jwtĬ�ϴ��Authorization��Ϣ��λ��(����ͷ��)
        Type = SecuritySchemeType.ApiKey
    });
    //��̨�ڲ�������Ȩ
    c.AddSecurityDefinition("Developer", new OpenApiSecurityScheme
    {
        Description = "Developer��Ȩ",
        Name = FrameConst.Developer,//�����߲�������
        In = ParameterLocation.Header,//���Developer��Ϣ��λ��(����ͷ��)
        Type = SecuritySchemeType.ApiKey
    });
    #region ���һ�������ȫ�ְ�ȫ��Ϣ
    var security = new OpenApiSecurityRequirement()
    {
        //Jwt Bearer ��֤�������� oauth2
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
        //��̨�ڲ�������Ȩ
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
    c.AddSecurityRequirement(security);//���һ�������ȫ�ְ�ȫ��Ϣ����AddSecurityDefinition����ָ���ķ�������Ҫһ�£�������Bearer��//ʵ���Զ�����֤
    #endregion
});
var symmetricKeyAsBase64 = AppSettings.app(new string[] { "Jwt", "Secret" });
var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
var signingKey = new SymmetricSecurityKey(keyByteArray);
var Issuer = AppSettings.app(new string[] { "Jwt", "Issuer" });
var Audience = AppSettings.app(new string[] { "Jwt", "Audience" });
var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
// ������֤����
var tokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = signingKey,
    ValidateIssuer = true,
    ValidIssuer = Issuer,//������
    ValidateAudience = true,
    ValidAudience = Audience,//������
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
// ��Ȩ+��֤ (JWT)
//builder.Services.AddAuthenticationJWTSetup();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}
app.UseSwagger();
app.UseSwaggerUI();

app.UseErrorHandling();//������󷵻��м��
app.UseQuartz();//�������

app.UseHttpsRedirection();
app.UseAuthentication();//��֤
app.UseAuthorization();

app.MapControllers();

app.Run();
