// Decompiled with JetBrains decompiler
// Type: RimWorld.JobGiver_ManTurrets
// Assembly: Assembly-CSharp, Version=1.5.9214.33606, Culture=neutral, PublicKeyToken=null
// MVID: 630E2863-BC9A-4A34-93F2-EFF01E3A9556
// Assembly location: E:\SteamLibrary\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\Assembly-CSharp.dll

using System;
using Verse;
using Verse.AI;
using RimWorld;
namespace VC_Faction
{
    public abstract class JobGiver_ManTurrets : ThinkNode_JobGiver
    {
        public float maxDistFromPoint = -1f;

        public override ThinkNode DeepCopy(bool resolve = true)
        {
            JobGiver_ManTurrets jobGiverManTurrets = (JobGiver_ManTurrets)base.DeepCopy(resolve);
            jobGiverManTurrets.maxDistFromPoint = this.maxDistFromPoint;
            return (ThinkNode)jobGiverManTurrets;
        }

        protected override Job TryGiveJob(Pawn pawn)
        {
            Predicate<Thing> validator = (Predicate<Thing>)(t =>
            {
                if (!t.def.hasInteractionCell || !t.def.HasComp(typeof(CompMannable)) || !pawn.CanReserve((LocalTargetInfo)t) || JobDriver_ManTurret.FindAmmoForTurret(pawn, (Building_TurretGun)t) == null)
                    return false;
                CompRefuelable comp = t.TryGetComp<CompRefuelable>();
                return comp == null || comp.IsFull || JobDriver_ManTurret.FindFuelForTurret(pawn, (Building_TurretGun)t) != null;
            });
            Thing targetA = GenClosest.ClosestThingReachable(this.GetRoot(pawn), pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.BuildingArtificial), PathEndMode.InteractionCell, TraverseParms.For(pawn), this.maxDistFromPoint, validator);
            if (targetA == null)
                return (Job)null;
            Job job = JobMaker.MakeJob(JobDefOf.VC_ManTurret, (LocalTargetInfo)targetA);
            job.expiryInterval = 2000;
            job.checkOverrideOnExpire = true;
            return job;
        }

        protected abstract IntVec3 GetRoot(Pawn pawn);
    }
}
