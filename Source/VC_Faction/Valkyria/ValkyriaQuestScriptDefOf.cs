using System;
using RimWorld;
using Verse;
namespace Valkyria
{
    [DefOf]
    public static class ValkyriaQuestScriptDefOf
    {
        public static QuestScriptDef OpportunitySite_AncientComplex_Valkyrur;
        static ValkyriaQuestScriptDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(ValkyriaQuestScriptDefOf));

    }
}
