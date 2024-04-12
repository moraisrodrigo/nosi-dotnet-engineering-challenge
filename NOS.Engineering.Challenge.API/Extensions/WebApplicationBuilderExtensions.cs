using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using NOS.Engineering.Challenge.Database;
using NOS.Engineering.Challenge.Managers;
using NOS.Engineering.Challenge.Models;
using NOS.Engineering.Challenge.Interfaces;
using NOS.Engineering.Challenge.Cache;

namespace NOS.Engineering.Challenge.API.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder RegisterServices(this WebApplicationBuilder webApplicationBuilder)
    {
        IServiceCollection serviceCollection = webApplicationBuilder.Services;

        serviceCollection.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.SerializerOptions.PropertyNamingPolicy = null;
        });

        string? connectionString = webApplicationBuilder.Configuration.GetConnectionString("DefaultConnection");

        serviceCollection.AddDbContext<AppDbContext>(option => option.UseMySQL(connectionString,
            src => src.MigrationsAssembly("NOS.Engineering.Challenge.API")));

        serviceCollection.AddControllers();
        serviceCollection
            .AddEndpointsApiExplorer();

        serviceCollection.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Nos Challenge Api", Version = "v1" });
        });

        serviceCollection
            .RegisterSlowDatabase()
            .RegisterContentsManager();
        return webApplicationBuilder;
    }

    private static IServiceCollection RegisterSlowDatabase(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddSingleton<ICacheService<Content>, CacheService<Content>>();
        services.AddSingleton<IDatabase<Content, ContentDto>, SlowDatabase<Content, ContentDto>>();
        services.AddSingleton<IMapper<Content, ContentDto>, ContentMapper>();
        services.AddSingleton<IMockData<Content>, MockData>();
        services.AddDbContext<AppDbContext>();

        return services;
    }

    private static IServiceCollection RegisterContentsManager(this IServiceCollection services)
    {
        services.AddSingleton<IContentsManager, ContentsManager>();

        return services;
    }


    public static WebApplicationBuilder ConfigureWebHost(this WebApplicationBuilder webApplicationBuilder)
    {

        webApplicationBuilder.Logging.ClearProviders();
        webApplicationBuilder.Logging.AddConsole();

        return webApplicationBuilder;
    }
}