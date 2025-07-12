using HarmonyLib;
using Verse;
using System.Collections.Generic;
using RimWorld.BaseGen;

namespace Valkyria
{
    [HarmonyPatch]
    public static class Patch_SymbolResolver_AncientShrine
    {
        private static int worldSeed = 0;
        [HarmonyPatch(typeof(SymbolResolver_AncientShrine), "Resolve")]
        [HarmonyPostfix]
        public static void SymbolResolver_AncientShrine_Postfix(ResolveParams rp)
        {
            if (worldSeed != Find.World.ConstantRandSeed)
            {
                ResolveParams resolveParams = rp;
                resolveParams.singleThingDef = ValkyriaThingDefOf.RelicSignalReceiver;
                RimWorld.BaseGen.BaseGen.symbolStack.Push("thing", resolveParams);
                worldSeed = Find.World.ConstantRandSeed;
            }
        }
    }
}
