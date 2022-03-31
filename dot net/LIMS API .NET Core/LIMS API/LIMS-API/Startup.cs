using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Reflection;
using LIMS_API.Bll;
using LIMS_API.Blls.CommonBlls;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;

namespace LIMS_API
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            LogHelper.Configure();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(name: "cors",
                    builder =>
                    {
                        builder.WithOrigins("https://*.force.com").SetIsOriginAllowedToAllowWildcardSubdomains(); ;
                    });
            });

            services.AddResponseCaching();
            services.AddControllers();
            //����Swagger
            //ע��Swagger������������һ��Swagger �ĵ�
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "�ӿ��ĵ�",
                    Description = "RESTful API"
                });
                // Ϊ Swagger ����xml�ĵ�ע��·��
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            ServiceProvider.Provider = app.ApplicationServices;
            app.UseRouting();
            
            var resFilePath = Path.Combine(Directory.GetCurrentDirectory(), "ResponseFile");
            if (!Directory.Exists(resFilePath)) {
                Directory.CreateDirectory(resFilePath);
            }
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(resFilePath),
                RequestPath = "/File"
            });//��̬�ļ�����

            var siteImgPath = Path.Combine(Directory.GetCurrentDirectory(), "SamplingSiteImage");
            if (!Directory.Exists(siteImgPath)) {
                Directory.CreateDirectory(siteImgPath);
            }


            var signImgPath = Path.Combine(Directory.GetCurrentDirectory(), "SignImage");
            if (!Directory.Exists(signImgPath)) {
                Directory.CreateDirectory(signImgPath);
            }

            var otherFilePath = Path.Combine(Directory.GetCurrentDirectory(), "OtherFile");
            if (!Directory.Exists(otherFilePath))
            {
                Directory.CreateDirectory(otherFilePath);
            }

            var invoiceAssociatedFile = Path.Combine(Directory.GetCurrentDirectory(), "InvoiceAssociatedFile");
            if (!Directory.Exists(invoiceAssociatedFile))
            {
                Directory.CreateDirectory(invoiceAssociatedFile);
            }

            var instrumentFile = Path.Combine(Directory.GetCurrentDirectory(), "InstrumentFile");
            if (!Directory.Exists(instrumentFile))
            {
                Directory.CreateDirectory(instrumentFile);
            }


            AsposeLicenseHelper licenseHelper = new AsposeLicenseHelper();
            licenseHelper.SetAsposeWordsLicense();
            licenseHelper.SetAsposeCellsLicense();
            licenseHelper.SetAsposePdfLicense();

            //app.UseCors("cors");
            app.UseAuthorization();

            app.UseCors(builder => { builder.SetIsOriginAllowed(_ => true).AllowCredentials().AllowAnyHeader(); });



            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });



            //�����м����������Swagger
            app.UseSwagger();
            //�����м����������SwaggerUI��ָ��Swagger JSON�ս��
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Web App V1");
                c.RoutePrefix = string.Empty;//���ø��ڵ����
            });
        }
    }
}
