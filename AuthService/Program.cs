using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Text;
using WordBook.Models;
using Shed.CoreKit.WebApi;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using WordBook.Hubs;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.HttpLogging;
using AuthService.Infrastructure.RabbitMq;
using AuthService.Infrastructure.MapProfile;
using Ocelot.Values;
using AuthService.Infrastructure.Service;
using Hangfire;
using Hangfire.SQLite;
using AuthService.Infrastructure.Jobs;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IRabbitMqService, RabbitMqService>();
builder.Services.AddSingleton<IStatisticJob, StatisticJob>();

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins,
                          policy =>
                          {
                              policy
                              .AllowAnyOrigin()
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                          });
});


builder.Services.AddDbContext<ApplicationDbContext>(opts =>
        opts.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));//HangfireConnection
																						//builder.Services.AddTransient<_userRep>();
																						// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddHangfire(hangfire =>
{
	hangfire.SetDataCompatibilityLevel(CompatibilityLevel.Version_170);
	hangfire.UseSimpleAssemblyNameTypeSerializer();
	hangfire.UseRecommendedSerializerSettings();
	hangfire.UseColouredConsoleLogProvider();
    hangfire.UseSQLiteStorage(
                 builder.Configuration.GetConnectionString("HangfireConnection"),
				 new SQLiteStorageOptions()
                 );
	var server = new BackgroundJobServer(new BackgroundJobServerOptions
	{
		ServerName = "hangfire-test",
	});
});
builder.Services.AddHangfireServer();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        //var key = builder.Configuration.GetSection("Jwt:Audience").Value;
        opt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration.GetSection("Jwt:Issuer").Value,
            ValidAudience = builder.Configuration.GetSection("Jwt:Audience").Value,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration.GetSection("Jwt:Key").Value))
  
        };
        opt.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Headers["access_token"];

                // If the request is for our hub...
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) &&
                    (path.StartsWithSegments("/api/pchealh/hub")))
                {
                    // Read the token out of the query string
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
    logging.RequestHeaders.Add("sec-ch-ua");
    logging.ResponseHeaders.Add("MyResponseHeader");
    logging.MediaTypeOptions.AddText("application/javascript");
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;

});
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddAppService();
builder.Services.AddAutoMapper((typeof(AppMappingProfile)));
builder.Configuration.AddJsonFile("ocelot.json");
builder.Services.AddOcelot(builder.Configuration);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseRouting();
app.UseHttpLogging();
app.UseAuthorization();
app.UseCors(MyAllowSpecificOrigins);
app.UseEndpoints(endpoints => {
    endpoints.MapControllerRoute(
    name: "default",
    pattern: "{controller=UserLogin}/{action=Index}/{id?}");
});
app.UseHangfireDashboard("/dashboard");
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
}
app.MapControllers();
app.UseWebSockets();
app.MapHub<PcHealthHub>("api/pchealh/hub", opt =>
{
    opt.Transports = HttpTransportType.LongPolling;
});
await app.UseOcelot();
app.Run();
