using ECPLibrary.Persistent;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECPLibrary.Test;

public static class TestServiceExtension
{
    public static void AddDataBase(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<EcpDatabase>(option =>
        {
            option.UseNpgsql(config.GetConnectionString("TEST_LIB"));
            option.EnableDetailedErrors();
        });
        
        services.AddIdentity<IdentityUser<string>, IdentityRole<string>>()
            .AddEntityFrameworkStores<EcpDatabase>()
            .AddDefaultTokenProviders();
    }
}