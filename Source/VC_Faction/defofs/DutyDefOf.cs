using RimWorld;
using Verse;
using Verse.AI;

namespace VC_Faction
{
    [DefOf]
    public static class DutyDefOf
    {
        public static DutyDef VC_ManClosestTurret;
        static DutyDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(DutyDefOf));
    }
}