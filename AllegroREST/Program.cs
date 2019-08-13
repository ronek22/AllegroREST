﻿using Microsoft.Extensions.DependencyInjection;
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

        static void DisplayMenu()
        {
            Console.WriteLine("Allegro REST Client");
            Console.WriteLine();
            Console.WriteLine("1. Get my offers");
            Console.WriteLine("2. Get listing by phrase");
            Console.WriteLine("3. Do something else");
            Console.WriteLine("[ESC] To Exit");
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

            ConsoleKeyInfo cki;
            do
            {
                DisplayMenu();
                cki = Console.ReadKey(false); // show the key as you read it
                Console.Clear();
                switch (cki.KeyChar.ToString())
                {
                    case "1":
                        await allegro.GetMyOffers();
                        break;
                    case "2":
                        Console.WriteLine("Type phrase you want to search: ");
                        await allegro.GetListingByPhrase(Console.ReadLine());
                        break;
                    case "3":
                        Console.WriteLine("Type number of auction: ");
                        Console.WriteLine(await allegro.GetOfferDetails(Console.ReadLine()));
                        break;

                }
            } while (cki.Key != ConsoleKey.Escape);
        }

    }
}
