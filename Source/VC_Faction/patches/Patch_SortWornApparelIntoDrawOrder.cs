using System;
using HarmonyLib;
using Verse;
using System.Collections.Generic;
using RimWorld;
using RimWorld.BaseGen;
using UnityEngine;

namespace VC_Faction
{
    [HarmonyPatch]
    public static class Patch_ApparelGraphicRecordGetter
    {
        // 上衣和裤子的渲染层补丁，衣服覆盖裤子！！
        public static bool IsLowerBodyApparel(Apparel apparel)
        {
            return apparel.def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.Legs);
        }

        [HarmonyPatch(typeof(Pawn_ApparelTracker), "SortWornApparelIntoDrawOrder")]
        [HarmonyPrefix]
        public static bool SortWornApparelIntoDrawOrder_Prefix(Pawn_ApparelTracker __instance)
        {
            ThingOwner<Apparel> wornApparel = (ThingOwner<Apparel>)Traverse.Create(__instance).Field("wornApparel").GetValue<ThingOwner<Apparel>>();
            wornApparel.InnerListForReading.Sort((Comparison<Apparel>)((a, b) =>
            {
                int result = a.def.apparel.LastLayer.drawOrder.CompareTo(b.def.apparel.LastLayer.drawOrder);

                if (result == 0)
                {
                    // 如果图层相同，进一步判断 bodyPart
                    bool aIsLower = IsLowerBodyApparel(a);
                    bool bIsLower = IsLowerBodyApparel(b);

                    if (aIsLower && !bIsLower) return -1;  // 上衣在裤子上面
                    if (!aIsLower && bIsLower) return 1; // 裤子在下层
                    return 0;
                }

                return result;
            }));
            Traverse.Create(__instance).Field("wornApparel").SetValue(wornApparel);
            return false;
        }
    }
}
