// Decompiled with JetBrains decompiler
// Type: RimWorld.QuestGen.QuestNode_Root_DistressCall
// Assembly: Assembly-CSharp, Version=1.5.9214.33606, Culture=neutral, PublicKeyToken=null
// MVID: 630E2863-BC9A-4A34-93F2-EFF01E3A9556
// Assembly location: E:\SteamLibrary\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\Assembly-CSharp.dll

using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using RimWorld;
using RimWorld.QuestGen;
using UnityEngine;
using Verse;

namespace VC_Faction
{
    public class QuestNode_Root_Bruhl : QuestNode
    {

        protected override bool TestRunInt(Slate slate)
        {
            return true;
        }

        protected override void RunInt()
        {
           
            Quest quest = RimWorld.QuestGen.QuestGen.quest;
            Slate slate = RimWorld.QuestGen.QuestGen.slate;
            quest.tags.Add("Bruhl");
            float num = slate.Get<float>("points");
            if ((double)num < 100.0)
                num = 100f;
            int tile;
            this.TryFindSiteTile(out tile);
            Faction faction1 = Find.FactionManager.FirstFactionOfDef(FactionDefOf.VC_Gallia);
            slate.Set<Faction>("faction", faction1);
            WorldObjectDefOf.Site.canBePlayerHome = false; // 不允许玩家定居
            Site site = QuestGen_Sites.GenerateSite((IEnumerable<SitePartDefWithParams>)new SitePartDefWithParams[1]
            {
        new SitePartDefWithParams(DefDatabase<SitePartDef>.AllDefs.Where<SitePartDef>((Func<SitePartDef, bool>) (def => def.tags != null && def.tags.Contains("Bruhl") && typeof (SitePartWorker_Bruhl_Empire_Troopers).IsAssignableFrom(def.workerClass))).RandomElementByWeight<SitePartDef>((Func<SitePartDef, float>) (sp => sp.selectionWeight)), new SitePartParams()
        {
          threatPoints = num
        })
            }, tile, faction1);
            quest.SpawnWorldObject((WorldObject)site);
            slate.Set<Site>("site", site);

            string inSignalEnable = QuestGenUtility.HardcodedSignalWithQuestID("site.MapGenerated");
            string inSignal1 = QuestGenUtility.HardcodedSignalWithQuestID("site.AllEnemiesDefeated");
            string inSignal2 = QuestGenUtility.HardcodedSignalWithQuestID("site.MapRemoved");
            string inSignal3 = QuestGenUtility.HardcodedSignalWithQuestID("WelkinIsDead");
            string inSignal4 = QuestGenUtility.HardcodedSignalWithQuestID("AliciaIsDead");
            string inSignal5 = QuestGenUtility.HardcodedSignalWithQuestID("MissionSuccessed");
            Pawn welkin = PawnCustomize.GenerateWelkin();
            Pawn alicia = PawnCustomize.GenerateAlicia();
            QuestPart_GiveRoyalFavor part = new QuestPart_GiveRoyalFavor();
            part.giveToAccepter = true;
            part.faction = Find.FactionManager.FirstFactionOfDef(FactionDefOf.VC_Gallia);
            part.amount = 6;
            part.inSignal = inSignal5;            
            quest.AddPart(part);

            var choice = new QuestPart_Choice.Choice();
            Reward_RoyalFavor honorReward = new Reward_RoyalFavor();
            honorReward.amount = 6;
            honorReward.faction = Find.FactionManager.FirstFactionOfDef(FactionDefOf.VC_Gallia);
            choice.rewards.Add(honorReward);
            quest.RewardChoice().choices.Add(choice);

            QuestPart_Letter part2 = new QuestPart_Letter();
            part2.inSignal = inSignal3;
            part2.letter = QuestGenUtility.MakeLetter((TaggedString)"failedLabel", (TaggedString)"failedCause1", LetterDefOf.NegativeEvent);
            part2.letter.relatedFaction = Find.FactionManager.FirstFactionOfDef(FactionDefOf.VC_Gallia);
            quest.AddPart((QuestPart)part2);

            QuestPart_Letter part3 = new QuestPart_Letter();
            part3.inSignal = inSignal4;
            part3.letter = QuestGenUtility.MakeLetter((TaggedString)"failedLabel", (TaggedString)"failedCause2", LetterDefOf.NegativeEvent);
            part3.letter.relatedFaction = Find.FactionManager.FirstFactionOfDef(FactionDefOf.VC_Gallia);
            quest.AddPart((QuestPart)part3);

            QuestPart_Letter part4 = new QuestPart_Letter();
            part4.inSignal = inSignal2;
            part4.letter = QuestGenUtility.MakeLetter((TaggedString)"failedLabel", (TaggedString)"failedCause3", LetterDefOf.NegativeEvent);
            part4.letter.relatedFaction = Find.FactionManager.FirstFactionOfDef(FactionDefOf.VC_Gallia);
            quest.AddPart((QuestPart)part4);

            QuestPart_AskWelkin part5 = new QuestPart_AskWelkin();
            part5.inSignal = inSignal1;
            part5.quest = quest;
            quest.AddPart((QuestPart)part5);

            quest.WorldObjectTimeout((WorldObject)site, 900000);
            quest.Delay(900000, (Action)(() => QuestGen_End.End(quest, QuestEndOutcome.Fail)));
            quest.End(QuestEndOutcome.Success, inSignal: inSignal5);
            quest.End(QuestEndOutcome.Fail, inSignal: inSignal2);
            quest.End(QuestEndOutcome.Fail, inSignal: inSignal3);
            quest.End(QuestEndOutcome.Fail, inSignal: inSignal4);

        }

        private bool TryFindSiteTile(out int tile) => TileFinder.TryFindNewSiteTile(out tile, 3, 9);
    }
}
