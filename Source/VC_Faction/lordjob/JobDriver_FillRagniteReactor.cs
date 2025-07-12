// Decompiled with JetBrains decompiler
// Type: RimWorld.JobDriver_FillFermentingBarrel
// Assembly: Assembly-CSharp, Version=1.5.9214.33606, Culture=neutral, PublicKeyToken=null
// MVID: 630E2863-BC9A-4A34-93F2-EFF01E3A9556
// Assembly location: E:\SteamLibrary\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using RimWorld;
using UnityEngine;

namespace VC_Faction
{
    public class JobDriver_FillRagniteReactor : JobDriver
    {
        private const int RagniteCount = 8;

        protected Building_RagniteReactor Reactor => (Building_RagniteReactor)this.job.GetTarget(TargetIndex.A).Thing;

        protected Thing Ragnite => this.job.GetTarget(TargetIndex.B).Thing;

        public override bool TryMakePreToilReservations(bool errorOnFailed) => this.pawn.Reserve((LocalTargetInfo)(Thing)this.Reactor, this.job, errorOnFailed: errorOnFailed) && this.pawn.Reserve((LocalTargetInfo)this.Ragnite, this.job, errorOnFailed: errorOnFailed);

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.FailOnBurningImmobile(TargetIndex.A);
            AddEndCondition(() =>
            {
                return Reactor.SpaceLeftForRagnite <= RagniteCount ? JobCondition.Incompletable : JobCondition.Ongoing;
            }); ;
            yield return Toils_General.DoAtomic(delegate
            {
                job.count = RagniteCount;
            });
            Toil reserveRagnite = Toils_Reserve.Reserve(TargetIndex.B);
            yield return reserveRagnite;
            yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(TargetIndex.B).FailOnSomeonePhysicallyInteracting(TargetIndex.B);
            yield return Toils_Haul.StartCarryThing(TargetIndex.B, putRemainderInQueue: false, subtractNumTakenFromJobCount: true).FailOnDestroyedNullOrForbidden(TargetIndex.B);
            yield return Toils_Haul.CheckForGetOpportunityDuplicate(reserveRagnite, TargetIndex.B, TargetIndex.None, takeFromValidStorage: true);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            yield return Toils_General.Wait(200).FailOnDestroyedNullOrForbidden(TargetIndex.B).FailOnDestroyedNullOrForbidden(TargetIndex.A)
                .FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch)
                .WithProgressBarToilDelay(TargetIndex.A);
            Toil toil = ToilMaker.MakeToil("MakeNewToils");
            toil.initAction = delegate
            {
                Reactor.AddRagnite(Ragnite);
                IBillGiver giver = (IBillGiver)TargetA.Thing;
                giver.BillStack.Bills.Remove(giver.BillStack.FirstShouldDoNow);
            };
            toil.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return toil;
        }
    }
}
