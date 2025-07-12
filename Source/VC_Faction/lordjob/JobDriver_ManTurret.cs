// Decompiled with JetBrains decompiler
// Type: RimWorld.JobDriver_ManTurret
// Assembly: Assembly-CSharp, Version=1.5.9214.33606, Culture=neutral, PublicKeyToken=null
// MVID: 630E2863-BC9A-4A34-93F2-EFF01E3A9556
// Assembly location: E:\SteamLibrary\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using Verse.Sound;
using RimWorld;
namespace VC_Faction
{
    public class JobDriver_ManTurret : JobDriver
    {
        private const float SearchRadius = 40f;
        private const int MaxPawnReservations = 10;

        private static bool GunNeedsLoading(Building b)
        {
            if (!(b is Building_TurretGun buildingTurretGun))
                return false;
            CompChangeableProjectile comp = buildingTurretGun.TryGetComp<CompChangeableProjectile>();
            return comp != null && !comp.Loaded;
        }

        private static bool GunNeedsRefueling(Building b)
        {
            if (!(b is Building_TurretGun thing))
                return false;
            CompRefuelable comp = thing.TryGetComp<CompRefuelable>();
            return comp != null && !comp.HasFuel && comp.Props.fuelIsMortarBarrel && !Find.Storyteller.difficulty.classicMortars;
        }

        public static Thing FindAmmoForTurret(Pawn pawn, Building_TurretGun gun)
        {
            StorageSettings allowedShellsSettings = pawn.IsColonist || pawn.IsColonyMech ? gun.TryGetComp<CompChangeableProjectile>().allowedShellsSettings : (StorageSettings)null;
            return GenClosest.ClosestThingReachable(gun.Position, gun.Map, ThingRequest.ForGroup(ThingRequestGroup.Shell), PathEndMode.OnCell, TraverseParms.For(pawn), 40f, new Predicate<Thing>(ShellValidator));

            bool ShellValidator(Thing t) => !t.IsForbidden(pawn) && pawn.CanReserve((LocalTargetInfo)t, 10, 1) && (allowedShellsSettings == null || allowedShellsSettings.AllowedToAccept(t)) && (pawn.Faction == Faction.OfPlayer || t.def.projectileWhenLoaded?.projectile == null || t.def.projectileWhenLoaded.projectile.damageDef.harmsHealth);
        }

        public static Thing FindFuelForTurret(Pawn pawn, Building_TurretGun gun)
        {
            CompRefuelable refuelableComp = gun.TryGetComp<CompRefuelable>();
            return refuelableComp == null ? (Thing)null : GenClosest.ClosestThingReachable(gun.Position, gun.Map, ThingRequest.ForGroup(ThingRequestGroup.HaulableEver), PathEndMode.OnCell, TraverseParms.For(pawn), 40f, new Predicate<Thing>(FuelValidator));

            bool FuelValidator(Thing t) => !t.IsForbidden(pawn) && pawn.CanReserve((LocalTargetInfo)t, 10, 1) && refuelableComp.Props.fuelFilter.Allows(t);
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed) => this.pawn.Reserve(this.job.targetA, this.job, errorOnFailed: errorOnFailed);

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden<JobDriver_ManTurret>(TargetIndex.A);
            Toil gotoTurret = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);
            Toil refuelIfNeeded = ToilMaker.MakeToil(nameof(MakeNewToils));
            refuelIfNeeded.initAction = (Action)(() =>
            {
                Pawn actor = refuelIfNeeded.actor;
                Building thing = (Building)actor.CurJob.targetA.Thing;
                Building_TurretGun gun = thing as Building_TurretGun;
                if (!JobDriver_ManTurret.GunNeedsRefueling(thing))
                {
                    this.JumpToToil(gotoTurret);
                }
                else
                {
                    Thing fuelForTurret = JobDriver_ManTurret.FindFuelForTurret(this.pawn, gun);
                    if (fuelForTurret == null)
                    {
                        CompRefuelable comp = thing.TryGetComp<CompRefuelable>();
                        if (actor.Faction == Faction.OfPlayer && comp != null)
                            Messages.Message((string)"MessageOutOfNearbyFuelFor".Translate((NamedArgument)actor.LabelShort, (NamedArgument)gun.Label, actor.Named("PAWN"), gun.Named("GUN"), comp.Props.fuelFilter.Summary.Named("FUEL")).CapitalizeFirst(), (LookTargets)(Thing)gun, MessageTypeDefOf.NegativeEvent);
                        actor.jobs.EndCurrentJob(JobCondition.Incompletable);
                    }
                    actor.CurJob.targetB = (LocalTargetInfo)fuelForTurret;
                    actor.CurJob.count = 1;
                }
            });
            yield return refuelIfNeeded;
            yield return Toils_Reserve.Reserve(TargetIndex.B, 10, 1);
            yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.OnCell).FailOnSomeonePhysicallyInteracting<Toil>(TargetIndex.B);
            yield return Toils_Haul.StartCarryThing(TargetIndex.B);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            yield return Toils_General.Wait(240).FailOnDestroyedNullOrForbidden<Toil>(TargetIndex.B).FailOnDestroyedNullOrForbidden<Toil>(TargetIndex.A).FailOnCannotTouch<Toil>(TargetIndex.A, PathEndMode.Touch).WithProgressBarToilDelay(TargetIndex.A);
            yield return Toils_Refuel.FinalizeRefueling(TargetIndex.A, TargetIndex.B);
            Toil loadIfNeeded = ToilMaker.MakeToil(nameof(MakeNewToils));
            loadIfNeeded.initAction = (Action)(() =>
            {
                Pawn actor = loadIfNeeded.actor;
                Building thing = (Building)actor.CurJob.targetA.Thing;
                Building_TurretGun gun = thing as Building_TurretGun;
                if (!JobDriver_ManTurret.GunNeedsLoading(thing))
                {
                    this.JumpToToil(gotoTurret);
                }
                else
                {
                    Thing ammoForTurret = JobDriver_ManTurret.FindAmmoForTurret(this.pawn, gun);
                    if (ammoForTurret == null)
                    {
                        if (actor.Faction == Faction.OfPlayer)
                            Messages.Message((string)"MessageOutOfNearbyShellsFor".Translate((NamedArgument)actor.LabelShort, (NamedArgument)gun.Label, actor.Named("PAWN"), gun.Named("GUN")).CapitalizeFirst(), (LookTargets)(Thing)gun, MessageTypeDefOf.NegativeEvent);
                        actor.jobs.EndCurrentJob(JobCondition.Incompletable);
                    }
                    actor.CurJob.targetB = (LocalTargetInfo)ammoForTurret;
                    actor.CurJob.count = 1;
                }
            });
            yield return loadIfNeeded;
            yield return Toils_Reserve.Reserve(TargetIndex.B, 10, 1);
            yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.OnCell).FailOnSomeonePhysicallyInteracting<Toil>(TargetIndex.B);
            yield return Toils_Haul.StartCarryThing(TargetIndex.B);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            Toil loadShell = ToilMaker.MakeToil(nameof(MakeNewToils));
            loadShell.initAction = (Action)(() =>
            {
                Pawn actor = loadShell.actor;
                Building_TurretGun thing = (Building)actor.CurJob.targetA.Thing as Building_TurretGun;
                SoundDefOf.Artillery_ShellLoaded.PlayOneShot((SoundInfo)new TargetInfo(thing.Position, thing.Map));
                thing.TryGetComp<CompChangeableProjectile>().LoadShell(actor.CurJob.targetB.Thing.def, 1);
                actor.carryTracker.innerContainer.ClearAndDestroyContents();
            });
            yield return loadShell;
            yield return gotoTurret;
            Toil man = ToilMaker.MakeToil(nameof(MakeNewToils));
            man.tickAction = (Action)(() =>
            {
                Pawn actor = man.actor;
                Building thing = (Building)actor.CurJob.targetA.Thing;
                if (JobDriver_ManTurret.GunNeedsLoading(thing))
                    this.JumpToToil(loadIfNeeded);
                else if (JobDriver_ManTurret.GunNeedsRefueling(thing))
                {
                    this.JumpToToil(refuelIfNeeded);
                }
                else
                {
                    thing.GetComp<CompMannable>().ManForATick(actor);
                    man.actor.rotationTracker.FaceCell(thing.Position);
                }
            });
            man.handlingFacing = true;
            man.defaultCompleteMode = ToilCompleteMode.Never;
            man.FailOnCannotTouch<Toil>(TargetIndex.A, PathEndMode.InteractionCell);
            yield return man;
        }
    }
}
