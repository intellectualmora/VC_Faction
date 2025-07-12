using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace VC_Faction
{
    public class RavenAttackTransition : MusicTransition
    {
        public override bool IsTransitionSatisfied()
        {
            if (!base.IsTransitionSatisfied())
                return false;
            Map map = Find.CurrentMap;
            if (this.IsValidMap(map))
            {
                foreach (Pawn pawn in (IEnumerable<Pawn>)map.mapPawns.AllPawnsSpawned)
                {
                    if (pawn.Faction == null)
                    {
                        continue;
                    }
                    if (this.IsValidPawn(pawn))
                        return true;
                }
            }
            return false;
        }
        private bool IsValidMap(Map map) => map.mapPawns.ColonistsSpawnedCount > 0;

        private bool IsValidPawn(Pawn pawn)
        {
            Lord lord = pawn.GetLord();
            if (lord == null)
                return false;
            if (pawn.Faction.def.defName == FactionDefOf.VC_Empire.defName && pawn.kindDef.defName.Contains("Raven") && !pawn.kindDef.isBoss)
                return lord.LordJob is LordJob_StageThenAttack || lord.LordJob is LordJob_AssaultColony || lord.LordJob is LordJob_Siege;
            return false;
        }
    }
}
