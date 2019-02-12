using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AllegroREST
{
    class Program
    {

        static async Task Main()
        {
            await Run();
        }

        static async Task Run()
        {
            var services = new ServiceCollection();
            services.AddHttpClient<AllegroClient>();

            var serviceProvider = services.BuildServiceProvider();
            var allegro = serviceProvider.GetRequiredService<AllegroClient>();
            await allegro.Authorize();
        }


    }
}
