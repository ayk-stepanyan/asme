using System;
using ASME.WebEnd.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ASME.WebEnd
{
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
            services.AddControllers();
            services.AddSwaggerGen();

            services.AddSingleton<ISecuritiesRepository, SecuritiesRepository>();
            services.AddSingleton<IOrderValidator, OrderValidator>();
            services.AddSingleton<IOrderRouter, OrdersRouter>();
            services.AddSingleton<IIdGenerator, IdGenerator>();
            services.AddSingleton<IOrderBook, OrderBook>();
            services.AddSingleton<ITransactionLedger, TransactionLedger>();
            services.AddTransient<IExecutionEngine, ExecutionEngine>();
            services.AddTransient<Func<IExecutionEngine>>(c => c.GetService<ExecutionEngine>);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ASME.WebEnd v1"));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
