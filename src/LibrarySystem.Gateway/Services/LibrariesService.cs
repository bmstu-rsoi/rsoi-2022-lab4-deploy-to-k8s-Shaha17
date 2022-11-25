using System.Net;
using LibrarySystem.Gateway.DTO;
using LibrarySystem.Gateway.Models;
using LibrarySystem.Gateway.Utils;

namespace LibrarySystem.Gateway.Services;

public class LibrariesService
{
    private readonly RetryRequestsBackgroundService _retryRequestsBackgroundService;
    private readonly HttpClient _httpClient;

    public LibrariesService(string libraryServiceHost, RetryRequestsBackgroundService retryRequestsBackgroundService)
    {
        _retryRequestsBackgroundService = retryRequestsBackgroundService;
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri($"http://{libraryServiceHost}/");
    }

    public async Task<PaginationResponse<IEnumerable<Library>>?> GetLibrariesByCityAsync(string city, int? page,
        int? size)
    {
        using var req = new HttpRequestMessage(HttpMethod.Get, $"api/v1/libraries?city={city}&page={page}&size={size}");
        using var res = await _httpClient.SendAsync(req);
        var response = await res.Content.ReadFromJsonAsync<PaginationResponse<IEnumerable<Library>>>();
        return response;
    }

    public async Task<LibraryBook?> GetLibraryBookByLibUidAndBookUidAsync(Guid libUid, Guid bookUid)
    {
        using var req = new HttpRequestMessage(HttpMethod.Get, $"api/v1/libraries/{libUid}/books/{bookUid}");
        using var res = await _httpClient.SendAsync(req);
        var response = await res.Content.ReadFromJsonAsync<LibraryBook>();
        return response;
    }

    public async Task<PaginationResponse<IEnumerable<Book>>?> GetBooksByLibraryAsync(Guid libraryUid, int? page,
        int? size, bool? showAll = false)
    {
        using var req = new HttpRequestMessage(HttpMethod.Get,
            $"api/v1/libraries/{libraryUid}/books?page={page}&size={size}&showAll={showAll}");
        using var res = await _httpClient.SendAsync(req);
        var response = await res.Content.ReadFromJsonAsync<PaginationResponse<IEnumerable<Book>>>();
        return response;
    }

    public async Task<LibraryBook> IncrementAvailableCountByLibUidAndBookUid(Guid libUid, Guid bookUid)
    {
        using var req = new HttpRequestMessage(HttpMethod.Patch,
            $"api/v1/libraries/{libUid}/books/{bookUid}/increment");
        req.Content = JsonContent.Create(new {value = 1});
        var msgCopy = new HttpRequestMessage();
        var stream = await req.Content.ReadAsStreamAsync();
        try
        {
            using var res = await _httpClient.SendAsync(req);
            if (!res.IsSuccessStatusCode)
            {
                var reqClone = await HttpRequestMessageHelper.CloneHttpRequestMessageAsync(req);
                _retryRequestsBackgroundService.AddRequestToQueue(reqClone);
                return null;
            }

            var response = await res.Content.ReadFromJsonAsync<LibraryBook>();
            return response;
        }
        catch (Exception e)
        {
            var reqClone = await HttpRequestMessageHelper.CloneHttpRequestMessageAsync(req);
            _retryRequestsBackgroundService.AddRequestToQueue(reqClone);
            return null;
        }
    }

    public async Task<LibraryBook> DecrementAvailableCountByLibUidAndBookUid(Guid libUid, Guid bookUid)
    {
        using var req = new HttpRequestMessage(HttpMethod.Patch,
            $"api/v1/libraries/{libUid}/books/{bookUid}/decrement");
        req.Content = JsonContent.Create(new {value = 1});

        try
        {
            using var res = await _httpClient.SendAsync(req);
            var response = await res.Content.ReadFromJsonAsync<LibraryBook>();
            return response;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public bool IsBookConditionWorse(string oldCondition, string newCondition)
    {
        if (oldCondition == BookConditions.EXCELLENT &&
            (newCondition == BookConditions.GOOD || newCondition == BookConditions.BAD))
        {
            return true;
        }

        if (oldCondition == BookConditions.GOOD && newCondition == BookConditions.BAD)
        {
            return true;
        }

        return false;
    }

    public async Task<bool> HealthCheckAsync()
    {
        using var req = new HttpRequestMessage(HttpMethod.Get,
            "manage/health");
        using var res = await _httpClient.SendAsync(req);
        return res.StatusCode == HttpStatusCode.OK;
    }
}