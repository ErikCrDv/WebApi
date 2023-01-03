using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using WebApi.Filters;
using WebApi.Middlewares;
using WebApi.Services;

namespace WebApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        public void ConfigureService(IServiceCollection services)
        {
            services.AddControllers( options =>
            {
                options.Filters.Add(typeof(MyExceptionFilter));
            }).AddJsonOptions(x => 
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

            services.AddDbContext<ApplicationDbContext>( options => 
                options.UseSqlServer( Configuration.GetConnectionString("defaultConnection")));

            services.AddTransient<IService, ServiceA>();
            //services.AddTransient<ServiceA>();
            services.AddTransient<ServiceTransiet>();
            services.AddScoped<ServiceScoped>();
            services.AddSingleton<ServiceSingleton>();

            services.AddTransient<MyActionFilter>();
            services.AddHostedService<WriteFile>(); 

            services.AddResponseCaching();  
            services.AddAuthentication( JwtBearerDefaults.AuthenticationScheme ).AddJwtBearer();   

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            //app.UseMiddleware<LoggerHttpMiddleware>();
            app.UseLoggerHttp();

            app.Map("/route1", app =>
            {
                app.Run(async context =>
                {
                    await context.Response.WriteAsync("Middleware is intercepting http request");
                });
            });



            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseResponseCaching();   

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
