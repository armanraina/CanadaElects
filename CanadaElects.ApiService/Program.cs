using CanadaElects.ApiService;


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

    var model = new UniformNationalSwing();

    // Return the forecast as JSON.
    return model.Project(result);
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
    public decimal VotePercentage { get; set; }
    public required string Name { get; set; }
}