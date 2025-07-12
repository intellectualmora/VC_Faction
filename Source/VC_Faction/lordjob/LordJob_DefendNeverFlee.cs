using System.Collections.Generic;
using Verse;
using Verse.AI.Group;
using RimWorld;
namespace VC_Faction
{
    public class LordJob_DefendNeverFlee : LordJob
    {
        private Faction faction;
        private IntVec3 baseCenter;
        private bool attackWhenPlayerBecameEnemy;
        private List<Pawn> tmpPawns;
        private List<IntVec3> tmpCells;
        public override bool AddFleeToil => false;
        public LordJob_DefendNeverFlee()
        {

        }

        public LordJob_DefendNeverFlee(
          Faction faction,
          IntVec3 baseCenter,
          bool attackWhenPlayerBecameEnemy = false)
        {
            this.faction = faction;
            this.baseCenter = baseCenter;
            this.attackWhenPlayerBecameEnemy = attackWhenPlayerBecameEnemy;
        }

        public override StateGraph CreateGraph()
        {
            StateGraph graph = new StateGraph();
            LordToil_DefendBase lordToilDefendBase1 = new LordToil_DefendBase(this.baseCenter);
            graph.StartingToil = (LordToil)lordToilDefendBase1;
            LordToil_DefendBase lordToilDefendBase2 = new LordToil_DefendBase(this.baseCenter);
            graph.AddToil((LordToil)lordToilDefendBase2);
            LordToil_AssaultColony toilAssaultColony = new LordToil_AssaultColony(false);
            toilAssaultColony.useAvoidGrid = true;
            graph.AddToil((LordToil)toilAssaultColony);
            Transition transition1 = new Transition((LordToil)lordToilDefendBase1, (LordToil)lordToilDefendBase2);
            transition1.AddSource((LordToil)toilAssaultColony);
            transition1.AddTrigger((Trigger)new Trigger_BecameNonHostileToPlayer());
            graph.AddTransition(transition1);
            Transition transition2 = new Transition((LordToil)lordToilDefendBase2, this.attackWhenPlayerBecameEnemy ? (LordToil)toilAssaultColony : (LordToil)lordToilDefendBase1);
            if (this.attackWhenPlayerBecameEnemy)
                transition2.AddSource((LordToil)lordToilDefendBase1);
            transition2.AddTrigger((Trigger)new Trigger_BecamePlayerEnemy());
            graph.AddTransition(transition2);
            Transition transition3 = new Transition((LordToil)lordToilDefendBase1, (LordToil)toilAssaultColony);
            transition3.AddTrigger((Trigger)new Trigger_FractionPawnsLost(0.2f));
            transition3.AddTrigger((Trigger)new Trigger_PawnHarmed(0.8f));
            transition3.AddTrigger((Trigger)new Trigger_OnClamor(ClamorDefOf.Ability));
            transition3.AddPostAction((TransitionAction)new TransitionAction_WakeAll());
            graph.AddTransition(transition3);
            return graph;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look<Faction>(ref this.faction, "faction");
            Scribe_Values.Look<IntVec3>(ref this.baseCenter, "baseCenter");
            Scribe_Values.Look<bool>(ref this.attackWhenPlayerBecameEnemy, "attackWhenPlayerBecameEnemy");
        }
    }
}