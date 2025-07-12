using System;
using RimWorld;
using Verse;
namespace Valkyria
{
    [DefOf]
    public static class ValkyriaSitePartDefOf
    {
        public static SitePartDef AncientComplex_Valkyria;
        static ValkyriaSitePartDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(ValkyriaSitePartDefOf));

    }
}
