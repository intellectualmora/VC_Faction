// Decompiled with JetBrains decompiler
// Type: RimWorld.JobDriver_TakeBeerOutOfFermentingBarrel
// Assembly: Assembly-CSharp, Version=1.5.9214.33606, Culture=neutral, PublicKeyToken=null
// MVID: 630E2863-BC9A-4A34-93F2-EFF01E3A9556
// Assembly location: E:\SteamLibrary\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace VC_Faction
{
    public class JobDriver_TakeLiquidRagniteOutOfReactor : JobDriver
    {
        private const TargetIndex BarrelInd = TargetIndex.A;
        private const TargetIndex BeerToHaulInd = TargetIndex.B;
        private const TargetIndex StorageCellInd = TargetIndex.C;
        private const int Duration = 200;

        protected Building_RagniteReactor Reactor => (Building_RagniteReactor)this.job.GetTarget(TargetIndex.A).Thing;

        protected Thing Beer => this.job.GetTarget(TargetIndex.B).Thing;

        public override bool TryMakePreToilReservations(bool errorOnFailed) => this.pawn.Reserve((LocalTargetInfo)(Thing)this.Reactor, this.job, errorOnFailed: errorOnFailed);

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden<JobDriver_TakeLiquidRagniteOutOfReactor>(TargetIndex.A);
            this.FailOnBurningImmobile<JobDriver_TakeLiquidRagniteOutOfReactor>(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            yield return Toils_General.Wait(200).FailOnDestroyedNullOrForbidden(TargetIndex.A).FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch)
                .FailOn(() => !Reactor.Refined)
                .WithProgressBarToilDelay(TargetIndex.A);
            Toil toil = ToilMaker.MakeToil(nameof(MakeNewToils));
            toil.initAction = delegate
            {
                Thing thing = Reactor.TakeOutRagnite();
                GenPlace.TryPlaceThing(thing, pawn.Position, base.Map, ThingPlaceMode.Near);
                StoragePriority currentPriority = StoreUtility.CurrentStoragePriorityOf(thing);
                if (StoreUtility.TryFindBestBetterStoreCellFor(thing, pawn, base.Map, currentPriority, pawn.Faction, out var foundCell))
                {
                    job.SetTarget(TargetIndex.C, foundCell);
                    job.SetTarget(TargetIndex.B, thing);
                    job.count = thing.stackCount;
                }
                else
                {
                    EndJobWith(JobCondition.Incompletable);
                }
            };
            toil.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return toil;
            yield return Toils_Reserve.Reserve(TargetIndex.B);
            yield return Toils_Reserve.Reserve(TargetIndex.C);
            yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch);
            yield return Toils_Haul.StartCarryThing(TargetIndex.B);
            Toil carryToCell = Toils_Haul.CarryHauledThingToCell(TargetIndex.C);
            yield return carryToCell;
            yield return Toils_Haul.PlaceHauledThingInCell(TargetIndex.C, carryToCell, true);
        }
    }
}
