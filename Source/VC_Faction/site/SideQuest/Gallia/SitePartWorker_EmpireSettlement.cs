// Decompiled with JetBrains decompiler
// Type: RimWorld.Planet.SitePartWorker_DistressCall
// Assembly: Assembly-CSharp, Version=1.5.9214.33606, Culture=neutral, PublicKeyToken=null
// MVID: 630E2863-BC9A-4A34-93F2-EFF01E3A9556
// Assembly location: E:\SteamLibrary\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.BaseGen;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace VC_Faction
{
    public class SitePartWorker_EmpireSettlement : SitePartWorker
    {
        private Map map;
        private Quest quest;
        private List<Building> flags;
        public override void Init(Site site, SitePart sitePart)
        {
            base.Init(site, sitePart);
            this.quest = null;
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
            base.SitePartWorkerTick(sitePart);
            CheckAllFlagsDestoryed();
        }
        private void CheckAllFlagsDestoryed()
        {
            if (map == null)
            {
                return;
            }

           
            foreach (var flag in flags)
            {
                if(flag!=null)
                    if (!flag.Destroyed)
                    {
                        return;
                    }
            }

            Find.SignalManager.SendSignal(new Signal("Quest" + quest.id.ToString() + ".AllFlagsDestoried"));

        }
        public override void PostMapGenerate(Map map)
        {
            quest = Find.QuestManager.questsInDisplayOrder.Find(q => q.tags.Contains("VC_EmpireSettlement"));
            this.map = map;
            Site parent = map.Parent as Site;
            Faction faction = parent.Faction;
            foreach (var thing in map.listerBuildings.allBuildingsNonColonist.FindAll(p=>p.def == VC_Faction.ThingDefOf.VC_RagniteGenerator))
            {
                CompRefuelable refuel = thing.TryGetComp<CompRefuelable>();
                refuel.Refuel(Rand.Range(2f, 10f));
            }
            flags =
                map.listerBuildings.allBuildingsNonColonist.FindAll(p => p.def == VC_Faction.ThingDefOf.VC_Empire_Flag);
            foreach (var flag in flags)
            {
                List<Pawn> plist = PawnGroupMakerUtility.GeneratePawns(new PawnGroupMakerParms()
                {
                    faction = faction,
                    groupKind = PawnGroupKindDefOf.VC_Empire_Troopers,
                    points = TroopersPointsModifierCurve.Evaluate(parent.desiredThreatPoints) * 0.6f,
                    tile = map.Tile
                }).ToList<Pawn>();
                VC_PawnUtility.SpawnPawns(map, (IEnumerable<Pawn>)plist, flag.Position, 20);
                LordMaker.MakeNewLord(map.ParentFaction, new LordJob_DefendFleeWithSignal(map.ParentFaction, flag.Position, "Quest" + quest.id.ToString() + ".AllFlagsDestoried"), map, plist);

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

        }

    }
}