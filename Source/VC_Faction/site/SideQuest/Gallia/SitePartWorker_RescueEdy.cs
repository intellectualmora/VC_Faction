// Decompiled with JetBrains decompiler
// Type: RimWorld.Planet.SitePartWorker_DistressCall
// Assembly: Assembly-CSharp, Version=1.5.9214.33606, Culture=neutral, PublicKeyToken=null
// MVID: 630E2863-BC9A-4A34-93F2-EFF01E3A9556
// Assembly location: E:\SteamLibrary\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.BaseGen;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.AI.Group;
using static HarmonyLib.Code;

namespace VC_Faction
{
    public class SitePartWorker_RescueEdy : SitePartWorker
    {
        public override void Init(Site site, SitePart sitePart)
        {
            base.Init(site, sitePart);
            site.customLabel = (string)sitePart.def.label.Formatted(site.Faction.Named("FACTION"));
        }

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
        public override string GetPostProcessedThreatLabel(Site site, SitePart sitePart) => (string)((string)("Hostiles".Translate() + ": " + "Unknown".Translate().CapitalizeFirst()) + ("\n" + "Contains".Translate() + ": " + "Unknown".Translate().CapitalizeFirst()));
        public override void SitePartWorkerTick(SitePart sitePart)
        {
            base.SitePartWorkerTick(sitePart);        }
     
        public override void PostMapGenerate(Map map)
        {
            Site parent = map.Parent as Site;
            Faction faction = parent.Faction;
            foreach (var thing in map.listerBuildings.allBuildingsNonColonist.FindAll(p=>p.def == VC_Faction.ThingDefOf.VC_RagniteGenerator))
            {
                CompRefuelable refuel = thing.TryGetComp<CompRefuelable>();
                refuel.Refuel(Rand.Range(2f, 10f));
            }

            List<Pawn> list = PawnGroupMakerUtility.GeneratePawns(new PawnGroupMakerParms()
            {
                groupKind = PawnGroupKindDefOf.VC_Empire_Troopers,
                points = 1000,
                inhabitants = false,
                raidStrategy = RaidStrategyDefOf.ImmediateAttack,
                faction = Find.FactionManager.FirstFactionOfDef(FactionDefOf.VC_Empire),
                tile = map.Tile
            }).ToList<Pawn>();
            List<Pawn> list2 = PawnGroupMakerUtility.GeneratePawns(new PawnGroupMakerParms()
            {
                faction = faction,
                groupKind = PawnGroupKindDefOf.VC_Gallia_Militia, //义勇军
                points = 200,
                tile = map.Tile
            }).ToList<Pawn>();
            IntVec3 position =  CellFinder.RandomEdgeCell(map);
            Pawn Edy = PawnCustomize.GenerateEdy();
            VC_PawnUtility.SpawnPawn(map, Edy, map.Center, 20);
            VC_PawnUtility.SpawnPawns(map, (IEnumerable<Pawn>)list, position, 20);
            VC_PawnUtility.SpawnCorpses(map, (IEnumerable<Pawn>)list2,list, position, 20);
            Edy.SetFaction(Faction.OfPlayer); // 设置为玩家派系
            Edy.guest?.SetGuestStatus(null);
            LordMaker.MakeNewLord(Find.FactionManager.FirstFactionOfDef(FactionDefOf.VC_Empire), new LordJob_AssaultColony(Find.FactionManager.FirstFactionOfDef(FactionDefOf.VC_Gallia)), map, list);
            Messages.Message("Edy 已加入你的殖民地。", Edy, MessageTypeDefOf.PositiveEvent);
            foreach (Thing allThing in map.listerThings.AllThings)
            {
                if (allThing.def.category == ThingCategory.Item)
                {
                    CompForbiddable comp = allThing.TryGetComp<CompForbiddable>();
                    if (comp != null && !comp.Forbidden)
                        allThing.SetForbidden(true, false);
                }
            }

            
            List<Building> shelfs = map.listerBuildings.allBuildingsNonColonist.FindAll(p => p.def == DefDatabase<ThingDef>.AllDefs.FirstOrDefault(d => d.defName == "Shelf"));
            foreach (Building shelf in shelfs)
            {
                if (Rand.Range(0f, 1f) > 0.5f)
                {
                    Thing rawCorn = ThingMaker.MakeThing(DefDatabase<ThingDef>.AllDefs.FirstOrDefault(d => d.defName == "RawCorn"));
                    rawCorn.stackCount = (int)Rand.Range(1f, 70f);
                    GenPlace.TryPlaceThing(rawCorn, shelf.Position, map, ThingPlaceMode.Direct);
                }
                if (Rand.Range(0f, 1f) > 0.5f)
                {
                    Thing medicine = ThingMaker.MakeThing(DefDatabase<ThingDef>.AllDefs.FirstOrDefault(d => d.defName == "MedicineIndustrial"));
                    medicine.stackCount = (int)Rand.Range(1f, 5f);
                    GenPlace.TryPlaceThing(medicine, shelf.Position, map, ThingPlaceMode.Direct);
                }
                if (Rand.Range(0f, 1f) > 0.5f)
                {
                    Thing mealSimple = ThingMaker.MakeThing(DefDatabase<ThingDef>.AllDefs.FirstOrDefault(d => d.defName == "MealSimple"));
                    mealSimple.stackCount = (int)Rand.Range(1f, 5f);
                    GenPlace.TryPlaceThing(mealSimple, shelf.Position, map, ThingPlaceMode.Direct);
                }
                if (Rand.Range(0f, 1f) > 0.5f)
                {
                    Thing ragnite = ThingMaker.MakeThing(DefDatabase<ThingDef>.AllDefs.FirstOrDefault(d => d.defName == "Ragnite"));
                    ragnite.stackCount = (int)Rand.Range(1f, 20f);
                    GenPlace.TryPlaceThing(ragnite, shelf.Position, map, ThingPlaceMode.Direct);
                }

            }
        }

    }
}