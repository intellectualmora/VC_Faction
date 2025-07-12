using System;
using System.Collections.Generic;
using System.Linq;
using LudeonTK;
using Verse;
using Verse.AI;
using RimWorld;
using RimWorld.QuestGen;
using UnityEngine;

namespace VC_Faction
{
    public class JobDriver_TalkWithPawn : JobDriver
    {
        private Pawn Talker => (Pawn)this.TargetThingA;
        public override bool TryMakePreToilReservations(bool errorOnFailed) => this.pawn.Reserve((LocalTargetInfo)(Thing)this.Talker, this.job, errorOnFailed: errorOnFailed);

        protected override IEnumerable<Toil> MakeNewToils()
        {

            this.FailOnDespawnedOrNull<JobDriver_TalkWithPawn>(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch).FailOn<Toil>((Func<bool>)(() => false));
            Toil talk = ToilMaker.MakeToil(nameof(MakeNewToils));
            Quest quest = QuestGen.slate.Get<Quest>(Talker.Name.ToStringShort+ "_VC_Quest");
            
            if (!(quest == null))
                talk.initAction = (Action)(() =>
                {
                    Pawn actor = talk.actor;
                    string name = Talker.Name.ToStringShort;
                    string key = quest.tags[0];
                    string choice1 = null;
                    string choice2 = null;
                    string content = null;
                    string outSignal1 = null;
                    string outSignal2 = null;
                    Texture2D img = null;

                    RoleScriptDef role = DefDatabase<RoleScriptDef>.AllDefs.FirstOrDefault(r => r.label == name);
                    if (role != null)
                    {
                        if (role.portraitPathDictionary.ContainsKey(key))
                            img = ContentFinder<Texture2D>.Get(role.portraitPathDictionary[key]);
                        if (role.contentDictionary.ContainsKey(key))
                            content = role.contentDictionary[key];
                        if (role.choice1Dictionary.ContainsKey(key))
                            choice1 = role.choice1Dictionary[key];
                        if (role.choice2Dictionary.ContainsKey(key))
                            choice2 = role.choice2Dictionary[key];
                        if (role.outSignal1Dictionary.ContainsKey(key))
                            outSignal1 = "Quest" + quest.id.ToString() + "." + role.outSignal1Dictionary[key];
                        if (role.outSignal2Dictionary.ContainsKey(key))
                            outSignal2 = "Quest" + quest.id.ToString() + "." + role.outSignal2Dictionary[key];
                        Find.WindowStack.Add((Window)new Dialog_Talk(img, content, choice1, outSignal1, choice2,
                            outSignal2));
                    }
                    QuestGen.slate.Set<Quest>(Talker.Name.ToStringShort + "_VC_Quest", null);
                });
            

            yield return talk;
        }
    }
}