using System;
using System.Collections.Generic;
using HarmonyLib;
using OriModding.BF.Core;

namespace KFT.OriBF.SRDC;

[HarmonyPatch(typeof(LeaderboardsB))]
public static class LeaderboardsBPatches
{
    [HarmonyPrefix, HarmonyPatch(nameof(LeaderboardsB.UpdateLeaderboard))]
    private static bool UpdateLoaderboardPrefix(Leaderboard leaderboard)
    {
        if (!Plugin.Instance || !LeaderboardsB.Instance.IsVisible || OptionsScreen.Instance.Navigation.Index != OptionsScreen.Instance.Navigation.MenuItems.Count - 1)
            return HarmonyHelper.StopExecution;

        LeaderboardsB.ClearTableUI();
        SRDCLeaderboard.Load(leaderboard, () =>
        {
            LeaderboardB leaderboardB = LeaderboardUtility.LeaderboardToLeaderboardB(leaderboard, DifficultyMode.Normal);

            List<LeaderboardData.Entry> ldes = new List<LeaderboardData.Entry>();
            uint index = 1;
            foreach (var run in SRDCLeaderboard.GetRuns(leaderboard, 10, 0))
            {
                long score = 0;
                ExtractedIntFromInt64 m_time = new ExtractedIntFromInt64(19);
                ExtractedIntFromInt64 m_deathCount = new ExtractedIntFromInt64(1);
                ExtractedIntFromInt64 m_incompletionPercentage = new ExtractedIntFromInt64(21);
                m_incompletionPercentage.Value = (int)(run.Date - DateTime.MinValue).TotalDays;
                m_incompletionPercentage.Encode(ref score);
                m_time.Value = run.Time;
                m_time.Encode(ref score);
                m_deathCount.Value = 0;
                m_deathCount.Encode(ref score);

                ldes.Add(new LeaderboardData.Entry(leaderboard, index++, score, "id." + run.Username, run.Username));
            }
            LeaderboardData ld = new LeaderboardData(leaderboard, Leaderboards.Filter.Overall, "???", 10, ldes);

            var m_data = Traverse.Create(LeaderboardsB.Instance).Field("m_data").GetValue<Dictionary<LeaderboardB, LeaderboardData>>();

            if (!m_data.ContainsKey(leaderboardB) || !m_data[leaderboardB].Update(ld))
            {
                m_data[leaderboardB] = ld;
            }

            LeaderboardsB.RefreshTableUI();
        });

        return HarmonyHelper.StopExecution;
    }

    private static readonly Dictionary<Leaderboard, string> leaderboardCategoryMap = new Dictionary<Leaderboard, string>()
    {
        [Leaderboard.Explorer] = "All Skills No OOB/TA",
        [Leaderboard.SpeedRunner] = "All Cells No NG+",
        [Leaderboard.Survivor] = "Any% No NG+"
    };

    [HarmonyPrefix, HarmonyPatch(nameof(LeaderboardsB.RefreshUIStrings))]
    private static bool RefreshUIStringsPrefix()
    {
        var currentLeaderboard = Traverse.Create(LeaderboardsB.Instance).Field("m_currentLeaderboard").GetValue<Leaderboard>();

        LeaderboardsB.Instance.LeaderboardTitlePC.SetMessage(new MessageDescriptor("Category: " + leaderboardCategoryMap[currentLeaderboard]));
        LeaderboardsB.Instance.FilterTextPC.SetMessage(new MessageDescriptor(""));
        LeaderboardsB.Instance.DifficultyTextPC.SetMessage(new MessageDescriptor(""));
        return HarmonyHelper.StopExecution;
    }

    [HarmonyPrefix, HarmonyPatch("NextFilter")]
    private static bool NextFilterPrefix() => HarmonyHelper.StopExecution;
    [HarmonyPrefix, HarmonyPatch("NextDifficulty")]
    private static bool NextDifficultyPrefix() => HarmonyHelper.StopExecution;
}

[HarmonyPatch(typeof(LeaderboardData.Entry))]
public static class LeaderboardDataPatches
{
    [HarmonyPrefix, HarmonyPatch(MethodType.Constructor, argumentTypes: new Type[] { typeof(Leaderboard), typeof(uint), typeof(long), typeof(string), typeof(string) })]
    private static bool ConstructorPrefix(Leaderboard leaderboard, uint rank, long score, string userID, string userHandle, LeaderboardData.Entry __instance, ref int ___m_time, ref int ___m_deathCount, ref int ___m_completionPercentage)
    {
        var traverse = Traverse.Create(__instance);
        traverse.Property("Rank").SetValue(rank);
        traverse.Property("UserID").SetValue(userID);
        traverse.Property("UserHandle").SetValue(userHandle);

        ExtractedIntFromInt64 m_time = new ExtractedIntFromInt64(19);
        ExtractedIntFromInt64 m_deathCount = new ExtractedIntFromInt64(1);
        ExtractedIntFromInt64 m_incompletionPercentage = new ExtractedIntFromInt64(21);
        m_deathCount.Decode(ref score);
        m_time.Decode(ref score);
        m_incompletionPercentage.Decode(ref score);

        ___m_time = (int)m_time.Value;
        ___m_deathCount = (int)m_deathCount.Value;
        ___m_completionPercentage = (int)m_incompletionPercentage.Value;

        return HarmonyHelper.StopExecution;
    }
}
