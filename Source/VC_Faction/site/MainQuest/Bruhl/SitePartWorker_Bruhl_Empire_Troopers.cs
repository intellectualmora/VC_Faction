// Decompiled with JetBrains decompiler
// Type: RimWorld.Planet.SitePartWorker_DistressCall
// Assembly: Assembly-CSharp, Version=1.5.9214.33606, Culture=neutral, PublicKeyToken=null
// MVID: 630E2863-BC9A-4A34-93F2-EFF01E3A9556
// Assembly location: E:\SteamLibrary\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.BaseGen;
using RimWorld.Planet;
using RimWorld.QuestGen;
using UnityEngine;
using Verse;
using Verse.AI.Group;
using Verse.Grammar;

namespace VC_Faction
{
    public class SitePartWorker_Bruhl_Empire_Troopers : SitePartWorker
    {
        private bool isDead = false;
        private Pawn welkin;
        private Pawn alicia;
        private Quest quest;
        public override void Init(Site site, SitePart sitePart)
        {
            base.Init(site, sitePart);
            site.customLabel = (string)sitePart.def.label.Formatted(site.Faction.Named("FACTION"));
        }
        public override string GetPostProcessedThreatLabel(Site site, SitePart sitePart) => (string)((string)("Hostiles".Translate() + ": " + "Unknown".Translate().CapitalizeFirst()) + ("\n" + "Contains".Translate() + ": " + "Unknown".Translate().CapitalizeFirst()));

        private static readonly SimpleCurve TroopersPointsModifierCurve = new SimpleCurve()
    {
      {
        new CurvePoint(100f, 100f),
        true
      },
      {
        new CurvePoint(500f, 250f),
        true
      },
      {
        new CurvePoint(1000f, 400f),
        true
      },
      {
        new CurvePoint(5000f, 800f),
        true
      }
    };
        public override void SitePartWorkerTick(SitePart sitePart)
        {
            base.SitePartWorkerTick(sitePart);
            CheckPawnisDead(this.welkin);
            CheckPawnisDead(this.alicia);
        }
        private void CheckPawnisDead(Pawn pawn)
        {
            if (quest == null)
            {
                foreach (Quest q in  Find.QuestManager.questsInDisplayOrder)
                {
                    if (q.tags!=null&&q.tags.Contains("Bruhl"))
                    {
                        this.quest = q;
                        break;
                    }

                }

            }
            if (pawn == null || isDead)
            {
                return;
            }
            if (pawn.health.Dead)
            {
                if (pawn.Name.ToStringShort.Contains("维尔金"))
                {
                    Find.SignalManager.SendSignal(new Signal("Quest" + quest.id.ToString() + ".WelkinIsDead"));
                }
                else
                {
                    Find.SignalManager.SendSignal(new Signal("Quest" + quest.id.ToString() + ".AliciaIsDead"));
                }
                this.isDead = true;
            }
        }
        public override void PostMapGenerate(Map map)
        {
            Site parent = map.Parent as Site;
            Faction faction = parent.Faction;
            List<Pawn> list1 = PawnGroupMakerUtility.GeneratePawns(new PawnGroupMakerParms()
            {
                faction = faction,
                groupKind = PawnGroupKindDefOf.VC_Villagers, //村民
                points = SymbolResolver_Settlement.DefaultPawnsPoints.RandomInRange * 0.22f,
                tile = map.Tile
            }).ToList<Pawn>();
            float num2 = SymbolResolver_Settlement.DefaultPawnsPoints.RandomInRange * 0.11f;
            List<Pawn> list2 = PawnGroupMakerUtility.GeneratePawns(new PawnGroupMakerParms()
            {
                faction = faction,
                groupKind = PawnGroupKindDefOf.VC_Gallia_Militia, //义勇军
                points = num2,
                tile = map.Tile
            }).ToList<Pawn>();
            float num3 = 2 * Mathf.Max(SitePartWorker_Bruhl_Empire_Troopers.TroopersPointsModifierCurve.Evaluate(parent.desiredThreatPoints), num2);
            List<Pawn> list3 = PawnGroupMakerUtility.GeneratePawns(new PawnGroupMakerParms()
            {
                groupKind = PawnGroupKindDefOf.VC_Empire_Troopers,
                points = num3,
                inhabitants = false,
                raidStrategy = RaidStrategyDefOf.ImmediateAttack,
                faction = Find.FactionManager.FirstFactionOfDef(FactionDefOf.VC_Empire),
                tile = map.Tile
            }).ToList<Pawn>();

            welkin = PawnCustomize.GenerateWelkin();
            alicia = PawnCustomize.GenerateAlicia();
            list2.Add(welkin);
            list2.Add(alicia);
            VC_PawnUtility.SpawnPawns(map, (IEnumerable<Pawn>)list1, map.Center, 20);
            VC_PawnUtility.SpawnPawns(map, (IEnumerable<Pawn>)list2, map.listerBuildings.allBuildingsNonColonist.FirstOrDefault(f=>f.def == VC_Faction.ThingDefOf.VC_Gallia_Flag).Position, 20);
            VC_PawnUtility.SpawnPawns(map, (IEnumerable<Pawn>)list3, CellFinder.RandomEdgeCell(map), 20);
            var hat = welkin.apparel.WornApparel.FirstOrDefault(apparel =>
                apparel.def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.FullHead) ||
                apparel.def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.UpperHead));
            if (hat != null)
            {
                welkin.apparel.Remove(hat);
            }
            var hat2 = alicia.apparel.WornApparel.FirstOrDefault(apparel =>
                apparel.def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.FullHead) ||
                apparel.def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.UpperHead));
            if (hat2 != null)
            {
                alicia.apparel.Remove(hat2);
            }
            list1 = list1.Concat(list2).ToList();

            LordMaker.MakeNewLord(Find.FactionManager.FirstFactionOfDef(FactionDefOf.VC_Gallia), new LordJob_DefendNeverFlee(Find.FactionManager.FirstFactionOfDef(FactionDefOf.VC_Gallia), map.Center), map, list1);
            LordMaker.MakeNewLord(Find.FactionManager.FirstFactionOfDef(FactionDefOf.VC_Empire), new LordJob_AssaultColony(Find.FactionManager.FirstFactionOfDef(FactionDefOf.VC_Gallia)), map, list3);
            foreach (var thing in map.listerBuildings.allBuildingsNonColonist)
            {
                if (thing.def == VC_Faction.ThingDefOf.VC_RagniteGenerator)
                {
                    CompRefuelable refuel = thing.TryGetComp<CompRefuelable>();
                    refuel.Refuel(Rand.Range(2f, 10f));
                }
            }
            foreach (Thing allThing in map.listerThings.AllThings)
            {
                if (allThing.def.category == ThingCategory.Item)
                {
                    CompForbiddable comp = allThing.TryGetComp<CompForbiddable>();
                    if (comp != null && !comp.Forbidden)
                        allThing.SetForbidden(true, false);
                }
            }
            //QuestGen.slate.Set<Map>("bruhlMap", map);

        }

    }
}
