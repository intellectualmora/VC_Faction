using System;
using RimWorld;
using Verse;
namespace Valkyria
{
    [DefOf]
    public static class ValkyriaKindDefOf
    {
        public static PawnKindDef Valkyria_Basic;
        static ValkyriaKindDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(ValkyriaKindDefOf));

    }
}
