using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AesirCoreApi
{

    public static class AesirCoreApi
    {
        public static void Init()
        {
            CreateHostBuilder(new string[] { }).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                    .UseUrls("https://0.0.0.0:5363", "http://0.0.0.0:5362");
                });
    }
}
