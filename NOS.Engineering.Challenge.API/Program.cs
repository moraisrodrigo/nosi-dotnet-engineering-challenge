using NOS.Engineering.Challenge.API.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args)
        .ConfigureWebHost()
        .RegisterServices();

WebApplication app = builder.Build();

app.MapControllers();
app.UseSwagger()
    .UseSwaggerUI();

app.Run();