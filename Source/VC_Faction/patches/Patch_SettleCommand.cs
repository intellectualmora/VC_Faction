using System;
using HarmonyLib;
using Verse;
using System.Collections.Generic;
using RimWorld;
using RimWorld.BaseGen;
using UnityEngine;
using RimWorld.Planet;
namespace VC_Faction
{
    [HarmonyPatch]
    public static class Patch_SettleInExistingMapUtility
    {
        // 上衣和裤子的渲染层补丁，衣服覆盖裤子！！
       

        [HarmonyPatch(typeof(SettleInExistingMapUtility), "SettleCommand")]
        [HarmonyPostfix]
        public static void SettleCommand_Postfix(Map map, bool requiresNoEnemies, ref Command __result)
        {
            if(map.ParentFaction.def == FactionDefOf.VC_Gallia || map.ParentFaction.def == FactionDefOf.VC_Empire || map.ParentFaction.def == FactionDefOf.VC_Federation)
                __result.Disable((string)"不能占领此地".Translate());
        }
        
    }
}
