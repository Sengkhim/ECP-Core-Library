// using ECPLibrary.Core.UnitOfWork;
// using ECPLibrary.Extensions;
// using ECPLibrary.Persistent;
// using ECPLibrary.Services;
// using ECPLibrary.Test;
using Microsoft.AspNetCore.Builder;

namespace ECPLibrary.Mock;

public abstract class WebApplicationMockUp
{
    public static void Builder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // builder.Services.AddControllers();
        // builder.Services.AddEndpointsApiExplorer();
        // builder.Services.AddCoreEcpLibrary();
        // builder.Services.AddDataBase(builder.Configuration);
        // builder.Services.AddTransient<IEcpDatabase, EcpDatabase>();
        // builder.Services.AddScoped<IUnitOfWork<EcpDatabase>, UnitOfWork<EcpDatabase>>();

        var app = builder.Build();

        app.MapControllers();
        app.Run();
    }
}