using System;
using RimWorld;
using Verse;
namespace Valkyria
{
    [DefOf]
    public static class ValkyriaLayoutDefOf
    {
        public static LayoutDef AncientComplex_Valkyria_Loot;
        static ValkyriaLayoutDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(ValkyriaLayoutDefOf));

    }
}
