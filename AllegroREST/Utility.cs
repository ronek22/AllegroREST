using AllegroREST.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace AllegroREST
{
    public static class Utility
    {
        static JsonSerializer serializer = new JsonSerializer();

        public static T Deserialize<T>(Stream stream)
        {
            var set = new DataContractJsonSerializerSettings
            {
                DateTimeFormat = new DateTimeFormat("yyyy-MM-dd'T'HH:mm:ss.SSSZ"),
            };

            var serializator = new DataContractJsonSerializer(typeof(T), set);
            T obj = (T)serializator.ReadObject(stream);
            stream.Flush();
            stream.Close();
            return obj;
        }
        public static void OpenUrl(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }


        public static string Serialize(Offer offer)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;
                serializer.Serialize(writer, offer);
            }
            return sb.ToString();
        }
        public static void SerializeToken(Token token)
        {
            string path = "secret.json";
            using (StreamWriter file = File.CreateText(path))
            {
                serializer.Serialize(file, token);
            }
        }

        public static Token DeserializeToken
        {
            get
            {
                string filename = "secret.json";
                if (File.Exists(filename))
                {
                    Console.WriteLine("Token istnieje pobieram dane z pliku");
                    using (StreamReader file = File.OpenText(filename))
                    {
                        Token token = (Token)serializer.Deserialize(file, typeof(Token));
                        return token;
                    }
                }
                Console.WriteLine("Token nie istnieje spróbujmy utworzyć nowy.");
                return default;
            }
        }
    }

}
