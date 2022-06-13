using HarmonyLib;
using OriDeModLoader;
using System;

namespace OriDeSRDC
{
    [HarmonyPatch(typeof(LeaderboardsController))]
    public static class LeaderboardsControllerPatches
    {
        [HarmonyPrefix, HarmonyPatch(nameof(LeaderboardsController.UploadScores))]
        private static bool UploadScoresPrefix() => HarmonyHelper.StopExecution;
    }

    [HarmonyPatch(typeof(Steamworks))]
    public static class SteamworksPatches
    {
        [HarmonyPrefix, HarmonyPatch(nameof(Steamworks.SendLeaderboardData), argumentTypes: new Type[] { typeof(LeaderboardB), typeof(int), typeof(int[]) })]
        private static bool SendLeaderboardDataPrefix() => HarmonyHelper.StopExecution;
    }
}
