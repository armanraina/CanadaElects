using System.Runtime.Intrinsics.Arm;
using System.Xml.Linq;
using Microsoft.Data.Analysis;

namespace CanadaElects.ApiService
{
    public class UniformNationalSwing : IPercentageBasedModel
    {
        private readonly VotePercentageResult baseVotePercentageResult;
        private readonly DataFrame dataFrame;
        private readonly Dictionary<string, string> officialNameMap = new Dictionary<string, string>()
        {
                ["Conservative/Conservateur"] ="Conservative",
                ["Liberal/Libéral"] = "Liberal",
                ["NDP-New Democratic Party/NPD-Nouveau Parti démocratique"] = "NDP",
                ["Bloc Québécois/Bloc Québécois"] = "BQ",
                ["People's Party - PPC/Parti populaire - PPC"] = "PPC",
                ["Green Party/Parti Vert"] = "Green"
        };


        public UniformNationalSwing()
        {
            dataFrame = DataFrame.LoadCsv("2021.csv");
            // Print out the DataFrame to see its content.


        baseVotePercentageResult = new VotePercentageResult
        {
            Parties = new List<PartyPercentage>
            {
                new PartyPercentage { VotePercentage = 33.74m, Name = "Conservative/Conservateur" },
                new PartyPercentage { VotePercentage = 32.62m, Name = "Liberal/Libéral" },
                new PartyPercentage { VotePercentage = 17.82m, Name = "NDP-New Democratic Party/NPD-Nouveau Parti démocratique" },
                new PartyPercentage { VotePercentage = 7.65m,  Name = "Bloc Québécois/Bloc Québécois" },
                new PartyPercentage { VotePercentage = 4.94m,  Name = "People's Party - PPC/Parti populaire - PPC" },
                new PartyPercentage { VotePercentage = 2.33m,  Name = "Green Party/Parti Vert" },
            }
        };

        }
        public List<PartySeatForecast> Project(VotePercentageResult result)
        {
            var shiftMap = new Dictionary<string, decimal>();
            var forecastDictionary = new Dictionary<string, int>();


            foreach (var party in result.Parties)
            {
                var baseResult = baseVotePercentageResult.Parties.Find(x => x.Name == party.Name);
                shiftMap[officialNameMap[party.Name]] = party.VotePercentage - baseResult!.VotePercentage;
            }

            foreach (var party in shiftMap.Keys)
            {
                dataFrame[party] = dataFrame[party].Add(shiftMap[party]);
                forecastDictionary[party] = 0;
            }

            // Loop over each row in the DataFrame.
            for (long rowIndex = 0; rowIndex < dataFrame.Rows.Count; rowIndex++)
            {
                decimal currentMax = decimal.MinValue;
                string nameOfMaxCol = String.Empty;

                // If you want to consider only numeric columns, you can either list them
                // explicitly or filter by type. In this example, we attempt a type-check 
                // on each column.
                foreach (DataFrameColumn col in dataFrame.Columns)
                {
                    if (col is PrimitiveDataFrameColumn<decimal> doubleCol)
                    {
                        // Get the value at this row.
                        decimal value = doubleCol[rowIndex]!.Value;

                        if (value > currentMax)
                        {
                            currentMax = value;
                            nameOfMaxCol = col.Name;
                        }
                    }
                }


                forecastDictionary[nameOfMaxCol]++;
            }

            var forecasts = new List<PartySeatForecast>();
            foreach (var key in forecastDictionary.Keys)
            {
                forecasts.Add(new() { Name = key, Seats = forecastDictionary[key] });
            }
            return forecasts;
        }
        
    }
}
