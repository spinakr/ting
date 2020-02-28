using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using PocketCqrs;
using PocketCqrs.EventStore;
using Serilog;

namespace Ting
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddHandlers(typeof(Startup).Assembly);
            services.AddSingleton<IMessaging, Messaging>();
            services.AddSingleton<IEventStore, EventStore>();
            var fileStorageLocation = $"{Configuration.GetValue<string>("FILE_BASE_PATH") ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/ting";
            services.AddSingleton<IAppendOnlyStore>(new FileAppendOnlyStore("ting", fileStorageLocation));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                // app.UseHsts();
            }
            app.UseSerilogRequestLogging();
            // app.UsePathBase("/ting");
            // app.UseHttpsRedirection();
            app.UseStaticFiles();
            var fileStorageLocation = $"{Configuration.GetValue<string>("FILE_BASE_PATH") ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/ting";
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(fileStorageLocation, @"images")),
                RequestPath = new PathString("/images")
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
