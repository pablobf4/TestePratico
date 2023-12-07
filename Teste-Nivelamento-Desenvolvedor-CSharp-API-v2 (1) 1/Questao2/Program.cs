using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

public class Program
{
    public static async Task Main()
    {
        string teamName = "Paris Saint-Germain";
        int year = 2013;
        int totalGoals = await GetTotalScoredGoals(teamName, year);

        Console.WriteLine($"Team {teamName} scored {totalGoals} goals in {year}");

        teamName = "Chelsea";
        year = 2014;
        totalGoals = await GetTotalScoredGoals(teamName, year);

        Console.WriteLine($"Team {teamName} scored {totalGoals} goals in {year}");

        // Output expected:
        // Team Paris Saint-Germain scored 21 goals in 2013 
        // Team Chelsea scored 26 goals in 2014

    }

    public static async Task<int> GetTotalScoredGoals(string team, int year)
    {
        using (var client = new HttpClient())
        {
            string apiUrl = $"https://jsonmock.hackerrank.com/api/football_matches?year={year}&team1={team}";

            HttpResponseMessage response = await client.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            JObject data = JObject.Parse(responseBody);

            int totalGoals = 0;

            foreach (var match in data["data"])
            {
                if (match["team1"].ToString() == team)
                {
                    totalGoals += Convert.ToInt32(match["team1goals"]);
                }
            }

            return totalGoals;
        }
    }
}
