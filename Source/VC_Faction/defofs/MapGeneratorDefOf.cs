using RimWorld;
using Verse;

namespace VC_Faction
{
    [DefOf]
    public static class MapGeneratorDefOf
    {
        public static MapGeneratorDef VC_Relic_Underground0;
        static MapGeneratorDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(MapGeneratorDefOf));
    }
}