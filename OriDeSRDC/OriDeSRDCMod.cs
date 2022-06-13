using BaseModLib;
using HarmonyLib;
using OriDeModLoader;

namespace OriDeSRDC
{
    public class OriDeSRDCMod : IMod
    {
        public string Name => "Speedrun.com Leaderboards";

        private Harmony harmony;

        public void Init()
        {
            Controllers.Add<SRDCLoader>();

            harmony = new Harmony("oridesrdc");
            harmony.PatchAll();
        }

        public void Unload()
        {
            harmony.UnpatchAll();
        }
    }
}
