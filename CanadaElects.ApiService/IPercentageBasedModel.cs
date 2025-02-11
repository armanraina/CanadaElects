namespace CanadaElects.ApiService
{
    public interface IPercentageBasedModel
    {

        public List<PartySeatForecast> Project(VotePercentageResult result);
    }
}
