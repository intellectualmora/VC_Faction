// Decompiled with JetBrains decompiler
// Type: RimWorld.CompProperties_AbilityGiveHediff
// Assembly: Assembly-CSharp, Version=1.5.9214.33606, Culture=neutral, PublicKeyToken=null
// MVID: 630E2863-BC9A-4A34-93F2-EFF01E3A9556
// Assembly location: E:\SteamLibrary\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\Assembly-CSharp.dll

using Verse;
using RimWorld;

namespace VC_Faction
{
    public class CompProperties_AbilityAllGiveHediff : CompProperties_AbilityEffectWithDuration
    {
        public HediffDef hediffDef;
        public bool onlyBrain;
        public bool applyToSelf;
        public bool onlyApplyToSelf;
        public bool applyToTarget = true;
        public bool replaceExisting = true;
        public float severity = -1f;
        public bool ignoreSelf;
    }
}