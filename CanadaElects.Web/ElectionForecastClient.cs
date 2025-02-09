using static CanadaElects.Web.Components.Pages.PollForm;

namespace CanadaElects.Web;

public class ElectionForecastClient(HttpClient httpClient)
{
    public async Task<PartySeatForecast[]> GetWeatherAsync(VotePercentageResult result, CancellationToken cancellationToken = default)
    {
        List<PartySeatForecast>? forecasts = null;

        var response = await httpClient.PostAsJsonAsync<VotePercentageResult>("/election-forecast", result, cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            await foreach (var forecast in response.Content.ReadFromJsonAsAsyncEnumerable<PartySeatForecast>(cancellationToken: cancellationToken))
            {
                if (forecast is not null)
                {
                    forecasts ??= [];
                    forecasts.Add(forecast);
                }
            }
        }
        return forecasts?.ToArray() ?? [];
    }
}

// Model representing each party's vote percentage.
public record PartySeatForecast
{
    public int? Seats { get; set; }
    public required string Name { get; set; }
}
