using System.Collections.Concurrent;
using LibrarySystem.Gateway.Utils;

namespace LibrarySystem.Gateway.Services;

public class RetryRequestsBackgroundService
{
    private readonly ConcurrentQueue<HttpRequestMessage> _requestMessagesQueue = new();
    private readonly HttpClient _httpClient = new();
    private const int TimeoutInSeconds = 5;
    private static object locker = new();

    public RetryRequestsBackgroundService()
    {
    }

    public void StartWorker()
    {
        Task.Factory.StartNew(async () =>
        {
            await Start();
        }, TaskCreationOptions.LongRunning);
        Console.WriteLine("Thread started");
    }

    public void AddRequestToQueue(HttpRequestMessage httpRequestMessage)
    {
        _requestMessagesQueue.Enqueue(httpRequestMessage);
    }

    private async Task Start()
    {
        while (true)
        {
            lock (locker)
            {
                if (!_requestMessagesQueue.TryPeek(out var req))
                {
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    continue;
                }

                try
                {
                    var res = _httpClient.Send(req);
                    if (res.IsSuccessStatusCode)
                    {
                        _requestMessagesQueue.TryDequeue(out _);
                    }
                    else
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(TimeoutInSeconds));
                    }
                }
                catch (Exception e)
                {
                    var reqClone = HttpRequestMessageHelper.CloneHttpRequestMessageAsync(req).GetAwaiter().GetResult();
                    _requestMessagesQueue.TryDequeue(out _);
                    _requestMessagesQueue.Enqueue(reqClone);

                    Thread.Sleep(TimeSpan.FromSeconds(TimeoutInSeconds));
                }
            }
        }
    }
}