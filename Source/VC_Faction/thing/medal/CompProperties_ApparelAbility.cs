using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace VC_Faction
{
    public class CompProperties_ApparelAbility : CompProperties
    {
        public AbilityDef abilityDef;

        public CompProperties_ApparelAbility() => this.compClass = typeof(CompApparelAbility);
    }
}
