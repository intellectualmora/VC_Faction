using RimWorld;
using Verse;

namespace VC_Faction
{
    [DefOf]
    public static class JobDefOf
    {
        public static JobDef TalkWithPawn;
        public static JobDef VC_UseRadio;
        public static JobDef FillRagniteReactor;
        public static JobDef TakeLiquidRagniteOutOfReactor;
        public static JobDef FillRagniteReactorBulk;
        public static JobDef VC_ManTurret;
        static JobDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(JobDefOf));
    }
}