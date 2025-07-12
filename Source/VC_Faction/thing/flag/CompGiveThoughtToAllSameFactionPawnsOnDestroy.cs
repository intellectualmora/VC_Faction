using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace VC_Faction
{
    public class CompGiveThoughtToAllSameFactionPawnsOnDestroy : ThingComp
    {
        private CompProperties_GiveThoughtToAllSameFactionPawnsOnDestroy Props => (CompProperties_GiveThoughtToAllSameFactionPawnsOnDestroy)this.props;

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            if (!this.Props.message.NullOrEmpty())
                Messages.Message(this.Props.message, (LookTargets)new TargetInfo(this.parent.Position, previousMap), MessageTypeDefOf.NegativeEvent);
            foreach (Pawn pawn in (IEnumerable<Pawn>)previousMap.mapPawns.AllHumanlikeSpawned) {
                if (pawn.Faction != null)
                {
                    if (pawn.Faction.def == this.Props.factionDef)
                    {
                        pawn.needs?.mood?.thoughts.memories.TryGainMemory(this.Props.thought1);
                    }
                    else if (pawn.Faction.HostileTo(Find.FactionManager.FirstFactionOfDef(this.Props.factionDef)))
                    {
                        pawn.needs?.mood?.thoughts.memories.TryGainMemory(this.Props.thought2);
                    }
                }
            }
        }
    }
 }
