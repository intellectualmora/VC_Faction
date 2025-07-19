// Decompiled with JetBrains decompiler
// Type: Verse.Verb_LaunchProjectile
// Assembly: Assembly-CSharp, Version=1.5.9214.33606, Culture=neutral, PublicKeyToken=null
// MVID: 630E2863-BC9A-4A34-93F2-EFF01E3A9556
// Assembly location: E:\SteamLibrary\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\Assembly-CSharp.dll

using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace VC_Faction
{
    public class Verb_LaunchProjectile : Verse.Verb
    {
        public override void DrawHighlight(LocalTargetInfo target)
        {
            if (target.IsValid)
            {
                GenDraw.DrawTargetHighlight(target);
                DrawHighlightFieldRadiusAroundTarget(target);
            }
        }
        private List<IntVec3> forcedMissTargetEvenDispersalCache = new List<IntVec3>();
        private bool IsInFiringArc(IntVec3 root, IntVec3 targetCell)
        {
            // 将 IntVec3 转换为带小数的世界坐标
            Vector3 rootVec = root.ToVector3Shifted();
            Vector3 targetVec = targetCell.ToVector3Shifted();

            // 计算 root → target 的单位向量
            Vector3 toTarget = (targetVec - rootVec).normalized;

            // 获取朝向向量
            Vector3 forward = caster.Rotation.FacingCell.ToVector3().normalized;

            // 计算夹角
            float angle = Vector3.Angle(forward, toTarget);

            // 限制 ±45° 扇形范围（即90°总角度）
            return angle <= 45f;
        }
        public override bool CanHitTargetFrom(IntVec3 root, LocalTargetInfo targ)
        {
            if (targ.Thing != null && targ.Thing == caster)
            {
                return targetParams.canTargetSelf;
            }

            if (targ.Pawn != null && targ.Pawn.IsPsychologicallyInvisible() && caster.HostileTo(targ.Pawn))
            {
                return false;
            }

            if (ApparelPreventsShooting())
            {
                return false;
            }

            ShootLine resultingLine;
            if (!IsInFiringArc(root, targ.Cell))
            {
                return false;
            }
            return TryFindShootLineFromTo(root, targ, out resultingLine);
        }
        public virtual ThingDef Projectile
        {
            get
            {
                CompChangeableProjectile comp = (this.DirectOwner as ThingWithComps)?.GetComp<CompChangeableProjectile>();
                return comp != null && comp.Loaded ? comp.Projectile : this.verbProps.defaultProjectile;
            }
        }

        public override void WarmupComplete()
        {
            base.WarmupComplete();
            Find.BattleLog.Add((LogEntry)new BattleLogEntry_RangedFire(this.caster, this.currentTarget.HasThing ? this.currentTarget.Thing : (Thing)null, this.EquipmentSource?.def, this.Projectile, this.ShotsPerBurst > 1));
        }

        protected IntVec3 GetForcedMissTarget(float forcedMissRadius)
        {
            if (this.verbProps.forcedMissEvenDispersal)
            {
                if (this.forcedMissTargetEvenDispersalCache.Count <= 0)
                {
                    this.forcedMissTargetEvenDispersalCache.AddRange(Verb_LaunchProjectile.GenerateEvenDispersalForcedMissTargets(this.currentTarget.Cell, forcedMissRadius, this.burstShotsLeft));
                    this.forcedMissTargetEvenDispersalCache.SortByDescending<IntVec3, int>((Func<IntVec3, int>)(p => p.DistanceToSquared(this.Caster.Position)));
                }
                if (this.forcedMissTargetEvenDispersalCache.Count > 0)
                    return this.forcedMissTargetEvenDispersalCache.Pop<IntVec3>();
            }
            int index = Rand.Range(0, GenRadial.NumCellsInRadius(forcedMissRadius));
            return this.currentTarget.Cell + GenRadial.RadialPattern[index];
        }

        private static IEnumerable<IntVec3> GenerateEvenDispersalForcedMissTargets(
          IntVec3 root,
          float radius,
          int count)
        {
            float randomRotationOffset = Rand.Range(0.0f, 360f);
            float goldenRatio = (float)((1.0 + (double)Mathf.Pow(5f, 0.5f)) / 2.0);
            for (int i = 0; i < count; ++i)
            {
                double f1 = 6.2831854820251465 * (double)i / (double)goldenRatio;
                float f2 = Mathf.Acos((float)(1.0 - 2.0 * ((double)i + 0.5) / (double)count));
                yield return root + new Vector3((float)(int)((double)Mathf.Cos((float)f1) * (double)Mathf.Sin(f2) * (double)radius), 0.0f, (float)(int)((double)Mathf.Cos(f2) * (double)radius)).RotatedBy(randomRotationOffset).ToIntVec3();
            }
        }

        protected override bool TryCastShot()
        {
            if (this.currentTarget.HasThing && this.currentTarget.Thing.Map != this.caster.Map)
                return false;
            ThingDef projectile1 = this.Projectile;
            if (projectile1 == null)
                return false;
            ShootLine resultingLine;
            bool shootLineFromTo = this.TryFindShootLineFromTo(this.caster.Position, this.currentTarget, out resultingLine);
            if (this.verbProps.stopBurstWithoutLos && !shootLineFromTo)
                return false;
            if ((this.DirectOwner as ThingWithComps) != null)
            {
                (this.DirectOwner as ThingWithComps).GetComp<CompChangeableProjectile>()?.Notify_ProjectileLaunched();
                (this.DirectOwner as ThingWithComps).GetComp<CompApparelVerbOwner_Charged>()?.UsedOnce();
            }
            this.lastShotTick = Find.TickManager.TicksGame;
            Thing launcher = this.caster;
            Thing equipment = (Thing)this.EquipmentSource;
            CompMannable comp = this.caster.TryGetComp<CompMannable>();
            if (comp?.ManningPawn != null)
            {
                launcher = (Thing)comp.ManningPawn;
                equipment = this.caster;
            }
            Vector3 drawPos = this.caster.DrawPos;
            Verse.Projectile projectile2 = (Verse.Projectile)GenSpawn.Spawn(projectile1, resultingLine.Source, this.caster.Map);
            if ((double)this.verbProps.ForcedMissRadius > 0.5)
            {
                float forcedMissRadius = this.verbProps.ForcedMissRadius;
                if (launcher is Pawn caster)
                    forcedMissRadius *= this.verbProps.GetForceMissFactorFor(equipment, caster);
                float adjustedForcedMiss = VerbUtility.CalculateAdjustedForcedMiss(forcedMissRadius, this.currentTarget.Cell - this.caster.Position);
                if ((double)adjustedForcedMiss > 0.5)
                {
                    IntVec3 forcedMissTarget = this.GetForcedMissTarget(adjustedForcedMiss);
                    if (forcedMissTarget != this.currentTarget.Cell)
                    {
                        this.ThrowDebugText("ToRadius");
                        this.ThrowDebugText("Rad\nDest", forcedMissTarget);
                        ProjectileHitFlags hitFlags = ProjectileHitFlags.NonTargetWorld;
                        if (Rand.Chance(0.5f))
                            hitFlags = ProjectileHitFlags.All;
                        if (!this.canHitNonTargetPawnsNow)
                            hitFlags &= ~ProjectileHitFlags.NonTargetPawns;
                        projectile2.Launch(launcher, drawPos, (LocalTargetInfo)forcedMissTarget, this.currentTarget, hitFlags, this.preventFriendlyFire, equipment);
                        return true;
                    }
                }
            }
            ShotReport shotReport = ShotReport.HitReportFor(this.caster, (Verb)this, this.currentTarget);
            Thing randomCoverToMissInto = shotReport.GetRandomCoverToMissInto();
            ThingDef def = randomCoverToMissInto?.def;
            if (this.verbProps.canGoWild && !Rand.Chance(shotReport.AimOnTargetChance_IgnoringPosture))
            {
                bool flyOverhead = projectile2?.def?.projectile != null && projectile2.def.projectile.flyOverhead;
                resultingLine.ChangeDestToMissWild(shotReport.AimOnTargetChance_StandardTarget, flyOverhead, this.caster.Map);
                this.ThrowDebugText("ToWild" + (this.canHitNonTargetPawnsNow ? "\nchntp" : ""));
                this.ThrowDebugText("Wild\nDest", resultingLine.Dest);
                ProjectileHitFlags hitFlags = ProjectileHitFlags.NonTargetWorld;
                if (Rand.Chance(0.5f) && this.canHitNonTargetPawnsNow)
                    hitFlags |= ProjectileHitFlags.NonTargetPawns;
                projectile2.Launch(launcher, drawPos, (LocalTargetInfo)resultingLine.Dest, this.currentTarget, hitFlags, this.preventFriendlyFire, equipment, def);
                return true;
            }
            if (this.currentTarget.Thing != null && this.currentTarget.Thing.def.CanBenefitFromCover && !Rand.Chance(shotReport.PassCoverChance))
            {
                this.ThrowDebugText("ToCover" + (this.canHitNonTargetPawnsNow ? "\nchntp" : ""));
                this.ThrowDebugText("Cover\nDest", randomCoverToMissInto.Position);
                ProjectileHitFlags hitFlags = ProjectileHitFlags.NonTargetWorld;
                if (this.canHitNonTargetPawnsNow)
                    hitFlags |= ProjectileHitFlags.NonTargetPawns;
                projectile2.Launch(launcher, drawPos, (LocalTargetInfo)randomCoverToMissInto, this.currentTarget, hitFlags, this.preventFriendlyFire, equipment, def);
                return true;
            }
            ProjectileHitFlags hitFlags1 = ProjectileHitFlags.IntendedTarget;
            if (this.canHitNonTargetPawnsNow)
                hitFlags1 |= ProjectileHitFlags.NonTargetPawns;
            if (!this.currentTarget.HasThing || this.currentTarget.Thing.def.Fillage == FillCategory.Full)
                hitFlags1 |= ProjectileHitFlags.NonTargetWorld;
            this.ThrowDebugText("ToHit" + (this.canHitNonTargetPawnsNow ? "\nchntp" : ""));
            if (this.currentTarget.Thing != null)
            {
                projectile2.Launch(launcher, drawPos, this.currentTarget, this.currentTarget, hitFlags1, this.preventFriendlyFire, equipment, def);
                this.ThrowDebugText("Hit\nDest", this.currentTarget.Cell);
            }
            else
            {
                projectile2.Launch(launcher, drawPos, (LocalTargetInfo)resultingLine.Dest, this.currentTarget, hitFlags1, this.preventFriendlyFire, equipment, def);
                this.ThrowDebugText("Hit\nDest", resultingLine.Dest);
            }
            return true;
        }

        private void ThrowDebugText(string text)
        {
            if (!DebugViewSettings.drawShooting)
                return;
            MoteMaker.ThrowText(this.caster.DrawPos, this.caster.Map, text);
        }

        private void ThrowDebugText(string text, IntVec3 c)
        {
            if (!DebugViewSettings.drawShooting)
                return;
            MoteMaker.ThrowText(c.ToVector3Shifted(), this.caster.Map, text);
        }

        public override float HighlightFieldRadiusAroundTarget(out bool needLOSToCenter)
        {
            needLOSToCenter = true;
            ThingDef projectile = this.Projectile;
            if (projectile == null)
                return 0.0f;
            float num = projectile.projectile.explosionRadius + projectile.projectile.explosionRadiusDisplayPadding;
            float forcedMissRadius = this.verbProps.ForcedMissRadius;
            if ((double)forcedMissRadius > 0.0 && this.verbProps.burstShotCount > 1)
                num += forcedMissRadius;
            return num;
        }

        public override bool Available()
        {
            if (!base.Available())
                return false;
            if (this.CasterIsPawn)
            {
                Pawn casterPawn = this.CasterPawn;
                if (casterPawn.Faction != Faction.OfPlayer && !this.verbProps.ai_ProjectileLaunchingIgnoresMeleeThreats && casterPawn.mindState.MeleeThreatStillThreat && casterPawn.mindState.meleeThreat.Position.AdjacentTo8WayOrInside(casterPawn.Position))
                    return false;
            }
            return this.Projectile != null;
        }

        public override void Reset()
        {
            base.Reset();
            this.forcedMissTargetEvenDispersalCache.Clear();
        }
    }
}
