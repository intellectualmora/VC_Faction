using System;
using System.Collections.Generic;
using RimWorld;
using RimWorld.QuestGen;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace VC_Faction
{
    public class LordToil_WaitForTalk : LordToil
    {
        public bool allowRandomInteractions = true;
        public IntVec3? cell;
        public Pawn target;
        public Quest quest;
        protected virtual DutyDef IdleDutyDef => !this.allowRandomInteractions ? RimWorld.DutyDefOf.IdleNoInteraction : RimWorld.DutyDefOf.Idle;

        public LordToil_WaitForTalk(bool allowRandomInteractions = true) => this.allowRandomInteractions = allowRandomInteractions;

        public override void DrawPawnGUIOverlay(Pawn pawn) => pawn.Map.overlayDrawer.DrawOverlay((Thing)pawn, OverlayTypes.QuestionMark);

        public override void UpdateAllDuties()
        {
            if (!this.cell.HasValue && this.lord.ownedPawns.Any<Pawn>())
                this.cell = new IntVec3?(this.lord.ownedPawns[0].Position);
            if (!this.cell.HasValue)
                return;
            for (int index = 0; index < this.lord.ownedPawns.Count; ++index)
            {
                PawnDuty pawnDuty = new PawnDuty(RimWorld.DutyDefOf.WanderClose, (LocalTargetInfo)this.cell.Value);
                this.lord.ownedPawns[index].mindState.duty = pawnDuty;
            }
        }

        public override IEnumerable<FloatMenuOption> ExtraFloatMenuOptions(Pawn target,Pawn forPawn)
        {
        
            if (target == this.target)
            {
                // ISSUE: reference to a compiler-generated method
                yield return new FloatMenuOption((string)("与"+this.target.Name.ToStringShort+"交谈"), () =>
                {
                    QuestGen.slate.Set<Quest>(this.target.Name.ToStringShort + "_VC_Quest", quest);
                    Job myJob = JobMaker.MakeJob(JobDefOf.TalkWithPawn, new LocalTargetInfo(target));
                    forPawn.jobs.StartJob(myJob, JobCondition.InterruptForced);
                });
            }
        }
        protected virtual void DecoratePawnDuty(PawnDuty duty)
        {
        }
    }
}