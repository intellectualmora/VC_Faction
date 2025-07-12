using System;
using RimWorld;
using Verse;
namespace Valkyria
{
    [DefOf]
    public static class ValkyriaThingDefOf
    {
        public static ThingDef RelicSignalReceiver;
        public static ThingDef Spear_Valkyria;
        public static ThingDef Spear_Valkyria_Rie;
        static ValkyriaThingDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(ValkyriaThingDefOf));
    }
}
