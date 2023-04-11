using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;
using Ocelot.Provider.Consul;
using System;
using System.IO;
using System.Reflection;
using Swashbuckle.AspNetCore.SwaggerUI;
using FrameCommon;
using FrameExtensions;

namespace APIGateway;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    { 
        //添加配置文件服务
        services.AddSingleton(new AppSettings(Configuration));
        services.AddControllers();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "APIGateway", Version = "v1" });
            // 为 Swagger 设置xml文档注释路径
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });
        //跨域
        services.AddCorsSetup();
        services.AddOcelot(Configuration).AddConsul().AddPolly();//网关
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseSwagger();
        app.UseSwaggerUI(c => {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "APIGateway v1");
            c.SwaggerEndpoint("/Frame/swagger/frame/swagger.json", "APIFrame API V1");
            c.SwaggerEndpoint("/OpenAI/swagger/openAI/swagger.json", "APIOpenAI API V1");
            c.DocExpansion(DocExpansion.None); //->修改界面打开时自动折叠
        });

        // CORS跨域
        app.UseCors(AppSettings.app(new string[] { "Startup", "Cors", "PolicyName" }));

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseOcelot().Wait();//网关

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
