// Decompiled with JetBrains decompiler
// Type: RimWorld.GenStep_Settlement
// Assembly: Assembly-CSharp, Version=1.5.9214.33606, Culture=neutral, PublicKeyToken=null
// MVID: 630E2863-BC9A-4A34-93F2-EFF01E3A9556
// Assembly location: E:\SteamLibrary\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\Assembly-CSharp.dll

using RimWorld.BaseGen;
using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace VC_Faction
{
    public class GenStep_EmpireRefinery : GenStep_Scatterer
    {
        private static readonly int SettlementSize = 60;
        private static readonly int WallSize = 50;

        private bool clearBuildingFaction;
        private static List<IntVec3> tmpCandidates = new List<IntVec3>();

        public override int SeedPart => 1806208471;

        protected override bool CanScatterAt(IntVec3 c, Map map)
        {
            int min = SettlementSize;
            return new CellRect(c.x - min / 2, c.z - min / 2, min, min).FullyContainedWithin(new CellRect(0, 0, map.Size.x, map.Size.z));
        }

        public static bool hasSteamGeyser(Map map, IntVec3 c)
        {
            List<Thing> things = c.GetThingList(map);
            for (int i = things.Count - 1; i >= 0; i--)
            {
                if (things[i].def == RimWorld.ThingDefOf.SteamGeyser)
                {
                   return true;
                }
            }

            return false;
        }
        public static void Destory(Map map,IntVec3 cell)
        {
            if (hasSteamGeyser(map, cell))
            {
                return;
            }
            List<Thing> things = cell.GetThingList(map);
            for (int i = things.Count - 1; i >= 0; i--)
            {
                things[i].Destroy();
            }
        }
        public static void SetTerrain(Map map,IntVec3 c,TerrainDef def)
        {
            if (!hasSteamGeyser(map, c))
                map.terrainGrid.SetTerrain(c, def);

        }
        public static void Spawn(Thing thing, IntVec3 pos, Map map)
        {
            if (!hasSteamGeyser(map, pos))
                GenSpawn.Spawn(thing, pos, map, WipeMode.Vanish);
        }
        public static void Spawn(Thing ting, IntVec3 pos, Map map,Rot4 rot)
        {
            if (!hasSteamGeyser(map, pos))
                GenSpawn.Spawn(ting, pos, map,rot, WipeMode.Vanish);
        }
        public static void ClearMap(Map map, CellRect rect)
        {
            // 清除障碍物
            foreach (IntVec3 cell in rect)
            {
                Destory(map, cell);
                SetTerrain(map, cell, TerrainDefOf.Soil);
                // 清除小石头、遗物
                Building building = cell.GetFirstBuilding(map);
                if (building != null)
                    building.Destroy();
                // 清除屋顶
                if (cell.Roofed(map))
                    map.roofGrid.SetRoof(cell, null);
            }
        }
        public static void ConstrutExternalWall(Map map, CellRect rect)
        {
            foreach (IntVec3 c in rect.EdgeCells)
            {
                if (c.InBounds(map))
                {
                    Thing wall = ThingMaker.MakeThing(RimWorld.ThingDefOf.Wall, RimWorld.ThingDefOf.Steel);
                    Spawn(wall, c, map);
                }
            }
            IntVec3 doorPos1 = new IntVec3(rect.minX+9, 0, rect.minZ + 49);//装门
            IntVec3 doorPos2 = new IntVec3(rect.minX + 49, 0, rect.minZ + 9);
            Thing door = ThingMaker.MakeThing(RimWorld.ThingDefOf.SecurityDoor);
            Spawn(door, doorPos1, map);
            door = ThingMaker.MakeThing(RimWorld.ThingDefOf.SecurityDoor);
            Rot4 rot = Rot4.East;
            Spawn(door, doorPos2, map, rot);
            foreach (IntVec3 c in rect.Cells)
            {
                SetTerrain(map, c, TerrainDefOf.Concrete);
            } //铺水泥地板
        }
        public void ConstrutWhistle(Map map, CellRect rect)
        {
            int whistleSize = rect.maxX - rect.minX;
            foreach (IntVec3 c in rect.EdgeCells)
            {
                Thing sandbag = ThingMaker.MakeThing(RimWorld.ThingDefOf.Sandbags,RimWorld.ThingDefOf.Cloth);
                Spawn(sandbag, c, map);
            }
            IntVec3 pos1 = new IntVec3(rect.minX, 0, rect.minZ + whistleSize/2);
            IntVec3 pos2 = new IntVec3(rect.maxX, 0, rect.minZ + whistleSize/2);
            IntVec3 pos3 = new IntVec3(rect.minX + whistleSize/2, 0, rect.minZ);
            IntVec3 pos4 = new IntVec3(rect.minX + whistleSize/2, 0, rect.maxZ);
            Thing flag = ThingMaker.MakeThing(ThingDefOf.VC_Empire_Flag);
            Spawn(flag,rect.CenterCell,map);
            Destory(map, pos1);
            Destory(map, pos2);
            Destory(map, pos3);
            Destory(map, pos4);

            
        }
        public void ConstrutFactory(Map map, CellRect rect)
        {
            foreach (IntVec3 c in rect.EdgeCells)
            {
                if (c.InBounds(map))
                {
                    Thing wall = ThingMaker.MakeThing(RimWorld.ThingDefOf.Wall, RimWorld.ThingDefOf.Steel);
                    Spawn(wall, c, map);
                }
            }
            IntVec3 doorVec3 = new IntVec3(rect.minX + 10, 0, rect.minZ);
            Thing door = ThingMaker.MakeThing(RimWorld.ThingDefOf.Door, RimWorld.ThingDefOf.Steel);
            Spawn(door, doorVec3, map);
            foreach (IntVec3 c in rect.Cells)
            {
                SetTerrain(map, c, DefDatabase<TerrainDef>.AllDefs.FirstOrDefault(d => d.defName == "SterileTile"));
            }

            for (int i = 3; i + rect.minX < rect.maxX; i += 4)
            {
                for (int j = 3; j + rect.minZ < rect.maxZ; j += 4)
                {
                    Thing reactor = ThingMaker.MakeThing(ThingDefOf.VC_RagniteReactor);
                    Building_RagniteReactor br = (Building_RagniteReactor)reactor;
                    br.AddRagnite(((int) Rand.Range(2f, 19f))*8);
                    br.Progress = Rand.Range(0f, 1f);
                    CellRect nc = new CellRect(i + rect.minX - 1, j + rect.minZ - 1, 3, 3);
                    bool hasSteam = false;
                    foreach (IntVec3 c in nc.Cells)
                    {
                        if (hasSteamGeyser(map, c))
                        {
                            hasSteam = true;
                            break;
                        }
                    }
                    if (hasSteam)
                    {
                        continue;
                    }

                    Spawn(reactor, new IntVec3(rect.minX+i,0,rect.minZ+j), map);
                }
            }
            Thing column = ThingMaker.MakeThing(RimWorld.ThingDefOf.Column, RimWorld.ThingDefOf.Steel);
            Spawn(column, rect.CenterCell, map);

            foreach (IntVec3 c in rect)
            {
                if (c.InBounds(map))
                {
                    map.roofGrid.SetRoof(c, RoofDefOf.RoofConstructed);
                }
            }
        }
        public void ConstrutKitchen(Map map, CellRect rect)
        {
            foreach (IntVec3 c in rect.EdgeCells)
            {
                if (c.InBounds(map))
                {
                    Thing wall = ThingMaker.MakeThing(RimWorld.ThingDefOf.Wall, RimWorld.ThingDefOf.Steel);
                    Spawn(wall, c, map);
                }
            }
            Thing lamp = ThingMaker.MakeThing(DefDatabase<ThingDef>.AllDefs.FirstOrDefault(d => d.defName == "WallLamp"));
            Spawn(lamp, new IntVec3(rect.CenterCell.x, 0, rect.minZ + 1), map,Rot4.South);
            Thing door = ThingMaker.MakeThing(RimWorld.ThingDefOf.Door, RimWorld.ThingDefOf.Steel);
            Spawn(door, new IntVec3(rect.minX, 0, rect.CenterCell.z), map);

            Thing electricStove = ThingMaker.MakeThing(DefDatabase<ThingDef>.AllDefs.FirstOrDefault(d => d.defName == "ElectricStove"));
            if (!(hasSteamGeyser(map, new IntVec3(rect.CenterCell.x - 1, 0, rect.minZ + 1)) && hasSteamGeyser(map, new IntVec3(rect.CenterCell.x + 1, 0, rect.minZ + 1))))
                Spawn(electricStove, new IntVec3(rect.CenterCell.x, 0, rect.minZ + 1), map, Rot4.South);
            electricStove = ThingMaker.MakeThing(DefDatabase<ThingDef>.AllDefs.FirstOrDefault(d => d.defName == "ElectricStove"));
            if (!(hasSteamGeyser(map, new IntVec3(rect.CenterCell.x + 2, 0, rect.minZ + 1)) && hasSteamGeyser(map, new IntVec3(rect.CenterCell.x + 4, 0, rect.minZ + 1))))
                Spawn(electricStove, new IntVec3(rect.CenterCell.x + 3, 0, rect.minZ + 1), map,Rot4.South);

            Thing tableButcher = ThingMaker.MakeThing(RimWorld.ThingDefOf.TableButcher, RimWorld.ThingDefOf.Steel);
            if (!(hasSteamGeyser(map, new IntVec3(rect.CenterCell.x - 1, 0, rect.minZ + 5)) && hasSteamGeyser(map, new IntVec3(rect.CenterCell.x + 1, 0, rect.minZ + 5))))
                Spawn(tableButcher, new IntVec3(rect.CenterCell.x, 0, rect.minZ + 5), map);
            tableButcher = ThingMaker.MakeThing(RimWorld.ThingDefOf.TableButcher, RimWorld.ThingDefOf.Steel);
            if (!(hasSteamGeyser(map, new IntVec3(rect.CenterCell.x + 2, 0, rect.minZ + 5)) && hasSteamGeyser(map, new IntVec3(rect.CenterCell.x + 4, 0, rect.minZ + 5))))
                Spawn(tableButcher, new IntVec3(rect.CenterCell.x + 3, 0, rect.minZ + 5), map);

            Thing shelf = ThingMaker.MakeThing(DefDatabase<ThingDef>.AllDefs.FirstOrDefault(d => d.defName == "Shelf"), RimWorld.ThingDefOf.Steel);
            if (!hasSteamGeyser(map, new IntVec3(rect.CenterCell.x, 0, rect.minZ + 3)))
                Spawn(shelf, new IntVec3(rect.CenterCell.x + 1, 0, rect.minZ + 3), map);
            shelf = ThingMaker.MakeThing(DefDatabase<ThingDef>.AllDefs.FirstOrDefault(d => d.defName == "Shelf"), RimWorld.ThingDefOf.Steel);
            if (!hasSteamGeyser(map, new IntVec3(rect.CenterCell.x + 2, 0, rect.minZ + 3)))
                Spawn(shelf, new IntVec3(rect.CenterCell.x + 3, 0, rect.minZ + 3), map);

            foreach (IntVec3 c in rect)
            {
                if (c.InBounds(map))
                {
                    map.roofGrid.SetRoof(c, RoofDefOf.RoofConstructed);
                }
            }
        }
        public void ConstrutFoodWarehouse(Map map, CellRect rect)
        {
            foreach (IntVec3 c in rect.EdgeCells)
            {
                if (c.InBounds(map))
                {
                    Thing wall = ThingMaker.MakeThing(RimWorld.ThingDefOf.Wall, RimWorld.ThingDefOf.Steel);
                    Spawn(wall, c, map);
                }
            }
            for (int i = 1; i + rect.minZ < rect.maxZ;i++)
            {

                if (hasSteamGeyser(map, new IntVec3( rect.minX + 2, 0, rect.minZ + i)))
                {
                    continue;
                }
                Thing shelf = ThingMaker.MakeThing(DefDatabase<ThingDef>.AllDefs.FirstOrDefault(d => d.defName == "Shelf"), RimWorld.ThingDefOf.Steel);
                Spawn(shelf, new IntVec3(rect.minX + 3,0,rect.minZ + i), map);
                if (Rand.Range(0f, 1f) > 0.5f)
                {
                    Thing rawCorn = ThingMaker.MakeThing(DefDatabase<ThingDef>.AllDefs.FirstOrDefault(d => d.defName == "RawCorn"));
                    rawCorn.stackCount = (int)Rand.Range(1f, 70f);
                    GenPlace.TryPlaceThing(rawCorn, shelf.Position, map, ThingPlaceMode.Direct);
                }
                if (Rand.Range(0f, 1f) > 0.5f)
                {
                    Thing egg = ThingMaker.MakeThing(DefDatabase<ThingDef>.AllDefs.FirstOrDefault(d => d.defName == "EggChickenFertilized"));
                    egg.stackCount = (int)Rand.Range(1f, 10f);
                    GenPlace.TryPlaceThing(egg, shelf.Position, map, ThingPlaceMode.Direct);
                }
                if (Rand.Range(0f, 1f) > 0.5f)
                {
                    Thing mealFine =
                        ThingMaker.MakeThing(
                            DefDatabase<ThingDef>.AllDefs.FirstOrDefault(d => d.defName == "MealFine"));
                    mealFine.stackCount = (int)Rand.Range(1f, 8f);
                    GenPlace.TryPlaceThing(mealFine, shelf.Position, map, ThingPlaceMode.Direct);
                }
                if (Rand.Range(0f, 1f) > 0.5f)
                {
                    Thing mealFine =
                        ThingMaker.MakeThing(
                            DefDatabase<ThingDef>.AllDefs.FirstOrDefault(d => d.defName == "MealSimple"));
                    mealFine.stackCount = (int)Rand.Range(1f, 8f);
                    GenPlace.TryPlaceThing(mealFine, shelf.Position, map, ThingPlaceMode.Direct);
                }
            }
            Thing lamp = ThingMaker.MakeThing(DefDatabase<ThingDef>.AllDefs.FirstOrDefault(d => d.defName == "WallLamp")); 
            Spawn(lamp,new IntVec3(rect.minX + 1,0,rect.CenterCell.z),map,Rot4.West);
            Thing door = ThingMaker.MakeThing(RimWorld.ThingDefOf.Door, RimWorld.ThingDefOf.Steel);
            Spawn(door, new IntVec3(rect.minX, 0, rect.minZ + 3), map);
            door = ThingMaker.MakeThing(RimWorld.ThingDefOf.Door, RimWorld.ThingDefOf.Steel);
            Spawn(door, new IntVec3(rect.minX+2, 0, rect.minZ), map);

            foreach (IntVec3 c in rect)
            {
                if (c.InBounds(map))
                {
                    map.roofGrid.SetRoof(c, RoofDefOf.RoofConstructed);
                }
            }
        }
        public void ConstrutArmory(Map map, CellRect rect)
        {
            foreach (IntVec3 c in rect.EdgeCells)
            {
                if (c.InBounds(map))
                {
                    Thing wall = ThingMaker.MakeThing(RimWorld.ThingDefOf.Wall, RimWorld.ThingDefOf.Steel);
                    Spawn(wall, c, map);
                }
            }
            for (int i = 1; i + rect.minZ < rect.maxZ; i++)
            {

                if (hasSteamGeyser(map, new IntVec3(rect.minX + 2, 0, rect.minZ + i)))
                {
                    continue;
                }
                Thing shelf = ThingMaker.MakeThing(DefDatabase<ThingDef>.AllDefs.FirstOrDefault(d => d.defName == "Shelf"), RimWorld.ThingDefOf.Steel);
                Spawn(shelf, new IntVec3(rect.minX + 3, 0, rect.minZ + i), map);
                if (Rand.Range(0f, 1f) > 0.9f)
                {
                    Thing VC_Empire_Helmet = ThingMaker.MakeThing(DefDatabase<ThingDef>.AllDefs.FirstOrDefault(d => d.defName == "VC_Empire_Helmet"),RimWorld.ThingDefOf.Steel);
                    GenPlace.TryPlaceThing(VC_Empire_Helmet, shelf.Position, map, ThingPlaceMode.Direct);
                }
                if (Rand.Range(0f, 1f) > 0.9f)
                {
                    Thing VC_Empire_FlakPants = ThingMaker.MakeThing(DefDatabase<ThingDef>.AllDefs.FirstOrDefault(d => d.defName == "VC_Empire_FlakPants"));
                    GenPlace.TryPlaceThing(VC_Empire_FlakPants, shelf.Position, map, ThingPlaceMode.Direct);
                }
                if (Rand.Range(0f, 1f) > 0.8f)
                {
                    Thing VC_Empire_FlakPants = ThingMaker.MakeThing(DefDatabase<ThingDef>.AllDefs.FirstOrDefault(d => d.defName == "VC_Empire_Combat_Uniform"));
                    GenPlace.TryPlaceThing(VC_Empire_FlakPants, shelf.Position, map, ThingPlaceMode.Direct);
                }
                if (Rand.Range(0f, 1f) > 0.9f)
                {
                    Thing VC_HildeA = ThingMaker.MakeThing(DefDatabase<ThingDef>.AllDefs.FirstOrDefault(d => d.defName == "VC_HildeA"));
                    GenPlace.TryPlaceThing(VC_HildeA, shelf.Position, map, ThingPlaceMode.Direct);
                }
                if (Rand.Range(0f, 1f) > 0.9f)
                {
                    Thing VC_MagsA = ThingMaker.MakeThing(DefDatabase<ThingDef>.AllDefs.FirstOrDefault(d => d.defName == "VC_MagsA"));
                    GenPlace.TryPlaceThing(VC_MagsA, shelf.Position, map, ThingPlaceMode.Direct);
                }
            }
            Thing lamp = ThingMaker.MakeThing(DefDatabase<ThingDef>.AllDefs.FirstOrDefault(d => d.defName == "WallLamp"));
            Spawn(lamp, new IntVec3(rect.minX + 1, 0, rect.CenterCell.z), map, Rot4.West);
            Thing door = ThingMaker.MakeThing(RimWorld.ThingDefOf.Door, RimWorld.ThingDefOf.Steel);
            Spawn(door, new IntVec3(rect.maxX, 0, rect.minZ+3), map);

            foreach (IntVec3 c in rect)
            {
                if (c.InBounds(map))
                {
                    map.roofGrid.SetRoof(c, RoofDefOf.RoofConstructed);
                }
            }

        }
        public void ConstrutSeniorApartment(Map map, CellRect rect)
        {
            foreach (IntVec3 c in rect.EdgeCells)
            {
                if (c.InBounds(map))
                {
                    Thing wall = ThingMaker.MakeThing(RimWorld.ThingDefOf.Wall, RimWorld.ThingDefOf.Steel);
                    Spawn(wall, c, map);
                }
            }

            foreach (IntVec3 c in rect.Cells)
            {
                SetTerrain(map,c,TerrainDefOf.WoodPlankFloor);
            }

            Thing lamp = ThingMaker.MakeThing(DefDatabase<ThingDef>.AllDefs.FirstOrDefault(d => d.defName == "WallLamp"));
            Spawn(lamp, new IntVec3(rect.minX + 1, 0, rect.CenterCell.z), map, Rot4.West);
            Thing door = ThingMaker.MakeThing(RimWorld.ThingDefOf.Door, RimWorld.ThingDefOf.Steel);
            Spawn(door, new IntVec3(rect.maxX, 0,rect.CenterCell.z), map);
            Thing doubleBed = ThingMaker.MakeThing(DefDatabase<ThingDef>.AllDefs.FirstOrDefault(d => d.defName == "DoubleBed"), RimWorld.ThingDefOf.Steel);
            Spawn(doubleBed, new IntVec3(rect.minX + 1, 0, rect.minZ + 1), map,Rot4.North);
            Thing endTable = ThingMaker.MakeThing(DefDatabase<ThingDef>.AllDefs.FirstOrDefault(d => d.defName == "EndTable"), RimWorld.ThingDefOf.Steel);
            Spawn(endTable, new IntVec3(rect.minX + 3, 0, rect.minZ + 1), map, Rot4.North);
            Thing dresser = ThingMaker.MakeThing(DefDatabase<ThingDef>.AllDefs.FirstOrDefault(d => d.defName == "Dresser"), RimWorld.ThingDefOf.Steel);
            Spawn(dresser, new IntVec3(rect.minX + 2, 0, rect.maxZ - 1), map, Rot4.South);

            foreach (IntVec3 c in rect)
            {
                if (c.InBounds(map))
                {
                    map.roofGrid.SetRoof(c, RoofDefOf.RoofConstructed);
                }
            }


        }

        public void ConstructInfirmary(Map map, CellRect rect)
        {
            foreach (IntVec3 c in rect.EdgeCells)
            {
                if (c.InBounds(map))
                {
                    Thing wall = ThingMaker.MakeThing(RimWorld.ThingDefOf.Wall, RimWorld.ThingDefOf.Steel);
                    Spawn(wall, c, map);
                }
            }
            foreach (IntVec3 c in rect.Cells)
            {
                SetTerrain(map, c, DefDatabase<TerrainDef>.AllDefs.FirstOrDefault(d => d.defName == "SterileTile"));
            }
            Thing lamp = ThingMaker.MakeThing(DefDatabase<ThingDef>.AllDefs.FirstOrDefault(d => d.defName == "WallLamp"));
            Spawn(lamp, new IntVec3(rect.minX + 1, 0, rect.CenterCell.z), map, Rot4.West);
            Thing door = ThingMaker.MakeThing(RimWorld.ThingDefOf.Door, RimWorld.ThingDefOf.Steel);
            Spawn(door, new IntVec3(rect.maxX, 0, rect.CenterCell.z), map);
            Thing endTable = ThingMaker.MakeThing(DefDatabase<ThingDef>.AllDefs.FirstOrDefault(d => d.defName == "EndTable"), RimWorld.ThingDefOf.Steel);
            Spawn(endTable, new IntVec3(rect.minX + 2, 0, rect.minZ + 1), map,Rot4.North);
            endTable = ThingMaker.MakeThing(DefDatabase<ThingDef>.AllDefs.FirstOrDefault(d => d.defName == "EndTable"), RimWorld.ThingDefOf.Steel);
            Spawn(endTable, new IntVec3(rect.minX + 2, 0, rect.maxZ - 1), map,Rot4.South);
            Thing hospitalBed = ThingMaker.MakeThing(DefDatabase<ThingDef>.AllDefs.FirstOrDefault(d => d.defName == "HospitalBed"), RimWorld.ThingDefOf.Steel);
            Spawn(hospitalBed, new IntVec3(rect.minX + 1, 0, rect.minZ + 1), map, Rot4.North);
            hospitalBed = ThingMaker.MakeThing(DefDatabase<ThingDef>.AllDefs.FirstOrDefault(d => d.defName == "HospitalBed"), RimWorld.ThingDefOf.Steel);
            Spawn(hospitalBed, new IntVec3(rect.minX + 1, 0, rect.maxZ - 1), map, Rot4.South);
            hospitalBed = ThingMaker.MakeThing(DefDatabase<ThingDef>.AllDefs.FirstOrDefault(d => d.defName == "HospitalBed"), RimWorld.ThingDefOf.Steel);
            Spawn(hospitalBed, new IntVec3(rect.maxX - 1, 0, rect.minZ + 1), map, Rot4.North);
            hospitalBed = ThingMaker.MakeThing(DefDatabase<ThingDef>.AllDefs.FirstOrDefault(d => d.defName == "HospitalBed"), RimWorld.ThingDefOf.Steel);
            Spawn(hospitalBed, new IntVec3(rect.maxX - 1, 0, rect.maxZ - 1), map, Rot4.South);

            foreach (IntVec3 c in rect)
            {
                if (c.InBounds(map))
                {
                    map.roofGrid.SetRoof(c, RoofDefOf.RoofConstructed);
                }
            }
        }

        public void ConstructSeniorOffice(Map map, CellRect rect)
        {
            foreach (IntVec3 c in rect.EdgeCells)
            {
                if (c.InBounds(map))
                {
                    Thing wall = ThingMaker.MakeThing(RimWorld.ThingDefOf.Wall, RimWorld.ThingDefOf.Steel);
                    Spawn(wall, c, map);
                }
            }
            foreach (IntVec3 c in rect.Cells)
            {
                SetTerrain(map, c, TerrainDefOf.WoodPlankFloor);
            }
            Thing lamp = ThingMaker.MakeThing(DefDatabase<ThingDef>.AllDefs.FirstOrDefault(d => d.defName == "WallLamp"));
            Spawn(lamp, new IntVec3(rect.minX + 1, 0, rect.CenterCell.z), map, Rot4.West);
            Thing door = ThingMaker.MakeThing(RimWorld.ThingDefOf.Door, RimWorld.ThingDefOf.Steel);
            Spawn(door, new IntVec3(rect.maxX, 0, rect.CenterCell.z), map);
            Thing table = ThingMaker.MakeThing(RimWorld.ThingDefOf.Table1x2c, RimWorld.ThingDefOf.Steel);
            Spawn(table, new IntVec3(rect.minX+2, 0, rect.minZ + 2), map,Rot4.West);
            Thing sofa = ThingMaker.MakeThing(DefDatabase<ThingDef>.AllDefs.FirstOrDefault(d => d.defName == "Armchair"), RimWorld.ThingDefOf.Cloth);
            Spawn(sofa, new IntVec3(rect.minX + 2, 0, rect.minZ + 1), map, Rot4.North);
            sofa = ThingMaker.MakeThing(DefDatabase<ThingDef>.AllDefs.FirstOrDefault(d => d.defName == "Armchair"), RimWorld.ThingDefOf.Cloth);
            Spawn(sofa, new IntVec3(rect.minX + 2, 0, rect.minZ + 3), map, Rot4.South);
            Thing bookcaseSmall = ThingMaker.MakeThing(DefDatabase<ThingDef>.AllDefs.FirstOrDefault(d => d.defName == "BookcaseSmall"), RimWorld.ThingDefOf.Cloth);
            Spawn(bookcaseSmall, new IntVec3(rect.minX + 1, 0, rect.minZ + 1), map, Rot4.South);
            Thing plantPot_Bonsai = ThingMaker.MakeThing(DefDatabase<ThingDef>.AllDefs.FirstOrDefault(d => d.defName == "PlantPot_Bonsai"), RimWorld.ThingDefOf.Cloth);
            Spawn(plantPot_Bonsai, new IntVec3(rect.maxX - 1, 0, rect.minZ + 2), map);

            foreach (IntVec3 c in rect)
            {
                if (c.InBounds(map))
                {
                    map.roofGrid.SetRoof(c, RoofDefOf.RoofConstructed);
                }
            }
        }
        public void ConstructCommonOffice(Map map, CellRect rect)
        {
            foreach (IntVec3 c in rect.EdgeCells)
            {
                if (c.InBounds(map))
                {
                    Thing wall = ThingMaker.MakeThing(RimWorld.ThingDefOf.Wall, RimWorld.ThingDefOf.Steel);
                    Spawn(wall, c, map);
                }
            }
            Thing lamp = ThingMaker.MakeThing(DefDatabase<ThingDef>.AllDefs.FirstOrDefault(d => d.defName == "WallLamp"));
            Spawn(lamp, new IntVec3(rect.minX + 1, 0, rect.CenterCell.z), map, Rot4.West);
            lamp = ThingMaker.MakeThing(DefDatabase<ThingDef>.AllDefs.FirstOrDefault(d => d.defName == "WallLamp"));
            Spawn(lamp, new IntVec3(rect.maxX - 1, 0, rect.CenterCell.z), map, Rot4.East);
            Thing door = ThingMaker.MakeThing(RimWorld.ThingDefOf.Door, RimWorld.ThingDefOf.Steel);
            Spawn(door, new IntVec3(rect.minX+8, 0, rect.maxZ), map);
            for (int i = 3; i + rect.minX < rect.maxX;i+=2)
            {
                for (int j = 1; j + rect.minZ < rect.maxZ; j += 3)
                {
                    Thing table = ThingMaker.MakeThing(RimWorld.ThingDefOf.Table1x2c, RimWorld.ThingDefOf.Steel);
                    Spawn(table, new IntVec3(rect.minX + i, 0, rect.minZ + j), map);
                    Thing chair = ThingMaker.MakeThing(RimWorld.ThingDefOf.DiningChair, RimWorld.ThingDefOf.Steel);
                    Spawn(chair, new IntVec3(rect.minX + i - 1, 0, rect.minZ + j), map,Rot4.East);
                }
            }

            foreach (IntVec3 c in rect)
            {
                if (c.InBounds(map))
                {
                    map.roofGrid.SetRoof(c, RoofDefOf.RoofConstructed);
                }
            }
        }

        public void ConstructEntertainmentRoom(Map map, CellRect rect)
        {
            foreach (IntVec3 c in rect.EdgeCells)
            {
                if (c.InBounds(map))
                {
                    Thing wall = ThingMaker.MakeThing(RimWorld.ThingDefOf.Wall, RimWorld.ThingDefOf.Steel);
                    Spawn(wall, c, map);
                }
            }
            Thing lamp = ThingMaker.MakeThing(RimWorld.ThingDefOf.StandingLamp);
            Spawn(lamp, rect.CenterCell, map);
            Thing door = ThingMaker.MakeThing(RimWorld.ThingDefOf.Door, RimWorld.ThingDefOf.Steel);
            Spawn(door, new IntVec3(rect.maxX, 0, rect.CenterCell.z), map);
            Thing billiardsTable = ThingMaker.MakeThing(DefDatabase<ThingDef>.AllDefs.FirstOrDefault(d => d.defName == "BilliardsTable"), RimWorld.ThingDefOf.Steel);
            Spawn(billiardsTable, new IntVec3(rect.maxX-3, 0, rect.minZ + 3), map);
            Thing pokerTable = ThingMaker.MakeThing(DefDatabase<ThingDef>.AllDefs.FirstOrDefault(d => d.defName == "PokerTable"), RimWorld.ThingDefOf.Steel);
            Spawn(pokerTable, new IntVec3(rect.maxX - 3, 0, rect.CenterCell.z + 2), map);
            Thing couch = ThingMaker.MakeThing(DefDatabase<ThingDef>.AllDefs.FirstOrDefault(d => d.defName == "Couch"), RimWorld.ThingDefOf.Cloth);
            Spawn(couch, new IntVec3(rect.maxX - 3, 0, rect.CenterCell.z + 1), map, Rot4.North);
            couch = ThingMaker.MakeThing(DefDatabase<ThingDef>.AllDefs.FirstOrDefault(d => d.defName == "Couch"), RimWorld.ThingDefOf.Cloth);
            Spawn(couch, new IntVec3(rect.maxX - 1, 0, rect.CenterCell.z + 2), map, Rot4.West);

            foreach (IntVec3 c in rect)
            {
                if (c.InBounds(map))
                {
                    map.roofGrid.SetRoof(c, RoofDefOf.RoofConstructed);
                }
            }

        }

        public void ConstructDiningRoom(Map map, CellRect rect)
        {
            foreach (IntVec3 c in rect.EdgeCells)
            {
                if (c.InBounds(map))
                {
                    Thing wall = ThingMaker.MakeThing(RimWorld.ThingDefOf.Wall, RimWorld.ThingDefOf.Steel);
                    Spawn(wall, c, map);
                }
            }
            Thing lamp = ThingMaker.MakeThing(RimWorld.ThingDefOf.StandingLamp);
            Spawn(lamp, rect.CenterCell, map);
            Thing door = ThingMaker.MakeThing(RimWorld.ThingDefOf.Door, RimWorld.ThingDefOf.Steel);
            Spawn(door, new IntVec3(rect.CenterCell.x, 0,rect.minZ), map);
            Thing table = ThingMaker.MakeThing(RimWorld.ThingDefOf.Table1x2c, RimWorld.ThingDefOf.Steel);
            Spawn(table, new IntVec3(rect.minX + 1, 0, rect.minZ + 1), map,Rot4.East);
            table = ThingMaker.MakeThing(RimWorld.ThingDefOf.Table1x2c, RimWorld.ThingDefOf.Steel);
            Spawn(table, new IntVec3(rect.minX + 3, 0, rect.minZ + 1), map, Rot4.East);
            Thing chair = ThingMaker.MakeThing(RimWorld.ThingDefOf.DiningChair, RimWorld.ThingDefOf.Steel);
            Spawn(chair, new IntVec3(rect.minX + 1, 0, rect.minZ + 2), map,Rot4.South);
            chair = ThingMaker.MakeThing(RimWorld.ThingDefOf.DiningChair, RimWorld.ThingDefOf.Steel);
            Spawn(chair, new IntVec3(rect.minX + 2, 0, rect.minZ + 2), map, Rot4.South);
            chair = ThingMaker.MakeThing(RimWorld.ThingDefOf.DiningChair, RimWorld.ThingDefOf.Steel);
            Spawn(chair, new IntVec3(rect.minX + 3, 0, rect.minZ + 2), map, Rot4.South);
            chair = ThingMaker.MakeThing(RimWorld.ThingDefOf.DiningChair, RimWorld.ThingDefOf.Steel);
            Spawn(chair, new IntVec3(rect.minX + 4, 0, rect.minZ + 2), map, Rot4.South);

            foreach (IntVec3 c in rect)
            {
                if (c.InBounds(map))
                {
                    map.roofGrid.SetRoof(c, RoofDefOf.RoofConstructed);
                }
            }
        }
        public void ConstructDormitory(Map map, CellRect rect)
        {
            foreach (IntVec3 c in rect.EdgeCells)
            {
                if (c.InBounds(map))
                {
                    Thing wall = ThingMaker.MakeThing(RimWorld.ThingDefOf.Wall, RimWorld.ThingDefOf.Steel);
                    Spawn(wall, c, map);
                }
            }
            Thing lamp = ThingMaker.MakeThing(DefDatabase<ThingDef>.AllDefs.FirstOrDefault(d => d.defName == "WallLamp"));
            Spawn(lamp, new IntVec3(rect.minX + 1, 0, rect.CenterCell.z), map, Rot4.West);
            Thing door = ThingMaker.MakeThing(RimWorld.ThingDefOf.Door, RimWorld.ThingDefOf.Steel);
            Spawn(door, new IntVec3(rect.minX + 3, 0, rect.minZ), map);
            Thing dresser = ThingMaker.MakeThing(DefDatabase<ThingDef>.AllDefs.FirstOrDefault(d => d.defName == "Dresser"), RimWorld.ThingDefOf.Steel);
            Spawn(dresser, new IntVec3(rect.minX + 1, 0, rect.minZ + 4), map, Rot4.East);
            Thing bed = ThingMaker.MakeThing(RimWorld.ThingDefOf.Bed, RimWorld.ThingDefOf.Steel);
            Spawn(bed, new IntVec3(rect.minX + 1, 0, rect.minZ + 1), map, Rot4.East);
            bed = ThingMaker.MakeThing(RimWorld.ThingDefOf.Bed, RimWorld.ThingDefOf.Steel);
            Spawn(bed, new IntVec3(rect.minX + 1, 0, rect.minZ + 2), map, Rot4.East);
            bed = ThingMaker.MakeThing(RimWorld.ThingDefOf.Bed, RimWorld.ThingDefOf.Steel);
            Spawn(bed, new IntVec3(rect.minX + 1, 0, rect.minZ + 5), map, Rot4.East);
            bed = ThingMaker.MakeThing(RimWorld.ThingDefOf.Bed, RimWorld.ThingDefOf.Steel);
            Spawn(bed, new IntVec3(rect.minX + 1, 0, rect.minZ + 6), map, Rot4.East);
            bed = ThingMaker.MakeThing(RimWorld.ThingDefOf.Bed, RimWorld.ThingDefOf.Steel);
            Spawn(bed, new IntVec3(rect.maxX - 1, 0, rect.minZ + 5), map, Rot4.West);
            bed = ThingMaker.MakeThing(RimWorld.ThingDefOf.Bed, RimWorld.ThingDefOf.Steel);
            Spawn(bed, new IntVec3(rect.maxX - 1, 0, rect.minZ + 6), map, Rot4.West);

            foreach (IntVec3 c in rect)
            {
                if (c.InBounds(map))
                {
                    map.roofGrid.SetRoof(c, RoofDefOf.RoofConstructed);
                }
            }
        }

        public void ConstructDormitoryMirror(Map map, CellRect rect)
        {
            foreach (IntVec3 c in rect.EdgeCells)
            {
                if (c.InBounds(map))
                {
                    Thing wall = ThingMaker.MakeThing(RimWorld.ThingDefOf.Wall, RimWorld.ThingDefOf.Steel);
                    Spawn(wall, c, map);
                }
            }
            Thing lamp = ThingMaker.MakeThing(DefDatabase<ThingDef>.AllDefs.FirstOrDefault(d => d.defName == "WallLamp"));
            Spawn(lamp, new IntVec3(rect.minX + 1, 0, rect.CenterCell.z), map, Rot4.West);
            Thing door = ThingMaker.MakeThing(RimWorld.ThingDefOf.Door, RimWorld.ThingDefOf.Steel);
            Spawn(door, new IntVec3(rect.minX + 2, 0, rect.minZ), map);
            Thing dresser = ThingMaker.MakeThing(DefDatabase<ThingDef>.AllDefs.FirstOrDefault(d => d.defName == "Dresser"), RimWorld.ThingDefOf.Steel);
            Spawn(dresser, new IntVec3(rect.maxX - 1, 0, rect.minZ + 3), map, Rot4.West);
            Thing bed = ThingMaker.MakeThing(RimWorld.ThingDefOf.Bed, RimWorld.ThingDefOf.Steel);
            Spawn(bed, new IntVec3(rect.maxX - 1, 0, rect.minZ + 1), map, Rot4.West);
            bed = ThingMaker.MakeThing(RimWorld.ThingDefOf.Bed, RimWorld.ThingDefOf.Steel);
            Spawn(bed, new IntVec3(rect.maxX - 1, 0, rect.minZ + 2), map, Rot4.West);
            bed = ThingMaker.MakeThing(RimWorld.ThingDefOf.Bed, RimWorld.ThingDefOf.Steel);
            Spawn(bed, new IntVec3(rect.minX + 1, 0, rect.minZ + 5), map, Rot4.East);
            bed = ThingMaker.MakeThing(RimWorld.ThingDefOf.Bed, RimWorld.ThingDefOf.Steel);
            Spawn(bed, new IntVec3(rect.minX + 1, 0, rect.minZ + 6), map, Rot4.East);
            bed = ThingMaker.MakeThing(RimWorld.ThingDefOf.Bed, RimWorld.ThingDefOf.Steel);
            Spawn(bed, new IntVec3(rect.maxX - 1, 0, rect.minZ + 5), map, Rot4.West);
            bed = ThingMaker.MakeThing(RimWorld.ThingDefOf.Bed, RimWorld.ThingDefOf.Steel);
            Spawn(bed, new IntVec3(rect.maxX - 1, 0, rect.minZ + 6), map, Rot4.West);

            foreach (IntVec3 c in rect)
            {
                if (c.InBounds(map))
                {
                    map.roofGrid.SetRoof(c, RoofDefOf.RoofConstructed);
                }
            }
        }

        public void ConstructPowerStation(Map map, CellRect rect)
        {
            foreach (IntVec3 c in rect.EdgeCells)
            {
                if (c.InBounds(map))
                {
                    Thing wall = ThingMaker.MakeThing(RimWorld.ThingDefOf.Wall, RimWorld.ThingDefOf.Steel);
                    Spawn(wall, c, map);
                }
            }
            Thing door = ThingMaker.MakeThing(RimWorld.ThingDefOf.Door, RimWorld.ThingDefOf.Steel);
            Spawn(door, new IntVec3(rect.maxX, 0, rect.minZ+1), map);
            Thing ragniteGenerator = ThingMaker.MakeThing(ThingDefOf.VC_RagniteGenerator);
            CompRefuelable refuel = ragniteGenerator.TryGetComp<CompRefuelable>();
            refuel.Refuel(Rand.Range(2f, 10f));
            Spawn(ragniteGenerator, new IntVec3(rect.minX+1, 0, rect.maxZ - 1), map);
            ragniteGenerator = ThingMaker.MakeThing(ThingDefOf.VC_RagniteGenerator);
            refuel = ragniteGenerator.TryGetComp<CompRefuelable>();
            refuel.Refuel(Rand.Range(2f, 10f));
            Spawn(ragniteGenerator, new IntVec3(rect.minX + 1, 0, rect.maxZ - 2), map);
            ragniteGenerator = ThingMaker.MakeThing(ThingDefOf.VC_RagniteGenerator);
            refuel = ragniteGenerator.TryGetComp<CompRefuelable>();
            refuel.Refuel(Rand.Range(2f, 10f));
            Spawn(ragniteGenerator, new IntVec3(rect.minX + 2, 0, rect.maxZ - 1), map);
            ragniteGenerator = ThingMaker.MakeThing(ThingDefOf.VC_RagniteGenerator);
            refuel = ragniteGenerator.TryGetComp<CompRefuelable>();
            refuel.Refuel(Rand.Range(2f, 10f));
            Spawn(ragniteGenerator, new IntVec3(rect.minX + 2, 0, rect.maxZ - 2), map);
            foreach (IntVec3 c in rect)
            {
                if (c.InBounds(map))
                {
                    map.roofGrid.SetRoof(c, RoofDefOf.RoofConstructed);
                }
            }
        }

        public void ConstructLiquidRagniteWarehouse(Map map, CellRect rect)
        {
            foreach (IntVec3 c in rect.EdgeCells)
            {
                if (c.InBounds(map))
                {
                    Thing wall = ThingMaker.MakeThing(RimWorld.ThingDefOf.Wall, RimWorld.ThingDefOf.Steel);
                    Spawn(wall, c, map);
                }
            }
            Thing door = ThingMaker.MakeThing(RimWorld.ThingDefOf.Door, RimWorld.ThingDefOf.Steel);
            Spawn(door, new IntVec3(rect.CenterCell.x, 0, rect.maxZ), map);
            Thing lamp = ThingMaker.MakeThing(DefDatabase<ThingDef>.AllDefs.FirstOrDefault(d => d.defName == "WallLamp"));
            Spawn(lamp, new IntVec3(rect.CenterCell.x, 0, rect.minZ + 1), map, Rot4.South);

            for (int i = 2; i < 5; i++)
            {
                for (int j = 1; j  < 5; j++)
                {
                    Thing shelfSmall = ThingMaker.MakeThing(DefDatabase<ThingDef>.AllDefs.FirstOrDefault(d => d.defName == "ShelfSmall"), RimWorld.ThingDefOf.Steel);
                    Spawn(shelfSmall,new IntVec3(j+rect.minX,0,i+ rect.minZ),map);
                    if (Rand.Range(0f, 1f) > 0.3f)
                    {
                        Thing lr = ThingMaker.MakeThing(ThingDefOf.VC_LiquidRagnite);
                        lr.stackCount = (int)Rand.Range(1f, 15f);
                        GenPlace.TryPlaceThing(lr, shelfSmall.Position, map, ThingPlaceMode.Direct);
                    }
                }
            }

            for (int i = 1; i < 5; i++)
            {
                for (int j = 1; j < 4; j++)
                {
                    Thing shelfSmall = ThingMaker.MakeThing(DefDatabase<ThingDef>.AllDefs.FirstOrDefault(d => d.defName == "ShelfSmall"), RimWorld.ThingDefOf.Steel);
                    Spawn(shelfSmall, new IntVec3(rect.maxX-j, 0, i + rect.minZ), map);
                }
            }

            foreach (IntVec3 c in rect)
            {
                if (c.InBounds(map))
                {
                    map.roofGrid.SetRoof(c, RoofDefOf.RoofConstructed);
                }
            }
        }

        public void ConstructRagniteWarehouse(Map map, CellRect rect)
        {
            foreach (IntVec3 c in rect.EdgeCells)
            {
                if (c.InBounds(map))
                {
                    Thing wall = ThingMaker.MakeThing(RimWorld.ThingDefOf.Wall, RimWorld.ThingDefOf.Steel);
                    Spawn(wall, c, map);
                }
            }
            Thing door = ThingMaker.MakeThing(RimWorld.ThingDefOf.Door, RimWorld.ThingDefOf.Steel);
            Spawn(door, new IntVec3(rect.CenterCell.x, 0, rect.maxZ), map);
            Thing lamp = ThingMaker.MakeThing(DefDatabase<ThingDef>.AllDefs.FirstOrDefault(d => d.defName == "WallLamp"));
            Spawn(lamp, new IntVec3(rect.CenterCell.x, 0, rect.minZ + 1), map, Rot4.South);
            for (int i = 1; i < 5; i++)
            {
                for (int j = 1; j < 11; j++)
                {
                    if (j == 6)
                    {
                        continue;
                    }
                    Thing shelfSmall = ThingMaker.MakeThing(DefDatabase<ThingDef>.AllDefs.FirstOrDefault(d => d.defName == "ShelfSmall"),RimWorld.ThingDefOf.Steel);
                    Spawn(shelfSmall, new IntVec3(rect.minX + j, 0, i + rect.minZ), map);
                    if (Rand.Range(0f, 1f) > 0.3f)
                    {
                        Thing ragnite = ThingMaker.MakeThing(ThingDefOf.Ragnite);
                        ragnite.stackCount = (int)Rand.Range(10f, 70f);
                        GenPlace.TryPlaceThing(ragnite, shelfSmall.Position, map, ThingPlaceMode.Direct);
                    }
                }
            }

            foreach (IntVec3 c in rect)
            {
                if (c.InBounds(map))
                {
                    map.roofGrid.SetRoof(c, RoofDefOf.RoofConstructed);
                }
            }
        }

        public void ConstructElectricWire(Map map, CellRect rect)
        {
            foreach (IntVec3 c in rect.EdgeCells)
            {
                if (c.InBounds(map))
                {
                    Thing hc = ThingMaker.MakeThing(RimWorld.ThingDefOf.HiddenConduit);
                    Spawn(hc, c, map);
                }
            }

            for (int i = 7; i + rect.minZ < rect.maxZ; i += 7)
            {
                for (int j = 1; j + rect.minX < rect.maxX; j++)
                {
                    Thing hc = ThingMaker.MakeThing(RimWorld.ThingDefOf.HiddenConduit);
                    Spawn(hc, new IntVec3(j+rect.minX,0,i+rect.minZ), map);
                }
            }

        }
        protected override void ScatterAt(IntVec3 c, Map map, GenStepParams parms, int stackCount = 1)
        {
            CellRect SettlementRect = new CellRect(c.x - SettlementSize / 2, c.z - SettlementSize / 2, SettlementSize, SettlementSize);
            CellRect WallRect = new CellRect(c.x - SettlementSize / 2, c.z - SettlementSize / 2, WallSize, WallSize);
            Faction faction = map.ParentFaction;
            SettlementRect.ClipInsideMap(map);
            ClearMap(map, SettlementRect);
            ConstrutExternalWall(map, WallRect);
            int whistleSize = 7;
            CellRect WhistleRect = new CellRect(WallRect.minX, WallRect.maxZ + 3, whistleSize, whistleSize);
            ConstrutWhistle(map, WhistleRect);
            WhistleRect = new CellRect(WallRect.minX + 12, WallRect.maxZ + 3, whistleSize, whistleSize);
            ConstrutWhistle(map, WhistleRect);
            WhistleRect = new CellRect(WallRect.maxX + 3, WallRect.minZ, whistleSize, whistleSize);
            ConstrutWhistle(map, WhistleRect);
            WhistleRect = new CellRect(WallRect.maxX + 3, WallRect.minZ + 12, whistleSize, whistleSize);
            ConstrutWhistle(map, WhistleRect);
            CellRect FactoryRect = new CellRect(WallRect.minX + 25, WallRect.minZ + 27, 19, 19);
            ConstrutFactory(map, FactoryRect);
            CellRect KitchenRect = new CellRect(WallRect.minX + 12, WallRect.minZ + 27, 11, 7);
            ConstrutKitchen(map, KitchenRect);
            CellRect FoodWarehouseRect = new CellRect(WallRect.minX + 12, WallRect.minZ + 33, 6, 13);
            ConstrutFoodWarehouse(map, FoodWarehouseRect);
            CellRect ArmoryRect = new CellRect(WallRect.minX + 17, WallRect.minZ + 33, 6, 13);
            ConstrutArmory(map, ArmoryRect);
            CellRect SeniorApartmentRect = new CellRect(WallRect.minX + 3, WallRect.minZ + 27, 5, 7);
            ConstrutSeniorApartment(map, SeniorApartmentRect);
            SeniorApartmentRect = new CellRect(WallRect.minX + 3, WallRect.minZ + 33, 5, 7);
            ConstrutSeniorApartment(map, SeniorApartmentRect);
            CellRect InfirmaryRect = new CellRect(WallRect.minX + 3, WallRect.maxZ - 10, 5, 7);
            ConstructInfirmary(map, InfirmaryRect);
            CellRect EntertainmentRoomRect = new CellRect(WallRect.minX + 8, WallRect.minZ + 10, 8,13);
            ConstructEntertainmentRoom(map, EntertainmentRoomRect);
            CellRect SeniorOfficeRect = new CellRect(WallRect.minX + 3, WallRect.minZ + 10, 6, 7);
            ConstructSeniorOffice(map, SeniorOfficeRect);
            SeniorOfficeRect = new CellRect(WallRect.minX + 3, WallRect.minZ + 16, 6, 7);
            ConstructSeniorOffice(map, SeniorOfficeRect);
            CellRect CommonOfficeRect = new CellRect(WallRect.minX + 3, WallRect.minZ + 3, 13, 8);
            ConstructCommonOffice(map, CommonOfficeRect);

            CellRect DiningRoomRect = new CellRect(WallRect.minX + 19, WallRect.minZ + 11, 11, 5);
            ConstructDiningRoom(map, DiningRoomRect);
            DiningRoomRect = new CellRect(WallRect.minX + 33, WallRect.minZ + 11, 11, 5);
            ConstructDiningRoom(map, DiningRoomRect);

            CellRect DormitoryRect = new CellRect(WallRect.minX + 19, WallRect.minZ + 15, 6, 8);
            ConstructDormitory(map, DormitoryRect);
            DormitoryRect = new CellRect(WallRect.minX + 24, WallRect.minZ + 15, 6, 8);
            ConstructDormitoryMirror(map, DormitoryRect);
            DormitoryRect = new CellRect(WallRect.minX + 33, WallRect.minZ + 15, 6, 8);
            ConstructDormitory(map, DormitoryRect);
            DormitoryRect = new CellRect(WallRect.minX + 38, WallRect.minZ + 15, 6, 8);
            ConstructDormitoryMirror(map, DormitoryRect);

            CellRect LiquidRagniteWarehouseRect = new CellRect(WallRect.minX + 20, WallRect.minZ + 3, 10, 6);
            ConstructLiquidRagniteWarehouse(map, LiquidRagniteWarehouseRect);

            CellRect RagniteWarehouseRect = new CellRect(WallRect.minX + 32, WallRect.minZ + 3, 12, 6);
            ConstructRagniteWarehouse(map, RagniteWarehouseRect);

            CellRect PowerStationRect = new CellRect(WallRect.minX + 17, WallRect.minZ + 3, 4, 6);
            ConstructPowerStation(map, PowerStationRect);

            ConstructElectricWire(map, WallRect);

            map.powerNetManager.UpdatePowerNetsAndConnections_First();//更新电网
            if (!this.clearBuildingFaction)
                return;
            foreach (Building building in map.listerThings.GetThingsOfType<Building>())
            {
                if (!(building is Building_Turret) && building.Faction == faction)
                    building.SetFaction((Faction)null, (Pawn)null);
            }
        }
    }
}
