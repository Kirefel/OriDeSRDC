using System;
using System.Linq;
using HarmonyLib;
using OriDeModLoader;
using UnityEngine;

namespace OriDeSRDC
{
    [HarmonyPatch(typeof(LeaderboardRowUI))]
    public static class LeaderboardRowUIPatches
    {
        [HarmonyPostfix, HarmonyPatch(nameof(LeaderboardRowUI.SetContent))]
        private static void SetContentPostfix(LeaderboardRowUI __instance, LeaderboardData.Entry entry)
        {
            if (__instance.Completion)
            {
                __instance.Completion.SetMessage(new MessageDescriptor(DateTime.MinValue.AddDays(entry.CompletionPercentage).ToString("yyyy-MM-dd")));
            }
            if (__instance.Deaths)
            {
                __instance.Deaths.SetMessage(new MessageDescriptor(""));
            }
        }
    }

    [HarmonyPatch(typeof(LeaderboardTableUI))]
    public static class LeaderboardTableUIPatches
    {
        [HarmonyPrefix, HarmonyPatch("get_CurrentMetaData")]
        private static bool get_CurrentMetaDataPrefix(LeaderboardTableUI __instance, ref LeaderboardTableUI.LeaderboardMetaData __result)
        {
            __result = __instance.MetaData.FirstOrDefault(x => x.Leaderboard == Leaderboard.SpeedRunner); // Defines column and row layout
            return HarmonyHelper.StopExecution;
        }

        [HarmonyPostfix, HarmonyPatch(nameof(LeaderboardTableUI.GenerateTable))]
        private static void GenerateTablePostfix(GameObject ___m_header)
        {
            if (___m_header)
            {
                UnityEngine.Object.Destroy(___m_header.transform.Find("deathsText").gameObject);
                ___m_header.transform.Find("completionText").GetComponent<MessageBox>().SetMessage(new MessageDescriptor("Date"));
            }
        }
    }

    [HarmonyPatch(typeof(LeaderboardsB), nameof(LeaderboardsB.Awake))]
    public static class OpenLeaderboardsWithoutSteam
    {
        private static void Postfix(LeaderboardsB __instance)
        {
            __instance.transform.Find("offline").gameObject.SetActive(false);
            __instance.transform.Find("highlightFade").gameObject.SetActive(true);
        }
    }
}
