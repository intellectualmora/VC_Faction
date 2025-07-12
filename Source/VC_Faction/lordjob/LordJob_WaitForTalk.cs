using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI.Group;
using RimWorld;
using RimWorld.QuestGen;

namespace VC_Faction
{
    public class LordJob_WaitForTalk : LordJob
    {
        public Pawn target;
        public Quest quest;
        public override bool AddFleeToil => false;
        public LordJob_WaitForTalk(Pawn target,Quest quest)
        {
            this.target = target;
            this.quest = quest;
        }


        public override StateGraph CreateGraph()
        {
            StateGraph graph = new StateGraph();
            LordToil_WaitForTalk lordToilWaitForTalk = new LordToil_WaitForTalk();
            lordToilWaitForTalk.target = target;
            lordToilWaitForTalk.cell = target.Map.Center;
            lordToilWaitForTalk.quest = quest;
            graph.StartingToil = (LordToil)lordToilWaitForTalk;
            return graph;
        }

        public override void ExposeData()
        {
            base.ExposeData();
        }
    }
}