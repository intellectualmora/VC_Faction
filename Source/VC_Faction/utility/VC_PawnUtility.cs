﻿// Decompiled with JetBrains decompiler
// Type: RimWorld.Planet.DistressCallUtility
// Assembly: Assembly-CSharp, Version=1.5.9214.33606, Culture=neutral, PublicKeyToken=null
// MVID: 630E2863-BC9A-4A34-93F2-EFF01E3A9556
// Assembly location: E:\SteamLibrary\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace VC_Faction
{
    public static class VC_PawnUtility
    {
        private static readonly IntRange BloodFilthToSpawn = new IntRange(1, 5);

        public static void SpawnPawns(Map map, IEnumerable<Pawn> pawns, IntVec3 root, int radius)
        {
            foreach (Pawn pawn in pawns)
            {
                IntVec3 result;
                if (!RCellFinder.TryFindRandomCellNearWith(root, (Predicate<IntVec3>)(c => c.Standable(map)), map, out result, radius))
                    break;
                GenSpawn.Spawn((Thing)pawn, result, map);
            }
        }

        public static void SpawnPawn(Map map, Pawn pawn, IntVec3 root, int radius)
        {
                IntVec3 result;
                if (!RCellFinder.TryFindRandomCellNearWith(root, (Predicate<IntVec3>)(c => c.Standable(map)), map, out result, radius))
                    return;
                GenSpawn.Spawn((Thing)pawn, result, map);
        }

        public static void SpawnCorpses(
          Map map,
          IEnumerable<Pawn> pawns,
          IEnumerable<Pawn> killers,
          IntVec3 root,
          int radius)
        {
            int num = Find.TickManager.TicksGame - map.Parent.creationGameTicks;
            foreach (Pawn pawn in pawns)
            {
                HealthUtility.SimulateKilledByPawn(pawn, killers.RandomElement<Pawn>());
                Corpse corpse = pawn.Corpse;
                if (corpse != null)
                {
                    corpse.timeOfDeath = map.Parent.creationGameTicks;
                    CompRottable comp = corpse.TryGetComp<CompRottable>();
                    if (comp != null)
                        comp.RotProgress += (float)num;
                    IntVec3 result;
                    if (RCellFinder.TryFindRandomCellNearWith(root, (Predicate<IntVec3>)(c => c.Standable(map) && c.GetEdifice(map) == null), map, out result, radius))
                    {
                        if (corpse.InnerPawn.kindDef.IsFleshBeast() && comp.Stage == RotStage.Dessicated)
                        {
                            FilthMaker.TryMakeFilth(result, map, RimWorld.ThingDefOf.Filth_RubbleRock);
                            break;
                        }
                        GenSpawn.Spawn((Thing)corpse, result, map);
                        pawn.DropAndForbidEverything();
                        if (num < 300000)
                        {
                            int randomInRange = VC_PawnUtility.BloodFilthToSpawn.RandomInRange;
                            for (int index = 0; index < randomInRange; ++index)
                            {
                                IntVec3 intVec3 = CellFinder.RandomClosewalkCellNear(result, map, 3);
                                if (intVec3.InBounds(map) && GenSight.LineOfSight(intVec3, result, map))
                                    FilthMaker.TryMakeFilth(intVec3, map, pawn.RaceProps.BloodDef, pawn.LabelIndefinite());
                            }
                        }
                    }
                }
            }
        }
    }
}
