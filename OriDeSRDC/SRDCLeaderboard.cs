using System;
using System.Collections.Generic;
using System.IO;

namespace OriDeSRDC
{
    public static class SRDCLeaderboard
    {
        private static Dictionary<Leaderboard, SRDCRun[]> runCache;

        private static Dictionary<Leaderboard, string> leaderboardGetMap = new Dictionary<Leaderboard, string>
        {
            [Leaderboard.Explorer] = "https://www.speedrun.com/api/v1/leaderboards/v1pwgmd8/category/zdnw64ed?embed=players&var-wle6xqe8=5lm8p48q", // All Skills No OOB/TA
            [Leaderboard.SpeedRunner] = "https://www.speedrun.com/api/v1/leaderboards/v1pwgmd8/category/jdrqz7gk?embed=players&var-j84k62yn=rqv69rrl", // All Cells No NG+
            [Leaderboard.Survivor] = "https://www.speedrun.com/api/v1/leaderboards/v1pwgmd8/category/q25oyqyk?embed=players&var-68kmd2zl=zqom9351" // Any% No NG+
        };


        public static void Load(Leaderboard leaderboard, Action callback)
        {
            string jsonString = File.ReadAllText(leaderboardGetMap[leaderboard]);
            var sj = SimpleJSON.JSON.Parse(jsonString);

            var runs = sj["data"]["runs"].AsArray;
            var players = sj["data"]["players"]["data"].AsArray;

            // runs.Count will always equal players.Count
            // todo confirm that

            if (runCache == null)
                runCache = new Dictionary<Leaderboard, SRDCRun[]>();
            runCache[leaderboard] = new SRDCRun[runs.Count];

            for (int i = 0; i < runs.Count; i++)
            {
                runCache[leaderboard][i] = new SRDCRun
                {
                    Username = players[i]["names"]["international"],
                    Time = runs[i]["run"]["times"]["primary_t"],
                    Date = runs[i]["run"]["date"]
                };
            }
        }

        public static IEnumerable<SRDCRun> GetRuns(Leaderboard leaderboard, int count, int offset)
        {
            if (runCache == null || !runCache.ContainsKey(leaderboard))
                throw new Exception("Call Load() to load this leaderboard: " + leaderboard);

            SRDCRun[] runs = runCache[leaderboard];

            if (offset >= runs.Length || offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), $"offset must be between 0 and {runs.Length - 1}");

            for (int i = offset; i < count && i < runs.Length; i++)
                yield return runs[i];
        }
    }

    public class SRDCRun
    {
        public string Username { get; set; }
        public int Time { get; set; }
        public string Date { get; set; }
    }
}
