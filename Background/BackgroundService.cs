using iCoin.Data;
using StackExchange.Redis;
using System.Text;

public class BackgroundService : IHostedService
{
    private readonly ILogger _logger;
    private readonly HttpClient _client;
    private Timer? _timer_dump;
    private Timer? _timer;
    private bool first = true;

    public BackgroundService(ILogger<BackgroundService> logger, IHttpClientFactory clientFactory)
    {
        _logger = logger;
        _client = clientFactory.CreateClient(); 
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Background Service is starting.");

        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(300));
        _timer_dump = new Timer(Dump, null, TimeSpan.Zero, TimeSpan.FromSeconds(3600));

        return Task.CompletedTask;
    }
    
    private void DoWork(object? state)
    {    
        _logger.LogInformation("Redis database refresh service started!");
        MakeApiCall().ConfigureAwait(false).GetAwaiter().GetResult();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Background Service is stopping.");

        _timer?.Change(Timeout.Infinite, 0);
        _timer_dump?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    private async Task MakeApiCall()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7046/api/Coin/addCoin");
        request.Content = new StringContent("{\"key\":\"value\"}", Encoding.UTF8, "application/json");

        var response = await _client.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError($"API call failed with status code {response.StatusCode}");
        }
    }

    private async void Dump(object? state)
    {
        if (!first)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:7046/api/Coin/dumpHistory");
            request.Content = new StringContent("{\"key\":\"value\"}", Encoding.UTF8, "application/json");

            var response = await _client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"API call failed with status code {response.StatusCode}");
            }
            first = false;
        }
    }

}



