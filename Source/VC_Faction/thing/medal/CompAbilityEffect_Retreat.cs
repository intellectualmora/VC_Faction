using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using UnityEngine;
using Verse.Sound;

namespace VC_Faction
{
    public class CompAbilityEffect_Retreat : CompAbilityEffect_WithFixedDest
    {
        public static string SkipUsedSignalTag = "CompAbilityEffect.SkipUsed";

        public new CompProperties_Retreat Props => (CompProperties_Retreat)props;

        public override IEnumerable<PreCastAction> GetPreCastActions()
        {
            yield return new PreCastAction
            {
                action = delegate (LocalTargetInfo t, LocalTargetInfo d)
                {
                    if (!parent.def.HasAreaOfEffect)
                    {
                        Pawn pawn = t.Pawn;
                        if (pawn != null)
                        {
                            FleckCreationData dataAttachedOverlay = FleckMaker.GetDataAttachedOverlay(pawn, RimWorld.FleckDefOf.PsycastSkipFlashEntry, new UnityEngine.Vector3(-0.5f, 0f, -0.5f));
                            dataAttachedOverlay.link.detachAfterTicks = 5;
                            pawn.Map.flecks.CreateFleck(dataAttachedOverlay);
                        }
                        else
                        {
                            FleckMaker.Static(t.CenterVector3, parent.pawn.Map, RimWorld.FleckDefOf.PsycastSkipFlashEntry);
                        }

                        FleckMaker.Static(d.Cell, parent.pawn.Map, RimWorld.FleckDefOf.PsycastSkipInnerExit);
                    }

                    if (Props.destination != AbilityEffectDestination.RandomInRange)
                    {
                        FleckMaker.Static(d.Cell, parent.pawn.Map, RimWorld.FleckDefOf.PsycastSkipOuterRingExit);
                    }
                    if (!parent.def.HasAreaOfEffect)
                    {
                        SoundDefOf.Psycast_Skip_Entry.PlayOneShot(new TargetInfo(t.Cell, parent.pawn.Map));
                        SoundDefOf.Psycast_Skip_Exit.PlayOneShot(new TargetInfo(d.Cell, parent.pawn.Map));
                    }
    
                },
                ticksAwayFromCast = 5
            };
        }

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            if (!target.HasThing)
            {
                return;
            }

            base.Apply(target, dest);
            LocalTargetInfo destination = GetDestination(dest.IsValid ? dest : target);
            if (!destination.IsValid)
            {
                return;
            }

            Pawn pawn = parent.pawn;
            if (!parent.def.HasAreaOfEffect)
            {
                parent.AddEffecterToMaintain(EffecterDefOf.Skip_Entry.Spawn(target.Thing, pawn.Map), target.Thing.Position, 60);
            }
            else
            {
                parent.AddEffecterToMaintain(EffecterDefOf.Skip_EntryNoDelay.Spawn(target.Thing, pawn.Map), target.Thing.Position, 60);
            }

            if (Props.destination == AbilityEffectDestination.Selected)
            {
                parent.AddEffecterToMaintain(EffecterDefOf.Skip_Exit.Spawn(destination.Cell, pawn.Map), destination.Cell, 60);
            }
            else
            {
                parent.AddEffecterToMaintain(EffecterDefOf.Skip_ExitNoDelay.Spawn(destination.Cell, pawn.Map), destination.Cell, 60);
            }

            target.Thing.TryGetComp<CompCanBeDormant>()?.WakeUp();
            target.Thing.Position = destination.Cell;
            Pawn pawn2;
            if ((pawn2 = (target.Thing as Pawn)) != null)
            {
                if ((pawn2.Faction == Faction.OfPlayer || pawn2.IsPlayerControlled) && pawn2.Position.Fogged(pawn2.Map))
                {
                    FloodFillerFog.FloodUnfog(pawn2.Position, pawn2.Map);
                }

                pawn2.stances.stunner.StunFor(Props.stunTicks.RandomInRange, parent.pawn, addBattleLog: false, showMote: false);
                pawn2.Notify_Teleported();
                SendSkipUsedSignal(pawn2.Position, pawn2);
            }

            if (Props.destClamorType != null)
            {
                GenClamor.DoClamor(pawn, target.Cell, Props.destClamorRadius, Props.destClamorType);
            }
        }

        public override bool CanHitTarget(LocalTargetInfo target)
        {
            if (!CanPlaceSelectedTargetAt(target))
            {
                return false;
            }

            return base.CanHitTarget(target);
        }

        public override bool Valid(LocalTargetInfo target, bool showMessages = true)
        {
            AcceptanceReport report = CanSkipTarget(target);
            if (!report)
            {
                Pawn pawn;
                if (showMessages && !report.Reason.NullOrEmpty() && (pawn = (target.Thing as Pawn)) != null)
                {
                    Messages.Message("CannotSkipTarget".Translate(pawn.Named("PAWN")) + ": " + report.Reason, pawn, MessageTypeDefOf.RejectInput, historical: false);
                }

                return false;
            }

            return base.Valid(target, showMessages);
        }

        private AcceptanceReport CanSkipTarget(LocalTargetInfo target)
        {
            Pawn pawn;
            if ((pawn = (target.Thing as Pawn)) != null)
            {
                if (pawn.BodySize > Props.maxBodySize)
                {
                    return "CannotSkipTargetTooLarge".Translate();
                }

                if (pawn.kindDef.skipResistant)
                {
                    return "CannotSkipTargetPsychicResistant".Translate();
                }
            }

            return true;
        }

        public override string ExtraLabelMouseAttachment(LocalTargetInfo target)
        {
            return CanSkipTarget(target).Reason;
        }

        public static void SendSkipUsedSignal(LocalTargetInfo target, Thing initiator)
        {
            Find.SignalManager.SendSignal(new Signal(SkipUsedSignalTag, target.Named("POSITION"), initiator.Named("SUBJECT")));
        }
    }
}
