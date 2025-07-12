using System;
using RimWorld;
using RimWorld.QuestGen;
using Verse;
namespace Valkyria
{
    public class QuestNode_Root_Loot_AncientComplex_Valkyria : QuestNode_Root_Loot_AncientComplex
    {
        protected override LayoutDef LayoutDef => ValkyriaLayoutDefOf.AncientComplex_Valkyria_Loot;

        protected override SitePartDef SitePartDef => ValkyriaSitePartDefOf.AncientComplex_Valkyria;
        protected override bool BeforeRunInt() => ModLister.CheckBiotech("Ancient Valkyria complex");

        protected override void RunInt()
        {
            Slate slate = RimWorld.QuestGen.QuestGen.slate;
            if (!slate.TryGet<bool>("discovered", out bool _))
                slate.Set<bool>("discovered", false);
            base.RunInt();
        }
    }
}
