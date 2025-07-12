using RimWorld;
using Verse;

namespace VC_Faction
{
    [DefOf]
    public static class PawnKindDefOf
    {
        public static PawnKindDef VC_Gallia_Fighter_Elite_Melee;
        public static PawnKindDef VC_Federation_Fighter_Elite_Melee;
        public static PawnKindDef VC_Empire_Fighter_Elite_Melee;

        public static PawnKindDef VC_Gallia_Fighter_Gunner;
        public static PawnKindDef VC_Federation_Fighter_Gunner;
        public static PawnKindDef VC_Empire_Fighter_Gunner;

        public static PawnKindDef VC_Militia;

        static PawnKindDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(PawnKindDefOf));
    }
}