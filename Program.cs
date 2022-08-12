using JwtAuth;
using JwtAuth.Common;
using JwtAuth.Services;

namespace JwtAuth
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}

/*
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

{
    var services = builder.Services;
    services.AddCors();
    services.AddControllers();
    services.AddAuthentication();
    // configure strongly typed settings object
    services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

    // configure DI for application services
    services.AddScoped<IUserService, UserService>();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
{
    // global cors policy
    app.UseCors(x => x
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());

    // custom jwt auth middleware
    app.UseMiddleware<JwtMiddleware>();
    app.UseAuthentication();
    app.MapControllers();
}
//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

app.Run();
*/