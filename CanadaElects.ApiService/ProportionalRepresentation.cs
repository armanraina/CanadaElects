
namespace CanadaElects.ApiService
{
    public class ProportionalRepresentation:IPercentageBasedModel
    {
        public ProportionalRepresentation() { }

        public List<PartySeatForecast> Project(VotePercentageResult result)
        {
            const int totalSeats = 338; // Total seats in the legislature (adjust as needed)
            var forecasts = new List<PartySeatForecast>();

            foreach (var party in result.Parties)
            {
                int seats = (int)Math.Round(party.VotePercentage / 100 * totalSeats);


                forecasts.Add(new PartySeatForecast
                {
                    Name = party.Name,
                    Seats = seats
                });
            }
            return forecasts;
        }
    }
}
