using Grpc.Net.Client;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.Hosting;
using Ozon.Route256.Five.OrderService.Core.Extensions;
using Ozon.Route256.Five.OrderService.Host;

namespace Ozon.Route256.Five.OrderService.IntegrationTests;

public sealed class OrderServiceAppFactory : WebApplicationFactory<Startup>
{
    public Orders.Grpc.Orders.OrdersClient OrdersClient { get; }

    public RestClient RestClient { get; }

    public OrderServiceAppFactory()
    {
        IHostBuilder hostBuilder = CreateHostBuilder();
        hostBuilder.ConfigureWebHost(builder => builder.UseTestServer());
        CreateHost(hostBuilder);

        HttpClient client = CreateDefaultClient(new ResponseVersionHandler());

        GrpcChannel grpcChannel = GrpcChannel.ForAddress(client.BaseAddress!, new GrpcChannelOptions
        {
            HttpClient = client
        });

        OrdersClient = new Orders.Grpc.Orders.OrdersClient(grpcChannel);
        RestClient = new RestClient(null, client);
    }

    protected override IHostBuilder CreateHostBuilder()
    {
        MemoryConfigurationSource memoryConfigurationSource =
            new MemoryConfigurationSource
            {
                InitialData = new[]
                {
                    new KeyValuePair<string, string?>(Constants.SERVICE_DISCOVERY_ADDRESS_KEY, "http://host.docker.internal:5084"),
                    new KeyValuePair<string, string?>(Constants.LOGISTICS_SIMULATOR_ADDRESS_KEY, "http://host.docker.internal:5085"),
                    new KeyValuePair<string, string?>(Constants.CUSTOMER_SERVICE_ADDRESS_KEY, "http://host.docker.internal:5081"),
                }
            };

        IHostBuilder builder = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(configurationBuilder =>
            {
                configurationBuilder.Add(memoryConfigurationSource);
                configurationBuilder.AddJsonFile("testsettings.json", false);
            })
            .ConfigureWebHostDefaults(webHostBuilder => webHostBuilder.UseStartup<Startup>());

        return builder;
    }

    private class ResponseVersionHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
            response.Version = request.Version;
            return response;
        }
    }
}