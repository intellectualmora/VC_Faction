using RimWorld;

namespace VC_Faction
{
    [DefOf]
    public static class FactionDefOf
    {
        public static FactionDef VC_Empire;
        public static FactionDef VC_Federation;
        public static FactionDef VC_Gallia;
        static FactionDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(FactionDefOf));
    }
}