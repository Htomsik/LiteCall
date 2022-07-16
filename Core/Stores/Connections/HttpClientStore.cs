using Core.VMD.Base;
using Microsoft.Extensions.Configuration;

namespace Core.Stores.Connections;

public class HttpClientStore : BaseVmd
{
    private const string ApiKey = "ACbaAS324hnaASD324bzZwq41";
    public readonly HttpClient CurrentHttpClient;

    public HttpClientStore(IConfiguration configuration)
    {
        var clientHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
        };


        CurrentHttpClient = new HttpClient(clientHandler)
        {
            Timeout = TimeSpan.FromSeconds(10),
            DefaultRequestHeaders = { { "ApiKey", configuration["ApiKey"] ?? configuration["AppSettings:DefaultApiKey"] } }
        };
    }
}
