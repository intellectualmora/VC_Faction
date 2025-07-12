using RimWorld;
using Verse;

namespace VC_Faction
{
    [DefOf]
    public static class ThingDefOf
    {
        public static ThingDef VC_RagnaidProjector;
        public static ThingDef VC_RagniteReactor;
        public static ThingDef VC_RagniteGenerator;
        public static ThingDef VC_Gallia_Flag;
        public static ThingDef VC_Federation_Flag;
        public static ThingDef VC_Empire_Flag;
        public static ThingDef VC_Radio;
        public static ThingDef VC_LiquidRagnite;
        public static ThingDef Ragnite;
        public static ThingDef VC_Stair_Up;
        public static ThingDef VC_Stair_Down;
        public static ThingDef VC_Portablemortar;
        static ThingDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(ThingDefOf));
    }
}