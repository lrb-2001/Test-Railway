using FrameCommon;
using FrameCommon.Hepler;
using FrameModel;
using log4net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SqlSugar.Extensions;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace FrameExtensions;

/// <summary>
/// 基础扩展类
/// </summary>
public static class BaseSteup
{
    #region 注册
    public static void AddTransientSetup(this IServiceCollection services, string transient)
    {
        ////按程序集注册
        //Assemblys.AddAssembly(services, "FrameService");//批量注册
        //Assemblys.AddAssembly(services, "FrameRepository");//批量注册

        //按程序集注册
        var Transient = transient;
        var transients = Transient.Split(';');
        foreach (var item in transients)
        {
            Assemblys.AddAssembly(services, item);//批量注册
        }


    }
    #endregion

    #region Swagger

    private static readonly ILog log = LogManager.GetLogger(typeof(BaseSteup));
    public static void AddSwaggerSetup(this IServiceCollection services)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        var ApiName = AppSettings.app(new string[] { "Startup", "ApiName" });
        var version = AppSettings.app(new string[] { "Startup", "Version" });
        var controllerXml = AppSettings.app(new string[] { "Startup", "ControllerXml" });
        var modelXml = AppSettings.app(new string[] { "Startup", "ModelXml" });
        var url = AppSettings.app(new string[] { "Startup", "Url" });
        var email = AppSettings.app(new string[] { "Startup", "Email" });
        services.AddSwaggerGen(c =>
        {
            //Swagger说明
            c.SwaggerDoc(version, new OpenApiInfo
            {
                Version = version,
                Title = $"{ApiName} 接口文档 - {RuntimeInformation.FrameworkDescription}",
                Description = $"{ApiName} HTTP API " + version,
                Contact = new OpenApiContact { Name = ApiName, Email = email, Url = new Uri(url) },
                License = new OpenApiLicense { Name = ApiName + " 官方文档", Url = new Uri(url) }
            });
            c.OrderActionsBy(o => o.RelativePath);

            try
            
            {
                // 为 Swagger JSON and UI设置xml文档注释路径
                var basePath = Path.Combine(AppContext.BaseDirectory, controllerXml);//这个就是刚刚配置的xml文件名
                c.IncludeXmlComments(basePath, true);//默认的第二个参数是false，这个是controller的注释，记得修改
                var xmlPath = Path.Combine(AppContext.BaseDirectory, modelXml);//模型注释
                c.IncludeXmlComments(xmlPath);
            }
            catch (Exception ex)
            {
                log.Error($"{controllerXml}和{modelXml} 丢失，请检查并拷贝。\n" + ex.Message);
            }

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

            #region 只有JWT默认授权才开启这写
            // 开启加权小锁
            c.OperationFilter<AddResponseHeadersFilter>();
            c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();

            // 在header中添加token，传递到后台
            //c.OperationFilter<SecurityRequirementsOperationFilter>();
            #endregion


        });

    }
    #endregion

    #region JWT认证

    /// <summary>
    /// JWT认证
    /// </summary>
    /// <param name="services"></param>
    public static void AddAuthenticationJWTSetup(this IServiceCollection services)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));


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

        #region 默认JWT验证
        //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        //.AddJwtBearer(options =>
        //{
        //    options.TokenValidationParameters = tokenValidationParameters;

        //    options.Events = new JwtBearerEvents
        //    {
        //        //权限验证失败后执行
        //        OnChallenge = context =>
        //        {
        //            //终止默认的返回结果(必须有)
        //            context.HandleResponse();
        //            var result = JsonConvert.SerializeObject(new { Code = "401", Message = "验证失败" });
        //            context.Response.ContentType = "application/json";
        //            //验证失败返回401
        //            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        //            context.Response.WriteAsync(result);
        //            return Task.FromResult(0);
        //        }
        //    };
        //});
        #endregion

        #region 自定义加默认JWT验证
        services.AddAuthentication(option =>
        {
            option.DefaultAuthenticateScheme = FrameConst.Developer;
            option.DefaultChallengeScheme = FrameConst.Developer;
        })
       .AddAuthentication(options => { })
       .AddJwtBearer(options =>
       {
           options.TokenValidationParameters = tokenValidationParameters;

           #region JWTBearer 验证失败返回
           //options.Events = new JwtBearerEvents
           //{
           //     //权限验证失败后执行
           //     OnChallenge = context =>
           //    {
           //         //终止默认的返回结果(必须有)
           //         context.HandleResponse();
           //        var result = JsonConvert.SerializeObject((new ApiResponse(StatusCode.CODE401)).MessageModel);
           //        //JsonConvert.SerializeObject(new { Code = "401", Message = "验证失败" });
           //        context.Response.ContentType = "application/json";
           //         //验证失败返回401
           //         context.Response.StatusCode = StatusCodes.Status401Unauthorized;
           //        context.Response.WriteAsync(result);
           //        return Task.FromResult(0);
           //    }
           //};
           #endregion

       });

        #endregion




        ////基于自定义策略授权
        //services.AddAuthorization(options =>
        //{
        //    options.AddPolicy("Admin", policy => policy.RequireRole("Admin").Build());
        //    options.AddPolicy("customizePermisson",
        //    policy => policy
        //    .Requirements
        //    .Add(new PermissionRequirement("admin")));
        //});

    }

    public static AuthenticationBuilder AddAuthentication(this AuthenticationBuilder authenticationBuilder, Action<FrameConst> options)
    {
        return authenticationBuilder.AddScheme<FrameConst, AddAuthenticationHandler>(FrameConst.Developer, options);
    }

    #endregion

    #region 中间件
    /// <summary>
    /// 添加限流中间件
    /// </summary>
    /// <param name="app"></param>
    public static IApplicationBuilder AddCurrentLimiting(this IApplicationBuilder app)
    {
        //限流
        app.UseMiddleware<CurrentLimiting>();
        return app;
    }
    /// <summary>
    /// 添加日志中间件
    /// </summary>
    /// <param name="app"></param>
    public static IApplicationBuilder AddHttp(this IApplicationBuilder app)
    {
        //日志
        app.UseMiddleware<HttpMiddleware>();
        return app;
    }

    #endregion

    #region 跨域
    /// <summary>
    /// 跨域
    /// </summary>
    /// <param name="services"></param>
    public static void AddCorsSetup(this IServiceCollection services)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        services.AddCors(c => {
            if (!AppSettings.app(new string[] { "Startup", "Cors", "EnableAllIPs" }).ObjToBool())
            {
                c.AddPolicy(AppSettings.app(new string[] { "Startup", "Cors", "PolicyName" }),
                    policy => {
                        policy.WithOrigins(AppSettings.app(new string[] { "Startup", "Cors", "IPs" }).Split(','))
                .AllowAnyHeader()//Ensures that the policy allows any header.
                .AllowAnyMethod();
                    });
            }
            else
            {
                //允许任意跨域请求
                c.AddPolicy(AppSettings.app(new string[] { "Startup", "Cors", "PolicyName" }),
                    policy =>
                    {
                        policy
                        .SetIsOriginAllowed((host) => true)
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                        //.AllowCredentials();
                    });
            }
        });

    }
    #endregion

    #region 路由
    /// <summary>
    /// 扩展方法
    /// </summary>
    /// <param name="opts"></param>
    /// <param name="routeAttribute"></param>
    public static void UseCentralRoutePrefix(this MvcOptions opts, IRouteTemplateProvider routeAttribute)
    {
        // 添加我们自定义 实现IApplicationModelConvention的RouteConvention
        opts.Conventions.Insert(0, new RouteConvention(routeAttribute));
    }
    #endregion

}
