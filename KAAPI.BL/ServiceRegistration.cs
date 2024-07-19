using KAAPI.BL.CustomService;
using KAAPI.BL.ICustomService;
using KAAPI.DataObject.Context;
using KAAPI.DL.DataService;
using KAAPI.DL.IDataService;
using Microsoft.Extensions.DependencyInjection;

namespace KAAPI.BL
{
    public static class ServiceRegistration
    {
        public static void AddInfracture(this IServiceCollection _service)
        {
            _service.AddScoped<IApplicationDBContext>(provider => provider.GetRequiredService<ApplicationDBContext>());

            _service.AddScoped<IUnitofWork, UnitofWork>();
            _service.AddScoped<IunitofService, UnitofService>();

            _service.AddTransient<IAuthenicationDL, AuthenicationDL>();
            _service.AddTransient<IAuthenticationBL, AuthenticationBL>();
        }
    }
}