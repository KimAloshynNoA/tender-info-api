using System.Reflection;
using TenderInfoAPI.Enums;
using TenderInfoAPI.Helpers;
using TenderInfoAPI.Services;
using TenderInfoAPI.Middleware;
using TenderInfoAPI.Repositories;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
});

builder.Services.AddMemoryCache();

builder.Services.AddScoped<ITenderRepository, TenderRepository>();
builder.Services.AddScoped<ITenderService, TenderService>();
builder.Services.AddHttpClient<ITenderRepository, TenderRepository>(client =>
{
    client.BaseAddress = new Uri("https://tenders.guru/api/pl/");
});

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Tender Info API",
        Description = "API for retrieving tender information"
    });

    options.MapType<OrderByField>(() => SwaggerHelper.CreateEnumSchema<OrderByField>());
    options.MapType<OrderDirection>(() => SwaggerHelper.CreateEnumSchema<OrderDirection>());

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

builder.WebHost.UseUrls("http://*:8080");

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tender Info API v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();