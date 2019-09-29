using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Books.Api.Contexts;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Books.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //throttle the thread pool(set available threads to amount of processors)
            ThreadPool.SetMaxThreads(Environment.ProcessorCount, Environment.ProcessorCount);
            var host = CreateWebHostBuilder(args).Build(); 
            //migrate the database on running the app
            using (var scope = host.Services.CreateScope())
            {
              try
              {
                  var context = scope.ServiceProvider.GetService<BooksContext>();
                  context.Database.Migrate();
              }
              catch (Exception e)
              {
                  var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                  logger.LogError(e , "An error occured while migrating the database");
              }
            } 
            //run the app
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
