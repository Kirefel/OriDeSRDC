using System;
using System.Collections.Generic;

namespace KFT.OriBF.SRDC;

public static class SRDCLeaderboard
{
    private static Dictionary<Leaderboard, SRDCRun[]> runCache;

    public static void Load(Leaderboard leaderboard, Action callback)
    {
        if (runCache != null && runCache.ContainsKey(leaderboard))
        {
            callback();
            return;
        }

        Plugin.Instance.Load(leaderboard, jsonString =>
        {
            var sj = SimpleJSON.JSON.Parse(jsonString);

            var runs = sj["data"]["runs"].AsArray;
            var players = sj["data"]["players"]["data"].AsArray;

            // runs.Count will always equal players.Count and runs[i] will always be player[i]'s run
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
                    Date = DateTime.Parse(runs[i]["run"]["date"])
                };
            }

            callback();
        });
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
    public DateTime Date { get; set; }
}
