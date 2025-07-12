// Decompiled with JetBrains decompiler
// Type: RimWorld.JobDriver_UseCommsConsole
// Assembly: Assembly-CSharp, Version=1.5.9214.33606, Culture=neutral, PublicKeyToken=null
// MVID: 630E2863-BC9A-4A34-93F2-EFF01E3A9556
// Assembly location: E:\SteamLibrary\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace VC_Faction
{
    public class JobDriver_UseRadio : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed) => this.pawn.Reserve(this.job.targetA, this.job, errorOnFailed: errorOnFailed);

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedOrNull<JobDriver_UseRadio>(TargetIndex.A);
            yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.InteractionCell).FailOn((Func<Toil, bool>)(to => !((Building_Radio)to.actor.jobs.curJob.GetTarget(TargetIndex.A).Thing).CanUseCommsNow));
            Toil openComms = ToilMaker.MakeToil(nameof(MakeNewToils));
            openComms.initAction = (Action)(() =>
            {
                Pawn actor = openComms.actor;
                if (!((Building_Radio)actor.jobs.curJob.GetTarget(TargetIndex.A).Thing).CanUseCommsNow || actor.jobs.curJob.commTarget == null)
                    return;
                actor.jobs.curJob.commTarget.TryOpenComms(actor);
            });
            yield return openComms;
        }
    }
}