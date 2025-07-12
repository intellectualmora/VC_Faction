using RimWorld.BaseGen;
using Verse;

namespace RimWorld
{
    public class GenStep_AncientComplex_Valkyria : GenStep_AncientComplex
    {
        protected override void GenerateComplex(Map map, ResolveParams parms)
        {
            RimWorld.BaseGen.BaseGen.globalSettings.map = map;
            RimWorld.BaseGen.BaseGen.symbolStack.Push("ancientValkyriaComplex", parms);
            RimWorld.BaseGen.BaseGen.Generate();
        }
    }
}