using System.Runtime.InteropServices.ComTypes;
using LibrarySystem.LibrarySystem.Context;
using LibrarySystem.LibrarySystem.HealthChecks;


var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddDbContext<LibrariesContext>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<LibrariesContext>(_ => new LibrariesContext());
builder.Services.AddHealthChecks().AddCheck<LibrarySystemHealthCheck>(nameof(LibrarySystemHealthCheck));

var app = builder.Build();

//Seed data 
{
    await Task.Delay(TimeSpan.FromSeconds(5));
    var scope = app.Services.CreateAsyncScope();
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var librariesContext = services.GetRequiredService<LibrariesContext>();
        ContextHelper.Seed(librariesContext);
        logger.LogInformation("DB filled");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error while try to fill database");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/manage/health");

app.Run();