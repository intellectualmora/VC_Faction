using RimWorld;
using System.Linq;
using Verse;
using Verse.Noise;

namespace VC_Faction
{

    public class CompProperties_AbilityEffect_DeployMortar : CompProperties_AbilityEffect
    {
        public CompProperties_AbilityEffect_DeployMortar()
        {
            this.compClass = typeof(CompAbilityEffect_DeployMortar);
        }
    }

    public class CompAbilityEffect_DeployMortar : CompAbilityEffect
    {
        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);

            Pawn pawn = parent.pawn;
            Map map = pawn.Map;
            // 触发建造设计器
            ThingDef turretDef = ThingDefOf.VC_Portablemortar;
            Thing turret = ThingMaker.MakeThing(turretDef);

            // 包装成 MinifiedThing（便携式）
            MinifiedThing minified = turret.MakeMinified();

            // 添加到角色身上
            IntVec3 placeCell = pawn.Position;
            GenPlace.TryPlaceThing(minified, placeCell, map, ThingPlaceMode.Near);

            // 放置蓝图（可选立即安排安装）
            Find.Selector.ClearSelection();
            Find.Selector.Select(minified);

            Thing pack = pawn.inventory?.innerContainer?.FirstOrDefault(t => t.def.defName == "VC_PortablemortarPack");
            if (pack != null)
            {
                pawn.inventory.innerContainer.Remove(pack);
            }


        }
    }
}
