using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Lofi.API.Database;
using Lofi.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Lofi.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment {get;}

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Lofi.API", Version = "v1" }); 
            });

            services.AddDbContext<LofiContext>(options => options.UseNpgsql(Configuration.GetConnectionString("LofiContext")));
            if (Environment.IsDevelopment())
            {
                services.AddDatabaseDeveloperPageExceptionFilter();
            }

            services.AddHttpClient();
            services.AddScoped<MoneroService>(provider =>
            {
                var deamonRpcUri = Configuration.GetValue<string>("MONEROD_RPC_URI", "http://monerod:28081/json_rpc");
                var walletRpcUri = Configuration.GetValue<string>("MONERO_WALLET_RPC_URI", "http://monero-wallet-rpc:28083/json_rpc");
                var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
                return new MoneroService(httpClientFactory, deamonRpcUri, walletRpcUri);
            });
            services.AddScoped<TipService>(provider =>
            {
                var lofiContext = provider.GetRequiredService<LofiContext>();
                var moneroService = provider.GetRequiredService<MoneroService>();
                var deamonRpcUri = Configuration.GetValue<string>("LOFI_WALLET_FILE", "testwallet");
                var walletRpcUri = Configuration.GetValue<string>("LOFI_WALLET_PASSWORD", string.Empty);
                return new TipService(lofiContext, moneroService, deamonRpcUri, walletRpcUri);
            });
            services.AddScoped<AlbumService>();
            services.AddScoped<GenreService>();
            services.AddScoped<TrackService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, LofiContext lofiContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.RoutePrefix = "swagger";
                    c.SwaggerEndpoint("v1/swagger.json", "Lofi.API v1");
                });
            }

            // lofiContext.Database.EnsureCreated();

            // app.UseHttpsRedirection();

            
            app.UseCors(cors => cors.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
