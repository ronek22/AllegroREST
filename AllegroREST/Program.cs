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
            // Konfigurowanie zaleznosci
            var services = new ServiceCollection();
            services.AddHttpClient<AllegroClient>();
            var serviceProvider = services.BuildServiceProvider();

            // Praca z klientem
            var allegro = serviceProvider.GetRequiredService<AllegroClient>();
            await allegro.Authorize();

            await allegro.GetMyOffers();
            await allegro.GetListingByPhrase("motorola");
            // Gdy skonczy sie waznosc AccessTokena (12h), mozemy otrzymac nowe tokeny za pomoca tej metody
            // await allegro.RefreshAccessToken();
        }

    }
}
