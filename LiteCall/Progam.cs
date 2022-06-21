using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace LiteCall
{
    public class Progam
    {
        [STAThread]
        public static void Main()
        {
            var app = new App();

            app.InitializeComponent();

            app.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] Args)
        {
            var host_builder = Host.CreateDefaultBuilder(Args)
                .UseContentRoot(Environment.CurrentDirectory)
                .ConfigureServices(App.ConfigureServices)
                .ConfigureAppConfiguration((host, cfg) => cfg
                    .SetBasePath(Environment.CurrentDirectory)
                    .AddJsonFile("apsettings.json", true, reloadOnChange: true)
                );

         
          
            return host_builder;
        }
        
    }
}
