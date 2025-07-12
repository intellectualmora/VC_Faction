using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace VC_Faction
{
    public class Building_Radio : Building
    {
        private CompPowerTrader powerComp;

        public bool CanUseCommsNow
        {
            get
            {
                if (this.Spawned && this.Map.gameConditionManager.ElectricityDisabled(this.Map))
                    return false;
                return this.powerComp == null || this.powerComp.PowerOn;
            }
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            this.powerComp = this.GetComp<CompPowerTrader>();
            if (!this.CanUseCommsNow)
                return;
            LongEventHandler.ExecuteWhenFinished(new Action(this.AnnounceRadioSignals));
        }

        private void UseAct(Pawn myPawn, ICommunicable commTarget)
        {
            Job job = JobMaker.MakeJob(VC_Faction.JobDefOf.VC_UseRadio, (LocalTargetInfo)(Thing)this);
            job.commTarget = commTarget;
            myPawn.jobs.TryTakeOrderedJob(job);
            PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.OpeningComms, KnowledgeAmount.Total);
        }

        private FloatMenuOption GetFailureReason(Pawn myPawn)
        {
            if (!myPawn.CanReach((LocalTargetInfo)(Thing)this, PathEndMode.InteractionCell, Danger.Some))
                return new FloatMenuOption((string)"CannotUseNoPath".Translate(), (Action)null);
            if (this.Spawned && this.Map.gameConditionManager.ElectricityDisabled(this.Map))
                return new FloatMenuOption((string)"CannotUseSolarFlare".Translate(), (Action)null);
            if (this.powerComp != null && !this.powerComp.PowerOn)
                return new FloatMenuOption((string)"CannotUseNoPower".Translate(), (Action)null);
            if (!myPawn.health.capacities.CapableOf(PawnCapacityDefOf.Talking))
                return new FloatMenuOption((string)"CannotUseReason".Translate((NamedArgument)"IncapableOfCapacity".Translate((NamedArgument)PawnCapacityDefOf.Talking.label, myPawn.Named("PAWN"))), (Action)null);
            if (!this.GetCommTargets(myPawn).Any<ICommunicable>())
                return new FloatMenuOption((string)"没有收到任何无线电信号", (Action)null);
            if (this.CanUseCommsNow)
                return (FloatMenuOption)null;
            Log.Error(myPawn.ToString() + " could not use comm console for unknown reason.");
            return new FloatMenuOption("Cannot use now", (Action)null);
        }

        public IEnumerable<ICommunicable> GetCommTargets(Pawn myPawn)
        {
            IEnumerable < ICommunicable > communicables = myPawn.Map.GetComponent<RadioSignalMapComponent>().radioSignalManager.radioSignals.Cast<ICommunicable>();
            return communicables;
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn myPawn)
        {
            Building_Radio console = this;
            FloatMenuOption failureReason = console.GetFailureReason(myPawn);
            if (failureReason != null)
            {
                yield return failureReason;
            }
            else
            {
                foreach (ICommunicable commTarget in console.GetCommTargets(myPawn))
                {
                    FloatMenuOption floatMenuOption = commTarget.CommFloatMenuOption(console, myPawn);
                    if (floatMenuOption != null)
                        yield return floatMenuOption;
                }
                // ISSUE: reference to a compiler-generated method
                foreach (FloatMenuOption floatMenuOption in base.GetFloatMenuOptions(myPawn))
                    yield return floatMenuOption;
            }
        }

        public void GiveUseCommsJob(Pawn negotiator, ICommunicable target)
        {
            Job job = JobMaker.MakeJob(VC_Faction.JobDefOf.VC_UseRadio, (LocalTargetInfo)(Thing)this);
            job.commTarget = target;
            negotiator.jobs.TryTakeOrderedJob(job);
            PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.OpeningComms, KnowledgeAmount.Total);
        }

        private void AnnounceRadioSignals()
        {
            foreach (MissionSignal missionSignal in this.Map.GetComponent<RadioSignalMapComponent>().radioSignalManager.radioSignals.OfType<MissionSignal>().Where<MissionSignal>((Func<MissionSignal, bool>)(s => !s.WasAnnounced)))
            {
                TaggedString baseLetterText = "收到到来自"+ missionSignal.Faction.Name+ missionSignal.name+"的无线电信号";
                IncidentWorker.SendIncidentLetter(missionSignal.MissionName, baseLetterText, LetterDefOf.PositiveEvent, new IncidentParms()
                {
                    target = (IIncidentTarget)this.Map,
                }, LookTargets.Invalid, (IncidentDef)null);
                missionSignal.WasAnnounced = true;
            }
        }
    }
}
