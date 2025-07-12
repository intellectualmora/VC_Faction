using System;
using RimWorld;
using RimWorld.BaseGen;
using Verse;
namespace Valkyria
{
    public class SymbolResolver_AncientComplex_Valkyria : SymbolResolver_AncientComplex_Base
    {
        protected override LayoutDef DefaultLayoutDef => ValkyriaLayoutDefOf.AncientComplex_Valkyria_Loot;

        public override void Resolve(ResolveParams rp)
        {
            ResolveParams resolveParams = rp;
            resolveParams.floorDef = TerrainDefOf.PackedDirt;
            RimWorld.BaseGen.BaseGen.symbolStack.Push("outdoorsPath", resolveParams);
            RimWorld.BaseGen.BaseGen.symbolStack.Push("ensureCanReachMapEdge", rp);
            this.ResolveComplex(rp);
        }
    }
}
