using DnDCharacterManager.Application;
using DnDCharacterManager.Web;
using Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext()
);

try
{
    builder.Services
        .AddInfrastructure(builder.Configuration)
        .AddApplication(builder.Configuration)
        .AddWeb();

    var app = builder.Build();

    await app.ConfigureWebApplicationAsync();

    app.Run();
}
catch (Exception e)
{
    Log.Fatal(e, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
