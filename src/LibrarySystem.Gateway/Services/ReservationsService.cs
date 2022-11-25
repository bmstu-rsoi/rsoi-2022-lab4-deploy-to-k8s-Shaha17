using System.Net;
using LibrarySystem.Gateway.DTO;
using LibrarySystem.ReservationSystem.Models;

namespace LibrarySystem.Gateway.Services;

public class ReservationsService
{
    private readonly HttpClient _httpClient;

    public ReservationsService(string reservationsServiceHost)
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri($"http://{reservationsServiceHost}/");
    }

    public async Task<IEnumerable<Reservation>?> GetReservationsByUsernameAsync(string username)
    {
        using var req = new HttpRequestMessage(HttpMethod.Get, "api/v1/reservations");
        req.Headers.Add("X-User-Name", username);
        using var res = await _httpClient.SendAsync(req);
        var response = await res.Content.ReadFromJsonAsync<IEnumerable<Reservation>>();
        return response;
    }

    public async Task<Reservation?> TakeBook(string username, TakeBookRequest request)
    {
        using var req = new HttpRequestMessage(HttpMethod.Post, "api/v1/reservations");
        req.Headers.Add("X-User-Name", username);
        req.Content = JsonContent.Create(request, typeof(TakeBookRequest));
        using var res = await _httpClient.SendAsync(req);
        var response = await res.Content.ReadFromJsonAsync<Reservation>();
        return response;
    }

    public async Task<Reservation?> GetReservationsByUidAsync(Guid reservationUid)
    {
        using var req = new HttpRequestMessage(HttpMethod.Get, $"api/v1/reservations/{reservationUid}");
        using var res = await _httpClient.SendAsync(req);
        var response = await res.Content.ReadFromJsonAsync<Reservation>();
        return response;
    }

    public async Task<Reservation?> UpdateReservationByUidAsync(Reservation reservation)
    {
        using var req = new HttpRequestMessage(HttpMethod.Put, $"api/v1/reservations/{reservation.ReservationUid}");
        req.Content = JsonContent.Create(reservation, typeof(Reservation));
        using var res = await _httpClient.SendAsync(req);
        var response = await res.Content.ReadFromJsonAsync<Reservation>();
        return response;
    }

    public async Task<bool> HealthCheckAsync()
    {
        using var req = new HttpRequestMessage(HttpMethod.Get,
            $"manage/health");
        using var res = await _httpClient.SendAsync(req);
        return res.StatusCode == HttpStatusCode.OK;
    }

    public async Task RollbackReservationAsync(Guid reservationReservationUid)
    {
        using var req = new HttpRequestMessage(HttpMethod.Delete, $"api/v1/reservations/{reservationReservationUid}");
        using var res = await _httpClient.SendAsync(req);
    }
}