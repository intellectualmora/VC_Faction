// Decompiled with JetBrains decompiler
// Type: RimWorld.CompAbilityEffect_GiveHediff
// Assembly: Assembly-CSharp, Version=1.5.9214.33606, Culture=neutral, PublicKeyToken=null
// MVID: 630E2863-BC9A-4A34-93F2-EFF01E3A9556
// Assembly location: E:\SteamLibrary\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\Assembly-CSharp.dll

using Verse;
using RimWorld;
namespace VC_Faction
{
    public class CompAbilityEffect_AllGiveHediff : CompAbilityEffect_WithDuration
    {
        

        public CompProperties_AbilityAllGiveHediff Props => (CompProperties_AbilityAllGiveHediff)this.props;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            this.ApplyInner(this.parent.pawn, target.Pawn);
        }

        protected void ApplyInner(Pawn target, Pawn other)
        {
            if (target == null)
                return;
            else
            {
                foreach (Pawn pawn in target.Map.mapPawns.AllHumanlikeSpawned)
                {
                    if (pawn.Faction.def == target.Faction.def)
                    {
                        if (this.Props.replaceExisting)
                        {
                            Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(this.Props.hediffDef);
                            if (firstHediffOfDef != null)
                                pawn.health.RemoveHediff(firstHediffOfDef);
                        }
                        Hediff hediff = HediffMaker.MakeHediff(this.Props.hediffDef, pawn, this.Props.onlyBrain ? pawn.health.hediffSet.GetBrain() : (BodyPartRecord)null);
                        HediffComp_Disappears comp1 = hediff.TryGetComp<HediffComp_Disappears>();
                        if (comp1 != null)
                            comp1.ticksToDisappear = this.GetDurationSeconds(pawn).SecondsToTicks();
                        if ((double)this.Props.severity >= 0.0)
                            hediff.Severity = this.Props.severity;
                        HediffComp_Link comp2 = hediff.TryGetComp<HediffComp_Link>();
                        if (comp2 != null)
                        {
                            comp2.other = (Thing)other;
                            comp2.drawConnection = pawn == this.parent.pawn;
                        }
                        pawn.health.AddHediff(hediff);
                    }
                }
            }
        }

        public override bool AICanTargetNow(LocalTargetInfo target) => this.parent.pawn.Faction != Faction.OfPlayer && target.Pawn != null;
    }
}
