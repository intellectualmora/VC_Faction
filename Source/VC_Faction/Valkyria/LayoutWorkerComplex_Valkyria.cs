using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Valkyria
{
    public class LayoutWorkerComplex_Valkyria : LayoutWorkerComplex
    {
        private const string ValkyriaCasketOpenedSignal = "ValkyriaCasketOpened";
        private const string ValkyriaCasketUnfoggedSignal = "ValkyriaCasketUnfogged";
        private readonly List<CellRect> tmpAllRoomRects = new List<CellRect>();

        public LayoutWorkerComplex_Valkyria(LayoutDef def)
          : base(def)
        {
        }

        public override Faction GetFixedHostileFactionForThreats() => Faction.OfMechanoids;

        protected override void PreSpawnThreats(
          List<List<CellRect>> rooms,
          Map map,
          List<Thing> allSpawnedThings)
        {
            base.PreSpawnThreats(rooms, map, allSpawnedThings);
            this.tmpAllRoomRects.Clear();
            this.tmpAllRoomRects.AddRange(rooms.SelectMany<List<CellRect>, CellRect>((Func<List<CellRect>, IEnumerable<CellRect>>)(r => (IEnumerable<CellRect>)r)));
            CellRect bounds = this.tmpAllRoomRects[0];
            for (int index = 0; index < this.tmpAllRoomRects.Count; ++index)
                bounds = bounds.Encapsulate(this.tmpAllRoomRects[index]);
            bool flag = false;
            bool flag2 = false;
            foreach (CellRect room in (IEnumerable<CellRect>)this.tmpAllRoomRects.OrderBy<CellRect, float>(new Func<CellRect, float>(OrderRoomsBy)))
            {
                Building_AncientCryptosleepPod casket;
                Building_Crate crate;
                if (!flag)
                {
                    if (this.TryPlaceValkyria(room, map, out casket))
                    {
                        allSpawnedThings.Add((Thing)casket);
                        flag = true;
                    }
                }
                if (!flag2)
                {
                    if (this.TryPlaceSpear(room, map, out crate))
                    {
                        allSpawnedThings.Add((Thing)crate);
                        flag2 = true;
                    }
                }
            }
            if (!flag)
                Log.Error("Failed to place Valkyria in ancient Valkyria complex.");
            this.tmpAllRoomRects.Clear();

            float OrderRoomsBy(CellRect r) => r.Contains(bounds.CenterCell) ? 0.0f : r.CenterCell.DistanceTo(bounds.CenterCell);
        }

        private bool TryPlaceValkyria(
          CellRect room,
          Map map,
          out Building_AncientCryptosleepPod casket)
        {
            foreach (IntVec3 intVec3 in room.Cells.InRandomOrder<IntVec3>())
            {
                if (CanPlaceCasketAt(intVec3))
                {
                    casket = (Building_AncientCryptosleepPod)GenSpawn.Spawn(ThingDefOf.AncientCryptosleepPod, intVec3, map);
                    casket.openedSignal = "ValkyriaCasketOpened" + (object)Find.UniqueIDsManager.GetNextSignalTagID();
                    Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(ValkyriaKindDefOf.Valkyria_Basic, Faction.OfAncients, certainlyBeenInCryptosleep: true, forceDead: false));
                    pawn.relations.hidePawnRelations = false;
                    casket.TryAcceptThing((Thing)pawn, false);
                    ScatterDebrisUtility.ScatterFilthAroundThing((Thing)casket, map, ThingDefOf.Filth_MachineBits);
                    return true;
                }
            }
            casket = (Building_AncientCryptosleepPod)null;
            return false;

            bool CanPlaceCasketAt(IntVec3 cell)
            {
                foreach (IntVec3 c in GenAdj.OccupiedRect(cell, Rot4.North, ThingDefOf.AncientCryptosleepPod.Size).ExpandedBy(1))
                {
                    if (c.GetEdifice(map) != null)
                        return false;
                }
                return true;
            }
        }

        private bool TryPlaceSpear(
            CellRect room,
            Map map,
            out Building_Crate crate)
        {
            foreach (IntVec3 intVec3 in room.Cells.InRandomOrder<IntVec3>())
            {
                if (CanPlaceCratetAt(intVec3))
                {
                    Thing spear;
                    Random rd = new Random();
                    crate = (Building_Crate)GenSpawn.Spawn(ThingDefOf.AncientSecurityCrate, intVec3, map);
                    if(rd.Next(0,2)==0){
                        spear = ThingMaker.MakeThing(ValkyriaThingDefOf.Spear_Valkyria);
                    }
                    else
                    {
                        spear = ThingMaker.MakeThing(ValkyriaThingDefOf.Spear_Valkyria_Rie);
                    }
                    crate.TryAcceptThing((Thing)spear, false);
                    ScatterDebrisUtility.ScatterFilthAroundThing((Thing)crate, map, ThingDefOf.Filth_MachineBits);
                    return true;
                }
            }
            crate = (Building_Crate)null;
            return false;

            bool CanPlaceCratetAt(IntVec3 cell)
            {
                foreach (IntVec3 c in GenAdj.OccupiedRect(cell, Rot4.North, ThingDefOf.AncientSecurityCrate.Size).ExpandedBy(1))
                {
                    if (c.GetEdifice(map) != null)
                        return false;
                }
                return true;
            }
        }
    }

}
