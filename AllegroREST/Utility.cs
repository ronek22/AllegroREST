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

    }

}
