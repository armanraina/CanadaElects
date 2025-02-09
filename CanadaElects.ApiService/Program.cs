var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


// POST endpoint that accepts VotePercentageResult and returns an array of PartySeatForecast.
app.MapPost("/election-forecast", (VotePercentageResult result) =>
{
    const int totalSeats = 338; // Total seats in the legislature (adjust as needed)
    var forecasts = new List<PartySeatForecast>();

    foreach (var party in result.Parties)
    {
        int? seats = null;
        if (party.VotePercentage.HasValue)
        {
            // Calculate seats by taking the party's vote percentage of the total seats.
            seats = (int)Math.Round(party.VotePercentage.Value / 100 * totalSeats);
        }

        forecasts.Add(new PartySeatForecast
        {
            Name = party.Name,
            Seats = seats
        });
    }

    // Return the forecast as JSON.
    return Results.Json(forecasts);
})
.WithName("GetElectionForecast");

app.MapDefaultEndpoints();

app.Run();

public record PartySeatForecast
{
    public int? Seats { get; set; }
    public required string Name { get; set; }
}
public class VotePercentageResult
{
    public List<PartyPercentage> Parties { get; set; } = new List<PartyPercentage>();
}

// Model representing each party's vote percentage.
public record PartyPercentage
{
    public decimal? VotePercentage { get; set; }
    public required string Name { get; set; }
}