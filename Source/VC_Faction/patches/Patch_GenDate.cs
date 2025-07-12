using System;
using HarmonyLib;
using Verse;
using System.Collections.Generic;
using System.Reflection.Emit;
using RimWorld;
using RimWorld.BaseGen;
using UnityEngine;
using RimWorld.Planet;
namespace VC_Faction
{

    [HarmonyPatch(typeof(GenDate), nameof(GenDate.Year))]
    public static class Patch_GenDate_Year
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var code in instructions)
            {
                // 把原本加载常数5500的指令改为加载1936
                if (code.opcode == OpCodes.Ldc_I4 && (int)code.operand == 5500)
                {
                    yield return new CodeInstruction(OpCodes.Ldc_I4, 1935);
                }
                else
                {
                    yield return code;
                }
            }
        }
    }

}
