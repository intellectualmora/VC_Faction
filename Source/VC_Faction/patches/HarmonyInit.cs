using HarmonyLib;
using RimWorld;
using System.Reflection;
using Verse;
namespace VC_Faction
{
    [StaticConstructorOnStartup]
    internal static class HarmonyInit
    {
        static HarmonyInit()
        {
            Harmony harmony = new Harmony("VC_Faction");
            harmony.PatchAll();
        }
    }

}
