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
        //��������ļ�����
        services.AddSingleton(new AppSettings(Configuration));
        services.AddControllers();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "APIGateway", Version = "v1" });
            // Ϊ Swagger ����xml�ĵ�ע��·��
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });
        //����
        services.AddCorsSetup();
        services.AddOcelot(Configuration).AddConsul().AddPolly();//����
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
            c.DocExpansion(DocExpansion.None); //->�޸Ľ����ʱ�Զ��۵�
        });

        // CORS����
        app.UseCors(AppSettings.app(new string[] { "Startup", "Cors", "PolicyName" }));

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseOcelot().Wait();//����

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
