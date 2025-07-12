using RimWorld;
using Verse;

namespace VC_Faction
{
    [DefOf]
    public static class PawnGroupKindDefOf
    {
        public static PawnGroupKindDef VC_Gallia_Militia;
        public static PawnGroupKindDef VC_Villagers;
        public static PawnGroupKindDef VC_Empire_Troopers;

        static PawnGroupKindDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(PawnKindDefOf));
    }
}