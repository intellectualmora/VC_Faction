﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;
using RimWorld;
using Verse.Noise;
namespace VC_Faction
{

    [StaticConstructorOnStartup]
    public class Building_TurretGun : Building_Turret,IVerbOwner
    {

        protected int burstCooldownTicksLeft;

        protected int burstWarmupTicksLeft;

        protected LocalTargetInfo currentTargetInt = LocalTargetInfo.Invalid;

        private bool holdFire;

        private bool burstActivated;

        protected CompPowerTrader powerComp;

        protected CompCanBeDormant dormantComp;

        protected CompInitiatable initiatableComp;

        protected CompMannable mannableComp;

        protected CompInteractable interactableComp;

        public CompRefuelable refuelableComp;

        protected Effecter progressBarEffecter;

        protected CompMechPowerCell powerCellComp;

        private const int TryStartShootSomethingIntervalTicks = 10;

        public static Material ForcedTargetLineMat = MaterialPool.MatFrom(GenDraw.LineTexPath, ShaderDatabase.Transparent, new Color(1f, 0.5f, 0.5f));

        public bool Active
        {
            get
            {
                if ((powerComp == null || powerComp.PowerOn) && (dormantComp == null || dormantComp.Awake) && (initiatableComp == null || initiatableComp.Initiated) && (interactableComp == null || burstActivated))
                {
                    if (powerCellComp != null)
                    {
                        return !powerCellComp.depleted;
                    }

                    return true;
                }

                return false;
            }
        }

        private VerbTracker verbTracker;
        public override LocalTargetInfo CurrentTarget => currentTargetInt;

        private bool WarmingUp => burstWarmupTicksLeft > 0;

        public override Verb AttackVerb => verbTracker.PrimaryVerb;
        public bool IsMannable => mannableComp != null;

        private bool PlayerControlled
        {
            get
            {
                if ((base.Faction == Faction.OfPlayer || MannedByColonist) && !MannedByNonColonist)
                {
                    return !IsActivable;
                }

                return false;
            }
        }

        protected virtual bool CanSetForcedTarget
        {
            get
            {
                if (mannableComp != null)
                {
                    return PlayerControlled;
                }

                return false;
            }
        }

        private bool CanToggleHoldFire => PlayerControlled;

        private bool IsMortar => def.building.IsMortar;

        private bool IsMortarOrProjectileFliesOverhead
        {
            get
            {
                if (!AttackVerb.ProjectileFliesOverhead())
                {
                    return IsMortar;
                }

                return true;
            }
        }

        private bool IsActivable => interactableComp != null;

        protected virtual bool HideForceTargetGizmo => false;


        private bool CanExtractShell
        {
            get
            {
                if (!PlayerControlled)
                {
                    return false;
                }

                return this.TryGetComp<CompChangeableProjectile>()?.Loaded ?? false;
            }
        }

        private bool MannedByColonist
        {
            get
            {
                if (mannableComp != null && mannableComp.ManningPawn != null)
                {
                    return mannableComp.ManningPawn.Faction == Faction.OfPlayer;
                }

                return false;
            }
        }

        private bool MannedByNonColonist
        {
            get
            {
                if (mannableComp != null && mannableComp.ManningPawn != null)
                {
                    return mannableComp.ManningPawn.Faction != Faction.OfPlayer;
                }

                return false;
            }
        }

        public VerbTracker VerbTracker => verbTracker;

        // 读取 XML 中 <verbs> 列表
        public List<VerbProperties> VerbProperties => def.Verbs;

        // 建筑不是近战单位，返回 null 即可
        public List<Tool> Tools => null;

        // 通常只用于近战计算，建筑可以用 ThingDefOf.HumanMadeWeapons 兜底或返回 null
        public ImplementOwnerTypeDef ImplementOwnerTypeDef => ImplementOwnerTypeDefOf.NativeVerb;

        // 返回这个物体自身作为施法者
        public Thing ConstantCaster => this;

        public Building_TurretGun()
        {
        }

        public override void PostMake()
        {
            base.PostMake();
            burstCooldownTicksLeft = def.building.turretInitialCooldownTime.SecondsToTicks();
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            this.verbTracker = new VerbTracker(this);
            foreach (VerbProperties verbProps in def.Verbs)
            {
                Verb verb = (Verb)Activator.CreateInstance(verbProps.verbClass);
                verb.verbProps = verbProps;
                verb.caster = this;
                verb.targetParams.canTargetLocations = true;
                verb.targetParams.canTargetBuildings = true;
                verb.targetParams.canTargetPawns = false;
                verb.castCompleteCallback = BurstComplete; // 如果你想保留攻击完成逻辑
                verbTracker.AllVerbs.Add(verb);
            }
            dormantComp = GetComp<CompCanBeDormant>();
            initiatableComp = GetComp<CompInitiatable>();
            powerComp = GetComp<CompPowerTrader>();
            mannableComp = GetComp<CompMannable>();
            interactableComp = GetComp<CompInteractable>();
            refuelableComp = GetComp<CompRefuelable>();
            powerCellComp = GetComp<CompMechPowerCell>();

        }

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            base.DeSpawn(mode);
            ResetCurrentTarget();
            progressBarEffecter?.Cleanup();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref burstCooldownTicksLeft, "burstCooldownTicksLeft", 0);
            Scribe_Values.Look(ref burstWarmupTicksLeft, "burstWarmupTicksLeft", 0);
            Scribe_TargetInfo.Look(ref currentTargetInt, "currentTarget");
            Scribe_Values.Look(ref holdFire, "holdFire", defaultValue: false);
            Scribe_Values.Look(ref burstActivated, "burstActivated", defaultValue: false);
            BackCompatibility.PostExposeData(this);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
     
                    UpdateGunVerbs();
               
            }
        }

        public override AcceptanceReport ClaimableBy(Faction by)
        {
            if (!base.ClaimableBy(by))
            {
                return false;
            }

            if (mannableComp != null && mannableComp.ManningPawn != null)
            {
                return false;
            }

            if (Active && mannableComp == null)
            {
                return false;
            }

            if (((dormantComp != null && !dormantComp.Awake) || (initiatableComp != null && !initiatableComp.Initiated)) && (powerComp == null || powerComp.PowerOn))
            {
                return false;
            }

            return true;
        }

        public override void OrderAttack(LocalTargetInfo targ)
        {
            if (!targ.IsValid)
            {
                if (forcedTarget.IsValid)
                {
                    ResetForcedTarget();
                }

                return;
            }

            if ((targ.Cell - base.Position).LengthHorizontal < AttackVerb.verbProps.EffectiveMinRange(targ, this))
            {
                Messages.Message("MessageTargetBelowMinimumRange".Translate(), this, MessageTypeDefOf.RejectInput, historical: false);
                return;
            }

            if ((targ.Cell - base.Position).LengthHorizontal > AttackVerb.verbProps.range)
            {
                Messages.Message("MessageTargetBeyondMaximumRange".Translate(), this, MessageTypeDefOf.RejectInput, historical: false);
                return;
            }

            if (forcedTarget != targ)
            {
                forcedTarget = targ;
                if (burstCooldownTicksLeft <= 0)
                {
                    TryStartShootSomething(canBeginBurstImmediately: false);
                }
            }

            if (holdFire)
            {
                Messages.Message("MessageTurretWontFireBecauseHoldFire".Translate(def.label), this, MessageTypeDefOf.RejectInput, historical: false);
            }
        }

        protected override void Tick()
        {
            base.Tick();
            if (CanExtractShell && MannedByColonist)
            {
                CompChangeableProjectile compChangeableProjectile = this.TryGetComp<CompChangeableProjectile>();
                if (!compChangeableProjectile.allowedShellsSettings.AllowedToAccept(compChangeableProjectile.LoadedShell))
                {
                    ExtractShell();
                }
            }

            if (forcedTarget.IsValid && !CanSetForcedTarget)
            {
                ResetForcedTarget();
            }

            if (!CanToggleHoldFire)
            {
                holdFire = false;
            }

            if (forcedTarget.ThingDestroyed)
            {
                ResetForcedTarget();
            }

            if (Active && (mannableComp == null || mannableComp.MannedNow) && !base.IsStunned && base.Spawned)
            {
                verbTracker.VerbsTick();
                if (AttackVerb.state == VerbState.Bursting)
                {
                    return;
                }

                burstActivated = false;
                if (WarmingUp)
                {
                    burstWarmupTicksLeft--;
                    if (burstWarmupTicksLeft == 0)
                    {
                        BeginBurst();
                    }
                }
                else
                {
                    if (burstCooldownTicksLeft > 0)
                    {
                        burstCooldownTicksLeft--;
                        if (IsMortar)
                        {
                            if (progressBarEffecter == null)
                            {
                                progressBarEffecter = EffecterDefOf.ProgressBar.Spawn();
                            }

                            progressBarEffecter.EffectTick(this, TargetInfo.Invalid);
                            MoteProgressBar mote = ((SubEffecter_ProgressBar)progressBarEffecter.children[0]).mote;
                            mote.progress = 1f - (float)Math.Max(burstCooldownTicksLeft, 0) / (float)BurstCooldownTime().SecondsToTicks();
                            mote.offsetZ = -0.8f;
                        }
                    }

                    if (burstCooldownTicksLeft <= 0 && this.IsHashIntervalTick(10))
                    {
                        TryStartShootSomething(canBeginBurstImmediately: true);
                    }
                }

            }
            else
            {
                ResetCurrentTarget();
            }
        }

        public void TryActivateBurst()
        {
            burstActivated = true;
            TryStartShootSomething(canBeginBurstImmediately: true);
        }
        public bool Available()
        {
            string reason = "";
            if (AttackVerb.CasterIsPawn && AttackVerb.EquipmentSource != null && EquipmentUtility.RolePreventsFromUsing(AttackVerb.CasterPawn, AttackVerb.EquipmentSource, out reason))
            {
                return false;
            }
            return true;
        }
        public void TryStartShootSomething(bool canBeginBurstImmediately)
        {
            if (progressBarEffecter != null)
            {
                progressBarEffecter.Cleanup();
                progressBarEffecter = null;
            }
         
            if (!base.Spawned || (holdFire && CanToggleHoldFire) || (AttackVerb.ProjectileFliesOverhead() && base.Map.roofGrid.Roofed(base.Position)) || !Available())
            {
                ResetCurrentTarget();
                return;
            }

            bool isValid = currentTargetInt.IsValid;
            if (forcedTarget.IsValid)
            {
                currentTargetInt = forcedTarget;
            }
            else
            {
                currentTargetInt = TryFindNewTarget();
            }

            if (!isValid && currentTargetInt.IsValid && def.building.playTargetAcquiredSound)
            {
                SoundDefOf.TurretAcquireTarget.PlayOneShot(new TargetInfo(base.Position, base.Map));
            }

            if (currentTargetInt.IsValid)
            {
                float randomInRange = def.building.turretBurstWarmupTime.RandomInRange;
                if (randomInRange > 0f)
                {
                    burstWarmupTicksLeft = randomInRange.SecondsToTicks();
                }
                else if (canBeginBurstImmediately)
                {
                    BeginBurst();
                }
                else
                {
                    burstWarmupTicksLeft = 1;
                }
            }
            else
            {
                ResetCurrentTarget();
            }
        }
        private bool IsInFiringArc(IntVec3 targetPos)
        {
            Vector3 forward = this.Rotation.FacingCell.ToVector3();
            Vector3 toTarget = (targetPos.ToVector3Shifted() - this.Position.ToVector3Shifted()).normalized;
            float angle = Vector3.Angle(forward, toTarget);
            return angle <= 45f; // ±45度范围
        }
        public virtual LocalTargetInfo TryFindNewTarget()
        {
            IAttackTargetSearcher attackTargetSearcher = TargSearcher();
            Faction faction = attackTargetSearcher.Thing.Faction;
            float range = AttackVerb.verbProps.range;
            if (Rand.Value < 0.5f && AttackVerb.ProjectileFliesOverhead() && faction.HostileTo(Faction.OfPlayer) && base.Map.listerBuildings.allBuildingsColonist.Where(delegate (Building x)
            {
                float num = AttackVerb.verbProps.EffectiveMinRange(x, this);
                float num2 = x.Position.DistanceToSquared(base.Position);
                return num2 > num * num && num2 < range * range && IsInFiringArc(x.Position);
            }).TryRandomElement(out var result))
            {
                return result;
            }

            TargetScanFlags targetScanFlags = TargetScanFlags.NeedThreat | TargetScanFlags.NeedAutoTargetable;
            if (!AttackVerb.ProjectileFliesOverhead())
            {
                targetScanFlags |= TargetScanFlags.NeedLOSToAll;
                targetScanFlags |= TargetScanFlags.LOSBlockableByGas;
            }

            if (AttackVerb.IsIncendiary_Ranged())
            {
                targetScanFlags |= TargetScanFlags.NeedNonBurning;
            }

            if (IsMortar)
            {
                targetScanFlags |= TargetScanFlags.NeedNotUnderThickRoof;
            }

            return (Thing)AttackTargetFinder.BestShootTargetFromCurrentPosition(attackTargetSearcher, targetScanFlags, IsValidTarget);
        }

        private IAttackTargetSearcher TargSearcher()
        {
            if (mannableComp != null && mannableComp.MannedNow)
            {
                return mannableComp.ManningPawn;
            }

            return this;
        }

        private bool IsValidTarget(Thing t)
        {
            if (t is Pawn pawn)
            {
                if (base.Faction == Faction.OfPlayer && pawn.IsPrisoner)
                {
                    return false;
                }

                if (AttackVerb.ProjectileFliesOverhead())
                {
                    RoofDef roofDef = base.Map.roofGrid.RoofAt(t.Position);
                    if (roofDef != null && roofDef.isThickRoof)
                    {
                        return false;
                    }
                }

                if (mannableComp == null)
                {
                    return !GenAI.MachinesLike(base.Faction, pawn);
                }

                if (pawn.RaceProps.Animal && pawn.Faction == Faction.OfPlayer)
                {
                    return false;
                }
            }

            return true;
        }

        protected virtual void BeginBurst()
        {
            bool iscast = AttackVerb.TryStartCastOn(CurrentTarget);
            OnAttackedTarget(CurrentTarget);

        }

        protected void BurstComplete()
        {
            burstCooldownTicksLeft = BurstCooldownTime().SecondsToTicks();
        }

        protected float BurstCooldownTime()
        {
            if (def.building.turretBurstCooldownTime >= 0f)
            {
                return def.building.turretBurstCooldownTime;
            }

            return AttackVerb.verbProps.defaultCooldownTime;
        }

        public override string GetInspectString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            string inspectString = base.GetInspectString();
            if (!inspectString.NullOrEmpty())
            {
                stringBuilder.AppendLine(inspectString);
            }
            if (AttackVerb != null)
            {
                if (AttackVerb?.verbProps?.minRange > 0f)
                {
                    stringBuilder.AppendLine("MinimumRange".Translate() + ": " + AttackVerb?.verbProps?.minRange.ToString("F0"));
                }
            }

            if (base.Spawned && IsMortarOrProjectileFliesOverhead && base.Position.Roofed(base.Map))
            {
                stringBuilder.AppendLine("CannotFire".Translate() + ": " + "Roofed".Translate().CapitalizeFirst());
            }
            else if (base.Spawned && burstCooldownTicksLeft > 0 && BurstCooldownTime() > 5f)
            {
                stringBuilder.AppendLine("CanFireIn".Translate() + ": " + burstCooldownTicksLeft.ToStringSecondsFromTicks());
            }

            CompChangeableProjectile compChangeableProjectile = this.TryGetComp<CompChangeableProjectile>();
            if (compChangeableProjectile != null)
            {
                if (compChangeableProjectile.Loaded)
                {
                    stringBuilder.AppendLine("ShellLoaded".Translate(compChangeableProjectile.LoadedShell.LabelCap, compChangeableProjectile.LoadedShell));
                }
                else
                {
                    stringBuilder.AppendLine("ShellNotLoaded".Translate());
                }
            }

            return stringBuilder.ToString().TrimEndNewlines();
        }

        public override void DrawExtraSelectionOverlays()
        {
            base.DrawExtraSelectionOverlays();

            float range = AttackVerb.verbProps.range;
            float angleDeg = 90f; // 扇形角度，例如 90°

            // 获取起点和方向
            Vector3 origin = this.TrueCenter();
            Vector3 forward = this.Rotation.FacingCell.ToVector3();

            // 绘制扇形范围

            // 可选：绘制最小有效射程（可以留圆圈）
            float minRange = AttackVerb.verbProps.EffectiveMinRange(allowAdjacentShot: true);
            if (minRange > 0.1f && minRange < 90f)
            {
                DrawConeRing(base.Position, minRange, range, angleDeg, forward, Color.white, this.Map);

            }

            // 绘制预热扇形饼图（可保留）
            if (WarmingUp)
            {
                int degreesWide = (int)((float)burstWarmupTicksLeft * 0.5f);
                GenDraw.DrawAimPie(this, CurrentTarget, degreesWide, (float)def.size.x * 0.5f);
            }

            // 绘制强制目标线（保留）
            if (forcedTarget.IsValid && (!forcedTarget.HasThing || forcedTarget.Thing.Spawned))
            {
                Vector3 b = (!forcedTarget.HasThing)
                    ? forcedTarget.Cell.ToVector3Shifted()
                    : forcedTarget.Thing.TrueCenter();
                Vector3 a = this.TrueCenter();
                b.y = AltitudeLayer.MetaOverlays.AltitudeFor();
                a.y = b.y;
                GenDraw.DrawLineBetween(a, b, ForcedTargetLineMat);
            }
        }
        public static void DrawConeRing(IntVec3 center, float minRadius, float radius, float angleDeg, Vector3 forward, Color color, Map map, Func<IntVec3, bool> predicate = null)
        {
            if (radius > GenRadial.MaxRadialPatternRadius)
            {
                return;
            }

            List<IntVec3> ringDrawCells = new List<IntVec3>();
            int num = GenRadial.NumCellsInRadius(radius);
            int num2 = GenRadial.NumCellsInRadius(minRadius);
            float halfAngle = angleDeg / 2f;
            Vector2 forward2D = new Vector2(forward.x, forward.z).normalized;

            for (int i = 0; i < num; i++)
            {
                IntVec3 intVec = center + GenRadial.RadialPattern[i];

                if (!intVec.InBounds(map))
                    continue;

                Vector3 offset = (intVec - center).ToVector3();
                Vector2 offset2D = new Vector2(offset.x, offset.z);

                if (offset2D.magnitude > radius || offset2D.magnitude < 0.1f|| offset2D.magnitude < minRadius)
                    continue;

                float angle = Vector2.Angle(forward2D, offset2D.normalized);
                if (angle <= halfAngle)
                {
                    if (predicate == null || predicate(intVec))
                    {
                        ringDrawCells.Add(intVec);
                    }
                }
            }

            GenDraw.DrawFieldEdges(ringDrawCells, color);
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo gizmo in base.GetGizmos())
            {
                yield return gizmo;
            }

            if (CanExtractShell)
            {
                CompChangeableProjectile compChangeableProjectile = this.TryGetComp<CompChangeableProjectile>();
                Command_Action command_Action = new Command_Action();
                command_Action.defaultLabel = "CommandExtractShell".Translate();
                command_Action.defaultDesc = "CommandExtractShellDesc".Translate();
                command_Action.icon = compChangeableProjectile.LoadedShell.uiIcon;
                command_Action.iconAngle = compChangeableProjectile.LoadedShell.uiIconAngle;
                command_Action.iconOffset = compChangeableProjectile.LoadedShell.uiIconOffset;
                command_Action.iconDrawScale = GenUI.IconDrawScale(compChangeableProjectile.LoadedShell);
                command_Action.action = delegate
                {
                    ExtractShell();
                };
                yield return command_Action;
            }

            CompChangeableProjectile compChangeableProjectile2 = this.TryGetComp<CompChangeableProjectile>();
            if (compChangeableProjectile2 != null)
            {
                StorageSettings storeSettings = compChangeableProjectile2.GetStoreSettings();
                foreach (Gizmo item in StorageSettingsClipboard.CopyPasteGizmosFor(storeSettings))
                {
                    yield return item;
                }
            }

            if (!HideForceTargetGizmo)
            {
                if (CanSetForcedTarget)
                {
                    Command_VerbTarget command_VerbTarget = new Command_VerbTarget();
                    command_VerbTarget.defaultLabel = "CommandSetForceAttackTarget".Translate();
                    command_VerbTarget.defaultDesc = "CommandSetForceAttackTargetDesc".Translate();
                    command_VerbTarget.icon = ContentFinder<Texture2D>.Get("UI/Commands/Attack");
                    command_VerbTarget.verb = AttackVerb;
                    command_VerbTarget.hotKey = KeyBindingDefOf.Misc4;
                    command_VerbTarget.drawRadius = false;
                    command_VerbTarget.requiresAvailableVerb = false;
                    if (base.Spawned && IsMortarOrProjectileFliesOverhead && base.Position.Roofed(base.Map))
                    {
                        command_VerbTarget.Disable("CannotFire".Translate() + ": " + "Roofed".Translate().CapitalizeFirst());
                    }

                    yield return command_VerbTarget;
                }

                if (forcedTarget.IsValid)
                {
                    Command_Action command_Action2 = new Command_Action();
                    command_Action2.defaultLabel = "CommandStopForceAttack".Translate();
                    command_Action2.defaultDesc = "CommandStopForceAttackDesc".Translate();
                    command_Action2.icon = ContentFinder<Texture2D>.Get("UI/Commands/Halt");
                    command_Action2.action = delegate
                    {
                        ResetForcedTarget();
                        SoundDefOf.Tick_Low.PlayOneShotOnCamera();
                    };
                    if (!forcedTarget.IsValid)
                    {
                        command_Action2.Disable("CommandStopAttackFailNotForceAttacking".Translate());
                    }

                    command_Action2.hotKey = KeyBindingDefOf.Misc5;
                    yield return command_Action2;
                }
            }

            if (!CanToggleHoldFire)
            {
                yield break;
            }

            Command_Toggle command_Toggle = new Command_Toggle();
            command_Toggle.defaultLabel = "CommandHoldFire".Translate();
            command_Toggle.defaultDesc = "CommandHoldFireDesc".Translate();
            command_Toggle.icon = ContentFinder<Texture2D>.Get("UI/Commands/HoldFire");
            command_Toggle.hotKey = KeyBindingDefOf.Misc6;
            command_Toggle.toggleAction = delegate
            {
                holdFire = !holdFire;
                if (holdFire)
                {
                    ResetForcedTarget();
                }
            };
            command_Toggle.isActive = () => holdFire;
            yield return command_Toggle;
        }

        private void ExtractShell()
        {
            GenPlace.TryPlaceThing(this.TryGetComp<CompChangeableProjectile>().RemoveShell(), base.Position, base.Map, ThingPlaceMode.Near);
        }

        private void ResetForcedTarget()
        {
            forcedTarget = LocalTargetInfo.Invalid;
            burstWarmupTicksLeft = 0;
            if (burstCooldownTicksLeft <= 0)
            {
                TryStartShootSomething(canBeginBurstImmediately: false);
            }
        }

        private void ResetCurrentTarget()
        {
            currentTargetInt = LocalTargetInfo.Invalid;
            burstWarmupTicksLeft = 0;
        }


        private void UpdateGunVerbs()
        {
            List<Verb> allVerbs = verbTracker.AllVerbs;
            for (int i = 0; i < allVerbs.Count; i++)
            {
                Verb verb = allVerbs[i];
                verb.caster = this;
                verb.castCompleteCallback = BurstComplete;
            }
        }

        public string UniqueVerbOwnerID()
        {
            // 用建筑的 ThingIDNumber 来确保唯一性
            return $"TurretVerbOwner_{ThingID}";
        }
        public bool VerbsStillUsableBy(Pawn p)
        {
            // 如果建筑是可操作的炮塔（带 Mannable），返回 true 表示 pawn 可使用它的 verb
            return true;
        }
    }
}