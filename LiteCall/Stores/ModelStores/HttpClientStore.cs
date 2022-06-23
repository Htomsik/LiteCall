using System;
using System.Net.Http;
using LiteCall.ViewModels.Base;

namespace LiteCall.Stores;

internal class HttpClientStore : BaseVmd
{
    public readonly HttpClient CurrentHttpClient;

    public HttpClientStore()
    {
        var clientHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
        };


        CurrentHttpClient = new HttpClient(clientHandler)
        {
            Timeout = TimeSpan.FromSeconds(10),
                DefaultRequestHeaders = { { "ApiKey", ApiKey } }
        };

    }

    private const string ApiKey = "ACbaAS324hnaASD324bzZwq41";



}