using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace VC_Faction
{
    public class WorkGiver_TakeLiquidRagniteOutOfReactor : WorkGiver_Scanner
    {
        public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(ThingDefOf.VC_RagniteReactor);

        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            List<Thing> thingList = pawn.Map.listerThings.ThingsOfDef(ThingDefOf.VC_RagniteReactor);
            for (int index = 0; index < thingList.Count; ++index)
            {
                if (((Building_RagniteReactor)thingList[index]).Refined)
                    return false;
            }
            return true;
        }

        public override PathEndMode PathEndMode => PathEndMode.Touch;

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false) => t is Building_RagniteReactor fermentingBarrel && fermentingBarrel.Refined && !t.IsBurning() && !t.IsForbidden(pawn) && pawn.CanReserve((LocalTargetInfo)t, ignoreOtherReservations: forced);

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false) => JobMaker.MakeJob(JobDefOf.TakeLiquidRagniteOutOfReactor, (LocalTargetInfo)t);
    }
}