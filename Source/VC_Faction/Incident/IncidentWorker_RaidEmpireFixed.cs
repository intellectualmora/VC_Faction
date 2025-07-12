using RimWorld;
using Verse;
using System.Linq;
using System;

namespace VC_Faction
{
    public class IncidentWorker_RaidEmpireFixed : IncidentWorker_RaidEnemy
    {
        protected override bool TryResolveRaidFaction(IncidentParms parms)
        {
            // 如果已经有派系就不修改
            if (parms.faction != null)
                return true;

            // 尝试获取名为 VC_Empire 的派系
            Faction empireFaction = Find.FactionManager.FirstFactionOfDef(FactionDefOf.VC_Empire);

            if (empireFaction != null && !empireFaction.defeated && empireFaction.HostileTo(Faction.OfPlayer))
            {
                parms.faction = empireFaction;
                return true;
            }

            // 获取失败则回退默认逻辑
            return base.TryResolveRaidFaction(parms);
        }

        public override void ResolveRaidStrategy(IncidentParms parms, PawnGroupKindDef groupKind)
        {
            parms.raidStrategy = DefDatabase<RaidStrategyDef>.AllDefs.FirstOrDefault(d=>d.defName == "StageThenAttack");
            parms.pawnGroupKind = PawnGroupKindDefOf.VC_Empire_Troopers;
        }

    }
}