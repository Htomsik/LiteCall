using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace LiteCall;

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
        var hostBuilder = Host.CreateDefaultBuilder(Args)
            .UseContentRoot(Environment.CurrentDirectory)
            .ConfigureServices(App.ConfigureServices)
            .ConfigureAppConfiguration((host, cfg) => cfg
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("apsettings.json", true, true)
            );


        return hostBuilder;
    }
}