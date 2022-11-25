using System.Net;
using LibrarySystem.Gateway.DTO;
using LibrarySystem.Gateway.Utils;
using LibrarySystem.RatingSystem.DTO;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Quic;

namespace LibrarySystem.Gateway.Services;

public class RatingService
{
    private readonly RetryRequestsBackgroundService _retryRequestsBackgroundService;
    private readonly HttpClient _httpClient;

    public RatingService(string ratingServiceHost, RetryRequestsBackgroundService retryRequestsBackgroundService)
    {
        _retryRequestsBackgroundService = retryRequestsBackgroundService;
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri($"http://{ratingServiceHost}/");
    }

    public async Task<UserRatingResponse?> GetUserRatingAsync(string username)
    {
        using var req = new HttpRequestMessage(HttpMethod.Get, "api/v1/rating");
        req.Headers.Add("X-User-Name", username);
        using var res = await _httpClient.SendAsync(req);
        var response = await res.Content.ReadFromJsonAsync<UserRatingResponse>();
        return response;
    }

    public async Task<UserRatingResponse?> ChangeUserRating(string username, int value)
    {
        using var req = new HttpRequestMessage(HttpMethod.Patch, "api/v1/rating");
        req.Headers.Add("X-User-Name", username);
        req.Content = JsonContent.Create(new ChangeUserRatingRequest()
        {
            Value = value
        });
        try
        {
            using var res = await _httpClient.SendAsync(req);
            if (!res.IsSuccessStatusCode)
            {
                var reqClone = await HttpRequestMessageHelper.CloneHttpRequestMessageAsync(req);
                _retryRequestsBackgroundService.AddRequestToQueue(reqClone);
                return null;
            }

            var response = await res.Content.ReadFromJsonAsync<UserRatingResponse>();

            return response;
        }
        catch (HttpRequestException e)
        {
            var reqClone = await HttpRequestMessageHelper.CloneHttpRequestMessageAsync(req);
            _retryRequestsBackgroundService.AddRequestToQueue(reqClone);
            return null;
        }

        return null;
    }

    public async Task<bool> HealthCheckAsync()
    {
        using var req = new HttpRequestMessage(HttpMethod.Get,
            $"manage/health");
        try
        {
            using var res = await _httpClient.SendAsync(req);
            return res.StatusCode == HttpStatusCode.OK;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
}