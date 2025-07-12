using RimWorld;
using Verse;

namespace VC_Faction
{
    [DefOf]
    public static class FleckDefOf
    {
        public static FleckDef VC_RagnaidFieldActivation;
        static FleckDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(RimWorld.FleckDefOf));
    }
}