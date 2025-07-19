// Decompiled with JetBrains decompiler
// Type: RimWorld.WorkGiver_FillFermentingBarrel
// Assembly: Assembly-CSharp, Version=1.5.9214.33606, Culture=neutral, PublicKeyToken=null
// MVID: 630E2863-BC9A-4A34-93F2-EFF01E3A9556
// Assembly location: E:\SteamLibrary\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using VC_Faction;
using Verse;
using Verse.AI;

namespace VC_Faction
{
    public class WorkGiver_FillRagniteReactor : WorkGiver_DoBill
    {
        private static string NoRagniteTrans;

        public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(ThingDefOf.VC_RagniteReactor);

        public override PathEndMode PathEndMode => PathEndMode.Touch;

        public static void ResetStaticData()
        {
            WorkGiver_FillRagniteReactor.NoRagniteTrans = "拉格奈特不足";
        }
        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            bool isSkip = base.ShouldSkip(pawn,forced);

            return isSkip;
        }
        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (!(t is Building_RagniteReactor reactor) || reactor.Refined || reactor.SpaceLeftForRagnite <= reactor.minRagniteCount)
                return false;
            if (t.IsForbidden(pawn) || !pawn.CanReserve((LocalTargetInfo)t, ignoreOtherReservations: forced) || pawn.Map.designationManager.DesignationOn(t, DesignationDefOf.Deconstruct) != null)
                return false;

            if (!(t is IBillGiver giver) || !this.ThingIsUsableBillGiver(t) || !giver.BillStack.AnyShouldDoNow || !giver.UsableForBillsAfterFueling() || !pawn.CanReserve((LocalTargetInfo)t, ignoreOtherReservations: forced) || t.IsBurning() || t.IsForbidden(pawn))
                return false;
            Bill bill = giver.BillStack.FirstShouldDoNow;
            if (bill == null)
            {
                return false;
            }
            Thing ragnite = this.FindRagnite(pawn);
            if (ragnite == null)
            {
                JobFailReason.Is(WorkGiver_FillRagniteReactor.NoRagniteTrans);
                return false;
            }

            if (bill.recipe.defName == RecipeDefOf.FillRagniteReactorBulk.defName)
            {
                if (ragnite.stackCount < 32)
                {
                    JobFailReason.Is(WorkGiver_FillRagniteReactor.NoRagniteTrans);
                    return false;
                }
            }
            else
            {
                if (ragnite.stackCount < 8)
                {
                    JobFailReason.Is(WorkGiver_FillRagniteReactor.NoRagniteTrans);
                    return false;
                }
            }

            CompPowerTrader comp = t.TryGetComp<CompPowerTrader>();
            if (comp != null && !comp.PowerOn)
                return false;
            return !t.IsBurning();
        }
        public bool ThingIsUsableBillGiver(Thing thing)
        {
            Pawn pawn1 = thing as Pawn;
            Corpse corpse = thing as Corpse;
            Pawn pawn2 = (Pawn)null;
            if (corpse != null)
                pawn2 = corpse.InnerPawn;
            return this.def.fixedBillGiverDefs != null && this.def.fixedBillGiverDefs.Contains(thing.def) || pawn1 != null && (this.def.billGiversAllHumanlikes && pawn1.RaceProps.Humanlike || this.def.billGiversAllMechanoids && pawn1.RaceProps.IsMechanoid || this.def.billGiversAllAnimals) || corpse != null && pawn2 != null && (this.def.billGiversAllHumanlikesCorpses && pawn2.RaceProps.Humanlike || this.def.billGiversAllMechanoidsCorpses && pawn2.RaceProps.IsMechanoid || this.def.billGiversAllAnimalsCorpses);
        }
        public override Job JobOnThing(Pawn pawn, Thing thing, bool forced = false)
        {
            IBillGiver giver = (IBillGiver) thing;
            Bill bill = giver.BillStack.FirstShouldDoNow;
            if (bill != null)
            {

                if (bill.recipe.defName == RecipeDefOf.FillRagniteReactorBulk.defName)
                {

                    Thing ragnite = this.FindRagnite(pawn);
                    if (ragnite.stackCount < 32)
                    {
                        return (Job)null;
                    }
                    return JobMaker.MakeJob(JobDefOf.FillRagniteReactorBulk, (LocalTargetInfo)thing, (LocalTargetInfo)ragnite);
                }
                else
                {
                    Thing ragnite = this.FindRagnite(pawn);
                    if (ragnite.stackCount < 8)
                    {
                        return (Job)null;
                    }
                    return JobMaker.MakeJob(JobDefOf.FillRagniteReactor, (LocalTargetInfo)thing, (LocalTargetInfo)ragnite);
                }
            }
            return (Job)null;
        }



        private Thing FindRagnite(Pawn pawn)
        {
            Predicate<Thing> validator = (Predicate<Thing>)(x => !x.IsForbidden(pawn) && pawn.CanReserve((LocalTargetInfo)x));
            return GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(ThingDefOf.Ragnite), PathEndMode.ClosestTouch, TraverseParms.For(pawn), validator: validator);
        }
    }
}
