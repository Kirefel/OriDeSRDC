using HarmonyLib;
using OriDeModLoader;

namespace OriDeSRDC
{
    public class OriDeSRDCMod : IMod
    {
        public string Name => "Speedrun.com Leaderboards";

        Harmony harmony;

        public void Init()
        {
            harmony = new Harmony("oridesrdc");
            harmony.PatchAll();
        }

        public void Unload()
        {
            harmony.UnpatchAll();
        }
    }
}
