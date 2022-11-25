using LibrarySystem.Gateway.HealthChecks;
using LibrarySystem.Gateway.Services;
using LibrarySystem.Gateway.Utils.Converters;


await Task.Delay(TimeSpan.FromSeconds(10));

var builder = WebApplication.CreateBuilder(args);


var retryRequestsBackgroundService = new RetryRequestsBackgroundService();
retryRequestsBackgroundService.StartWorker();
// Add services to the container.
builder.Services.AddTransient<LibrariesService>((serviceProvider) =>
{
    var libraryServiceHost = Environment.GetEnvironmentVariable("LIBRARY_HOST");
    return new LibrariesService(libraryServiceHost, retryRequestsBackgroundService);
});
builder.Services.AddTransient<RatingService>((serviceProvider) =>
{
    var ratingServiceHost = Environment.GetEnvironmentVariable("RATINGS_HOST");
    return new RatingService(ratingServiceHost, retryRequestsBackgroundService);
});

builder.Services.AddTransient<ReservationsService>(_ =>
{
    var reservationsServiceHost = Environment.GetEnvironmentVariable("RESERVATIONS_HOST");
    return new ReservationsService(reservationsServiceHost);
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new TimeOnlyJsonConverter());
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks().AddCheck<GatewayHealthCheck>(nameof(GatewayHealthCheck));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/manage/health");


app.Run();