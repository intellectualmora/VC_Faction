using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace VC_Faction
{
    public class CompProperties_GiveThoughtToAllSameFactionPawnsOnDestroy : CompProperties
    {
        public ThoughtDef thought1;
        public ThoughtDef thought2;
        public FactionDef factionDef;
        [MustTranslate]
        public string message;
        public bool onlyWhenKilled;
        public bool ignoreOnVanish = false;

        public CompProperties_GiveThoughtToAllSameFactionPawnsOnDestroy() => this.compClass = typeof(CompGiveThoughtToAllSameFactionPawnsOnDestroy);

    }
}
