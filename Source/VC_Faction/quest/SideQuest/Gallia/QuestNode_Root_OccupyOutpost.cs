// Decompiled with JetBrains decompiler
// Type: RimWorld.QuestGen.QuestNode_Root_DistressCall
// Assembly: Assembly-CSharp, Version=1.5.9214.33606, Culture=neutral, PublicKeyToken=null
// MVID: 630E2863-BC9A-4A34-93F2-EFF01E3A9556
// Assembly location: E:\SteamLibrary\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\Assembly-CSharp.dll

using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using RimWorld;
using RimWorld.QuestGen;
using UnityEngine;
using Verse;

namespace VC_Faction
{
    public class QuestNode_Root_OccupyOutpost : QuestNode
    {
        public FactionDef enemyfactionDef;
        public FactionDef hostfactionDef;
        public QuestScriptDef questScriptDef;
        public List<ThingDefCountClass> things = new List<ThingDefCountClass>();
        protected override bool TestRunInt(Slate slate)
        {
            return true;
        }
        protected override void RunInt()
        {
            Quest quest = RimWorld.QuestGen.QuestGen.quest;
            Slate slate = RimWorld.QuestGen.QuestGen.slate;
            quest.tags.Add("VC_EmpireSettlement");
            float num = slate.Get<float>("points");
            if ((double)num < 100.0)
                num = 100f;
            PlanetTile tile;
            this.TryFindSiteTile(out tile);
            Faction faction1 = Find.FactionManager.FirstFactionOfDef(enemyfactionDef);
            slate.Set<Faction>("faction", faction1);
            Site site = QuestGen_Sites.GenerateSite((IEnumerable<SitePartDefWithParams>)new SitePartDefWithParams[1]
            {
        new SitePartDefWithParams(DefDatabase<SitePartDef>.AllDefs.Where<SitePartDef>((Func<SitePartDef, bool>) (def => def.tags != null && def.tags.Contains("VC_EmpireSettlement") && typeof (VC_Faction.SitePartWorker_EmpireSettlement).IsAssignableFrom(def.workerClass))).RandomElementByWeight<SitePartDef>((Func<SitePartDef, float>) (sp => sp.selectionWeight)), new SitePartParams()
        {
          threatPoints = num
        })
            }, tile, faction1);
            quest.SpawnWorldObject((WorldObject)site);
            slate.Set<Site>("site", site);

            string inSignalEnable = QuestGenUtility.HardcodedSignalWithQuestID("site.MapGenerated");
            string inSignal1 = QuestGenUtility.HardcodedSignalWithQuestID("site.AllEnemiesDefeated");
            string inSignal2 = QuestGenUtility.HardcodedSignalWithQuestID("site.MapRemoved");
            string inSignal3 = QuestGenUtility.HardcodedSignalWithQuestID("AllFlagsDestoried");
            QuestPart_GiveRoyalFavor part = new QuestPart_GiveRoyalFavor();
            part.giveToAccepter = false;
            part.giveTo = slate.Get<Pawn>(questScriptDef.defName + "_Accepter");
            part.faction = Find.FactionManager.FirstFactionOfDef(hostfactionDef);
            part.amount = 6;
            part.inSignal = inSignal1;            
            quest.AddPart(part);

            QuestPart_GiveNearPawn gpart = new QuestPart_GiveNearPawn();
            gpart.nearPawn =
                Find.AnyPlayerHomeMap.mapPawns.AllHumanlikeSpawned.Find(p=>p.Faction == Faction.OfPlayer);
            ThingDefCountClass silver = new ThingDefCountClass();
            silver.thingDef = RimWorld.ThingDefOf.Silver;
            silver.count = 500;
            things.Add(silver);
            ThingDefCountClass medicine = new ThingDefCountClass();
            medicine.thingDef = RimWorld.ThingDefOf.MedicineIndustrial;
            medicine.count = 5;
            things.Add(medicine);
            gpart.thingDefs = things;
            gpart.inSignal = inSignal1;
            quest.AddPart(gpart);

            QuestPart_Letter part2 = new QuestPart_Letter();
            part2.inSignal = inSignal2;
            part2.letter = QuestGenUtility.MakeLetter((TaggedString)"failedLabel", (TaggedString)"failedCause1", LetterDefOf.NegativeEvent);
            part2.letter.relatedFaction = Find.FactionManager.FirstFactionOfDef(FactionDefOf.VC_Gallia);
            quest.AddPart((QuestPart)part2);

            quest.WorldObjectTimeout((WorldObject)site, 400000);
            quest.Delay(400000, (Action)(() => QuestGen_End.End(quest, QuestEndOutcome.Fail)));
            quest.End(QuestEndOutcome.Success, inSignal: inSignal1);
            quest.End(QuestEndOutcome.Fail, inSignal: inSignal2);

        }

        private bool TryFindSiteTile(out PlanetTile tile) => TileFinder.TryFindNewSiteTile(out tile, 3, 9);
    }
}
